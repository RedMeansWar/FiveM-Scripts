using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Server
{
    public class CallbackHandlerEntry
    {
        #region Variables
        internal readonly HashSet<Delegate> _callbacks = new();
        #endregion

        #region Methods
        /// <summary>
        /// Adds a delegate action to the callback handler entry.
        /// </summary>
        /// <param name="entry">The callback handler entry to add the action to.</param>
        /// <param name="action">The delegate action to be added.</param>
        /// <returns>The updated <see cref="CallbackHandlerEntry"/> with the new action.</returns>
        public static CallbackHandlerEntry operator +(CallbackHandlerEntry entry, Delegate action)
        {
            entry._callbacks.Add(action);
            return entry;
        }

        /// <summary>
        /// Removes a delegate action from the callback handler entry.
        /// </summary>
        /// <param name="entry">The callback handler entry to remove the action from.</param>
        /// <param name="action">The delegate action to be removed.</param>
        /// <returns>The updated <see cref="CallbackHandlerEntry"/> with the action removed.</returns>
        public static CallbackHandlerEntry operator -(CallbackHandlerEntry entry, Delegate action)
        {
            entry._callbacks.Remove(action);
            return entry;
        }

        /// <summary>
        /// Clears all callbacks from the callback handler entry.
        /// </summary>
        public void Destroy() => _callbacks.Clear();

        /// <summary>
        /// Invokes all registered callbacks asynchronously.
        /// </summary>
        /// <param name="args">Arguments to pass to the callbacks.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Invoke(params object[] args)
        {
            // Copy callbacks to an array to avoid modification during iteration.
            Delegate[] callbacks = _callbacks.ToArray();
            Delegate[] array = callbacks;

            foreach (Delegate action in array)
            {
                // Dynamically invoke the delegate with the given arguments.
                object resval = action.DynamicInvoke(args);

                // If the result is a Task, await it.
                if (resval != null && resval is Task task)
                {
                    await task;
                }
            }
        }
        #endregion
    }

    public class CallbackHandlerDictionary : Dictionary<string, CallbackHandlerEntry>
    {
        /// <summary>
        /// Gets or sets a <see cref="CallbackHandlerEntry"/> based on the provided key.
        /// </summary>
        /// <param name="key">The key to retrieve or set the callback handler entry. The key is converted to lowercase.</param>
        /// <returns>
        /// The <see cref="CallbackHandlerEntry"/> associated with the provided key. 
        /// If the key does not exist, a new <see cref="CallbackHandlerEntry"/> is created, added, and returned.
        /// </returns>
        public new CallbackHandlerEntry this[string key]
        {
            get
            {
                // Convert the key to lowercase for case-insensitive lookup
                key = key.ToLower();

                // If the dictionary contains the key, return the associated entry
                if (ContainsKey(key))
                {
                    return base[key];
                }

                // Otherwise, create a new CallbackHandlerEntry, add it to the dictionary, and return it
                CallbackHandlerEntry entry = new();
                Add(key, entry);
                return entry;
            }
            set
            {
                // Currently, the setter is empty, but it can be implemented as needed
            }
        }
    }
}