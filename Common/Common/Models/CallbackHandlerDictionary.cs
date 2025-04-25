using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CallbackHandlerDictionary : Dictionary<string, CallbackHandlerEntry>
    {
        public new CallbackHandlerEntry this[string key]
        {
            get
            {
                key = key.ToLower();
                if (this.ContainsKey(key))
                {
                    return base[key];
                }

                CallbackHandlerEntry entry = new CallbackHandlerEntry();
                base.Add(key, entry);
                return entry;
            }

            set
            {
                // No override
            }
        }
    }

    public class CallbackHandlerEntry
    {
        private readonly HashSet<Delegate> _callbacks = new();

        public static CallbackHandlerEntry operator +(CallbackHandlerEntry entry, Delegate action)
        {
            entry._callbacks.Add(action);
            return entry;
        }

        public static CallbackHandlerEntry operator -(CallbackHandlerEntry entry, Delegate action)
        {
            entry._callbacks.Remove(action);
            return entry;
        }

        public void Destroy()
        {
            _callbacks.Clear();
        }

        public async Task Invoke(params object[] args)
        {
            var callbacks = _callbacks.ToArray();
            foreach (Delegate action in callbacks)
            {
                var resval = action.DynamicInvoke(args);
                if (resval is not null && resval is Task task)
                {
                    await task;
                }
            }
        }
    }
}