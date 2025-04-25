using System;
using System.Diagnostics;
using CitizenFX.Core.Native;

namespace Common.Client
{
    public class Convar
    {
        #region Variables
        internal static string _context;
        #endregion

        #region Methods
        /// <summary>
        /// Sets the current context for resource operations.
        /// </summary>
        /// <param name="context">The context to set.</param>
        public static void SetContext(string context) => _context = context;

        /// <summary>
        /// Clears the current context.
        /// </summary>
        public static void UnsetContext() => _context = "";

        /// <summary>
        /// Sets a resource KVP value by name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="name">The name of the key.</param>
        /// <param name="value">The value to set.</param>
        public static void Set<T>(string name, T value) where T : IConvertible => API.SetResourceKvp(name, value.ToString());

        /// <summary>
        /// Sets a resource KVP value by prefix and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="prefix">The prefix to prepend to the name.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="value">The value to set.</param>
        public static void Set<T>(string prefix, string name, T value) where T : IConvertible => Set($"{prefix}{name}", value);

        /// <summary>
        /// Sets a resource KVP value with the current context and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="name">The name of the key.</param>
        /// <param name="value">The value to set.</param>
        public static void CSet<T>(string name, T value) where T : IConvertible => Set($"{_context}{name}", value);

        /// <summary>
        /// Sets a resource KVP value with the current context, prefix, and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="prefix">The prefix to prepend to the name.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="value">The value to set.</param>
        public static void CSet<T>(string prefix, string name, T value) where T : IConvertible => Set($"{_context}{prefix}{name}", value);

        /// <summary>
        /// Gets a resource KVP value with a prefix and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="prefix">The prefix to prepend to the name.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The retrieved value, or the default value if the key does not exist.</returns>
        public static T Get<T>(string prefix, string name, T defaultValue) where T : IConvertible => Get<T>($"{prefix}{name}", defaultValue);

        /// <summary>
        /// Gets a resource KVP value with the current context and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="name">The name of the key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The retrieved value, or the default value if the key does not exist.</returns>
        public static T CGet<T>(string name, T defaultValue) where T : IConvertible => Get<T>($"{_context}{name}", defaultValue);

        /// <summary>
        /// Gets a resource KVP value with the current context, prefix, and name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="prefix">The prefix to prepend to the name.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The retrieved value, or the default value if the key does not exist.</returns>
        public static T CGet<T>(string prefix, string name, T defaultValue) where T : IConvertible => Get<T>($"{_context}{prefix}{name}", defaultValue);

        /// <summary>
        /// Gets a resource KVP value by name.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IConvertible"/>.</typeparam>
        /// <param name="name">The name of the key.</param>
        /// <param name="defaultVal">The default value to return if the key is not found.</param>
        /// <returns>The retrieved value, or the default value if the key does not exist.</returns>
        public static T Get<T>(string name, T defaultVal) where T : IConvertible
        {
            // Attempt to retrieve the raw string value associated with the given key name
            string raw = API.GetResourceKvpString(name);

            // Check if the retrieved value is not null or empty
            if (!string.IsNullOrEmpty(raw))
            {
                try
                {
                    // Convert the raw string value to the specified type T and return it
                    return (T)Convert.ChangeType(raw, typeof(T));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // If the key is not found or conversion fails, return the provided default value
            return defaultVal;
        }
        #endregion
    }
}