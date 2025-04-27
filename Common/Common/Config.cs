using System;
using SharpConfig;

namespace Common
{
    public static class Config
    {
        /// <summary>
        /// Retrieves a configuration value from the specified section and key.
        /// </summary>
        /// <typeparam name="T">The expected type of the configuration value.</typeparam>
        /// <param name="configData">The string containing the configuration data.</param>
        /// <param name="sectionName">The name of the section containing the key.</param>
        /// <param name="keyName">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not found or cannot be converted.</param>
        /// <returns>The configuration value, or the default value if not found or invalid.</returns>
        public static T GetValue<T>(string configData, string sectionName, string keyName, T defaultValue)
        {
            try
            {
                Configuration config = Configuration.LoadFromString(configData);
                if (!config.Contains(sectionName)) // check for the section
                {
                    Log.InfoOrError($"Warning: Section '{sectionName}' not found. Returning default value for {sectionName}:{keyName}.", "CONFIG");
                    return defaultValue;
                }

                Section section = config[sectionName];
                if (!section.Contains(keyName)) // check if the section has a specific key
                {
                    Log.InfoOrError($"Warning: Key '{keyName}' not found in section '{sectionName}'. Returning default value.", "CONFIG");
                    return defaultValue;
                }

                return config[sectionName][keyName].GetValue<T>();
            }
            catch (Exception ex)
            {
                Log.Error($"Could not retrieve/convert value of key '{keyName}' in section '{sectionName}' to type '{typeof(T).Name}'. Returning default value.", "CONFIG ERROR", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Checks if a configuration key exists in the specified section.
        /// </summary>
        /// <param name="configData">The string containing the configuration data.</param>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public static bool KeyExists(string configData, string sectionName, string keyName)
        {
            try
            {
                // setup a configuration from a loaded string
                Configuration config = Configuration.LoadFromString(configData);
                if (!config.Contains(sectionName)) // check for the section
                {
                    // if nothing returns: return as false
                    return false;
                }

                Section section = config[sectionName];

                // return true if the section contains key
                return section.Contains(keyName);
            }
            catch (Exception ex)
            {
                Log.Error($"Could not check the existence of key '{keyName}' in section '{sectionName}'.  Exception: {ex.Message}", "CONFIG ERROR", ex);
                return false;
            }
        }
    }
}