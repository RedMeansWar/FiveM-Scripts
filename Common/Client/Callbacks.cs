using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Common.Models;

namespace Common.Client
{
    public class Callbacks : BaseScript
    {
        #region Variables
        internal static string _callbackClientEvent = "Common:Client:InternalCallback:";
        internal static string _callbackServerEvent = "Common:Server:InternalCallback:";
        #endregion

        #region Properties
        /// <summary>
        /// Dictionary which contains the current callbacks
        /// </summary>
        internal CallbackHandlerDictionary CurrentCallbackHandlers { get; set; } = new();
        #endregion

        #region Methods
        /// <summary>
        /// Triggers an event on the client and registers a callback for the response
        /// </summary>
        /// <param name="eventName">The name of the client event to trigger.</param>
        /// <param name="callback">The delegate representing the callback function to execute after the event is handled.</param>
        /// <param name="args">An array of arguments to pass to the event.</param>
        public void TriggerClientCallback(string eventName, Delegate callback, params object[] args)
        {
            // Generate a unique identifier for the callback function.
            string callbackGuid = RegisterCallback(callback);

            // Trigger the specified client event, passing the callback identifier and any additional arguments.
            TriggerEvent(eventName, callbackGuid, args);
        }

        /// <summary>
        /// Triggers an event on the server and registers a callback for the response
        /// </summary>
        /// <param name="eventName">The name of the server event to trigger.</param>
        /// <param name="callback">The delegate representing the callback function to execute once the server processes the event.</param>
        /// <param name="args">An array of arguments to pass to the server event.</param>
        public void TriggerServerCallback(string eventName, Delegate callback, params object[] args)
        {
            // Generate a unique identifier for the callback function, linking the event with this callback.
            string callbackGuid = RegisterCallback(callback);

            // Trigger the specified server event, passing the callback identifier and any additional arguments.
            TriggerServerEvent(eventName, callbackGuid, args);
        }

        /// <summary>
        /// Returns a client callback specified by the callbacks GUID
        /// </summary>
        /// <param name="callbackGuid">The unique identifier for the callback, used to locate and execute the correct client-side callback function.</param>
        /// <param name="args">An array of arguments to pass back to the client with the callback result.</param>
        public void ReturnClientCallback(string callbackGuid, params object[] args) => TriggerEvent($"{_callbackClientEvent}{callbackGuid}", callbackGuid, args);

        /// <summary>
        /// Returns a server callback specified by the callbacks GUID
        /// </summary>
        /// <param name="callbackGuid">The unique identifier for the callback, used to locate and execute the correct server-side callback function.</param>
        /// <param name="args">An array of arguments to pass back to the server with the callback result.</param>
        public void ReturnServerCallback(string callbackGuid, params object[] args) => TriggerServerEvent($"{_callbackServerEvent}{callbackGuid}", callbackGuid, args);

        /// <summary>
        /// Registers a callback function and associates it with a unique identifier (GUID).
        /// </summary>
        /// <param name="callback">The delegate representing the callback function to be registered.</param>
        /// <returns>
        /// A unique identifier (GUID) for the registered callback, which can be used to trigger the callback later.
        /// </returns>
        public string RegisterCallback(Delegate callback)
        {
            // Generate a new GUID to uniquely identify this callback.
            string callbackGuid = Guid.NewGuid().ToString();

            // Add the callback to the current callback handlers, using the GUID as the key.
            CurrentCallbackHandlers[callbackGuid] += callback;

            // Register an event handler for the client event associated with this callback GUID,
            // linking it to the method that handles the callback.
            EventHandlers.Add($"{_callbackClientEvent}{callbackGuid}", new Action<string, List<object>>(HandleCallback));

            // Return the unique identifier for the registered callback.
            return callbackGuid;
        }

        /// <summary>
        /// Handles the execution of a callback identified by the provided GUID and passes the specified arguments to it.
        /// </summary>
        /// <param name="callbackGuid">The unique identifier for the callback that is to be invoked.</param>
        /// <param name="args">A list of arguments to pass to the callback function when it is invoked.</param>
        public async void HandleCallback(string callbackGuid, List<object> args)
        {
            // Invoke the callback function associated with the provided GUID, passing the arguments as an array.
            await CurrentCallbackHandlers[callbackGuid].Invoke(args.ToArray());

            // Perform cleanup for the callback, removing it from the current handlers.
            CallbackCleanup(callbackGuid);
        }

        /// <summary>
        /// Cleans up resources associated with a callback identified by the provided GUID.
        /// </summary>
        /// <param name="callbackGuid">The unique identifier for the callback that is to be cleaned up.</param>
        private void CallbackCleanup(string callbackGuid)
        {
            // Destroy the callback handler associated with the provided GUID to release any resources it may be using.
            CurrentCallbackHandlers[callbackGuid].Destroy();

            // Remove the callback from the current callback handlers to prevent further invocations.
            CurrentCallbackHandlers.Remove(callbackGuid);

            // Remove the event handler registered for the client event associated with the callback GUID.
            EventHandlers.Remove($"{_callbackClientEvent}{callbackGuid}");
        }
        #endregion
    }
}