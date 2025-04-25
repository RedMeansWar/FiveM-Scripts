using System;
using System.Collections.Generic;

namespace Common
{
    public static class Extensions
    {
        /// <summary>
        /// Attempts to retrieve a value of type <typeparamref name="T"/> from the dictionary by its key.
        /// </summary>
        /// <param name="dict">The dictionary from which to retrieve the value.</param>
        /// <param name="key">The key associated with the value to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key does not exist or the value is not of type <typeparamref name="T"/>.</param>
        /// <returns>
        /// The value associated with the specified <paramref name="key"/> if it exists and is of type <typeparamref name="T"/>; 
        /// otherwise, the <paramref name="defaultVal"/>.
        /// </returns>
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            // Attempt to get the value associated with the key.
            if (dict.TryGetValue(key, out object value) && value is T t)
            {
                return t; // Return the value if it matches the expected type.
            }

            return defaultVal; // Return the default value if the key is not found or the value type doesn't match.
        }

        /// <summary>
        /// Retrieves the value associated with the specified key from the dictionary.
        /// If the key does not exist, a new instance of the value type is created, added to the dictionary, and returned.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary. Must have a parameterless constructor.</typeparam>
        /// <param name="dict">The dictionary from which to retrieve the value.</param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <returns>
        /// The value associated with the specified key, or a new instance of <typeparamref name="TValue"/> if the key does not exist.
        /// </returns>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            // Check if the key exists in the dictionary.
            if (!dict.ContainsKey(key))
            {
                // If the key does not exist, create a new instance of TValue and add it to the dictionary.
                dict[key] = new TValue();
            }

            // Return the value associated with the specified key.
            return dict[key];
        }

        /// <summary>
        /// Clamps a value within a specified range. If the value is less than the minimum, the minimum is returned. If the value is greater than the maximum, the maximum is returned. Otherwise, the original value is returned.
        /// </summary>
        /// <typeparam name="T">The type of the value to clamp. Must implement <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="In">The value to be clamped.</param>
        /// <param name="Min">The minimum value of the range.</param>
        /// <param name="Max">The maximum value of the range.</param>
        /// <returns>The clamped value, which will be between <paramref name="Min"/> and <paramref name="Max"/>.</returns>
        public static T Clamp<T>(T In, T Min, T Max) where T : IComparable<T>
        {
            // If the value is less than the minimum, return the minimum.
            if (In.CompareTo(Min) < 0)
            {
                return Min;
            }
            // If the value is greater than the maximum, return the maximum.
            else if (In.CompareTo(Max) > 0)
            {
                return Max;
            }

            // Otherwise, return the value as it is within the range.
            return In;
        }

    }
}