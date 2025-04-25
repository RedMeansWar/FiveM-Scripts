using System;
using CitizenFX.Core;
using Newtonsoft.Json;

namespace Common
{
    public static class Json
    {
        /// <summary>
        /// Parses a JSON string into a reference type (class).
        /// </summary>
        /// <typeparam name="T">The type of the class to deserialize into.</typeparam>
        /// <param name="json">The JSON string to be deserialized. Can be null, empty, or whitespace.</param>
        /// <returns>
        /// An instance of the class type <typeparamref name="T"/> populated with the data from the JSON string. 
        /// Returns <c>null</c> if the JSON string is null, whitespace, or if deserialization fails.
        /// </returns>
        public static T Parse<T>(string json) where T : class
        {
            // Check if the input JSON string is null, empty, or contains only whitespace.
            // If so, return null as there is no valid JSON to process.
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            T obj;

            try
            {
                // Define deserialization settings to handle reference loops by ignoring them.
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // Attempt to deserialize the JSON string into an object of type T.
                obj = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception ex)
            {
                // Log the JSON string to the debug output to assist in troubleshooting.
                Debug.WriteLine(json);

                // Log the exception message to the debug output for error details.
                Debug.WriteLine(ex.Message);

                // Log the exception stack trace for additional debugging context.
                Debug.WriteLine(ex.StackTrace);

                // If deserialization fails, set the result to null.
                obj = null;
            }

            // Return the deserialized object, or null if deserialization failed.
            return obj;
        }


        /// <summary>
        /// Parses a JSON string into a value type (struct).
        /// </summary>
        /// <typeparam name="T">The type of the struct to deserialize into.</typeparam>
        /// <param name="json">The JSON string to be deserialized. Can be null, empty, or whitespace.</param>
        /// <returns>
        /// An instance of the struct type <typeparamref name="T"/> populated with the data from the JSON string. 
        /// Returns the default value of <typeparamref name="T"/> if the JSON string is null, whitespace, or if deserialization fails.
        /// </returns>
        public static T ParseStruct<T>(string json) where T : struct
        {
            // Check if the input JSON string is null, empty, or whitespace.
            // If so, return the default value of the struct type.
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            T obj;

            try
            {
                // Define deserialization settings to handle reference loops by ignoring them.
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // Attempt to deserialize the JSON string into an object of type T.
                obj = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception ex)
            {
                // Log the JSON string to the debug output for troubleshooting.
                Debug.Write(json);

                // Log the exception message to the debug output to identify the error.
                Debug.WriteLine(ex.Message);

                // Log the stack trace for additional debugging context.
                Debug.WriteLine(ex.StackTrace);

                // If deserialization fails, return the default value of the struct type.
                obj = default;
            }

            // Return the deserialized object, or the default value if deserialization failed.
            return obj;
        }

        /// <summary>
        /// Converts an object to its JSON string representation.
        /// </summary>
        /// <param name="data">The object to be serialized to JSON. Can be null.</param>
        /// <returns>
        /// A JSON string representing the object. 
        /// Returns <c>null</c> if the input is <c>null</c> or if an error occurs during serialization.
        /// </returns>
        public static string Stringify(object data)
        {
            // Check if the input object is null. If null, return null immediately.
            if (data is null)
            {
                return null;
            }

            string json;

            try
            {
                // Define serialization settings to handle reference loops by ignoring them.
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // Attempt to serialize the object to a JSON string.
                json = JsonConvert.SerializeObject(data, settings);
            }
            catch (Exception ex)
            {
                // Log the exception message to the debug output for troubleshooting.
                Debug.WriteLine(ex.Message);

                // Log the exception stack trace to provide more context for debugging.
                Debug.WriteLine(ex.StackTrace);

                // If serialization fails, set the JSON result to null.
                json = null;
            }

            // Return the JSON string, or null if serialization failed.
            return json;
        }

    }
}