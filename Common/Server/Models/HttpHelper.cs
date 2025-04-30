using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Common.Server.Models
{
    public class HttpHelper : ServerCommonScript
    {
        #region General Variables
        internal static readonly Dictionary<int, Dictionary<string, dynamic>> _responseDictionary = new();
        #endregion

        #region Event Handlers
        [EventHandler("__cfx_internal:httpResponse")]
        internal void OnHttpResponse(int token, int status, string content, dynamic header, string errorContent)
        {
            Dictionary<string, dynamic> response = new()
            {
                ["headers"] = header,
                ["status"] = status,
                ["content"] = content ?? errorContent
            };

            _responseDictionary[token] = response;
        }
        #endregion

        #region HTTP Methods
        /// <summary>
        /// Sends an asynchronous GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <param name="headers">Optional. A dictionary of headers to include in the request.</param>
        /// <returns>An HttpResponseMessage representing the response from the HTTP request.</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> headers = null)
            => await Http(url, "GET", null, headers);

        /// <summary>
        /// Sends an asynchronous POST request to the specified URL with the provided data.
        /// </summary>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="headers">A dictionary of headers to include in the request.</param>
        /// <returns>An HttpResponseMessage representing the response from the HTTP request.</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, string data, IDictionary<string, string> headers = null)
            => await Http(url, "POST", data, headers);

        /// <summary>
        /// Sends an asynchronous PUT request to the specified URL with the provided data.
        /// </summary>
        /// <param name="url">The URL to send the PUT request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="headers">A dictionary of headers to include in the request.</param>
        /// <returns>An HttpResponseMessage representing the response from the HTTP request.</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, string data, IDictionary<string, string> headers = null)
            => await Http(url, "PUT", data, headers);

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the DELETE request to.</param>
        /// <param name="headers">Optional. A dictionary of headers to include in the request.</param>
        /// <returns>An HttpResponseMessage representing the response from the HTTP request.</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string> headers = null)
            => await Http(url, "DELETE", null, headers);
        #endregion

        #region Helper Functions
        /// <summary>
        /// Converts an integer status code into an HttpStatusCode enum.
        /// </summary>
        /// <param name="status">The integer value representing the HTTP status code.</param>
        /// <returns>The corresponding HttpStatusCode enum.</returns>
        internal static HttpStatusCode ParseStatusInternal(int status)
        {
            // Converts the integer status code to an HttpStatusCode enum value.
            return (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), status);
        }

        /// <summary>
        /// Parses a request response dictionary and converts it into a RequestResponse object.
        /// </summary>
        /// <param name="requestResponse">A dictionary containing the response details (status, headers, and content).</param>
        /// <returns>A RequestResponse object populated with the parsed status, headers, and content.</returns>
        internal static RequestResponse ParseRequestResponseInternal(IDictionary<string, dynamic> requestResponse)
        {
            // Check if the provided requestResponse is a valid dictionary.
            if (requestResponse is IDictionary<string, dynamic>)
            {
                // Return a new RequestResponse object with parsed status, headers, and content.
                return new RequestResponse
                {
                    status = ParseStatusInternal(requestResponse["status"]),   // Parse the status code.
                    headers = ParseHeadersInternal(requestResponse["headers"]), // Parse the headers.
                    content = requestResponse["content"]  // Get the content from the dictionary.
                };
            }
            else
            {
                // Return a default RequestResponse with a 500 status code (Internal Notes.Server Error).
                return new RequestResponse
                {
                    status = HttpStatusCode.InternalServerError,   // Default to InternalServerError status.
                    headers = new WebHeaderCollection(),           // Empty headers collection.
                    content = ""                                   // Empty content.
                };
            }
        }


        /// <summary>
        /// Converts a dynamic object (typically a dictionary) into a WebHeaderCollection.
        /// </summary>
        /// <param name="headerDyn">A dynamic object representing HTTP headers as key-value pairs.</param>
        /// <returns>A WebHeaderCollection containing the parsed headers.</returns>
        internal static WebHeaderCollection ParseHeadersInternal(dynamic headerDyn)
        {
            // Initialize a new WebHeaderCollection to store the parsed headers.
            WebHeaderCollection headers = new();

            // Cast the dynamic object to a dictionary of string keys and object values.
            IDictionary<string, object> headerDict = (IDictionary<string, object>)headerDyn;

            // Iterate through each key-value pair in the dictionary.
            foreach (KeyValuePair<string, object> entry in headerDict)
            {
                // Add each header to the WebHeaderCollection, converting the value to a string.
                headers.Add(entry.Key, entry.Value.ToString());
            }

            // Return the populated WebHeaderCollection.
            return headers;
        }

        /// <summary>
        /// Performs an HTTP request and waits for the response asynchronously.
        /// </summary>
        /// <param name="url">The URL for the HTTP request.</param>
        /// <param name="method">The HTTP method (e.g., "GET", "POST").</param>
        /// <param name="data">The data to send with the request (e.g., body of the request).</param>
        /// <param name="headers">The headers to include in the request.</param>
        /// <returns>A dictionary containing the response data, or null if the request fails or times out.</returns>
        internal static async Task<HttpResponseMessage> Http(string url, string method, string data, dynamic headers)
        {
            // Prepare the request data encapsulated in a RequestDataInternal object.
            RequestDataInternal requestData = new RequestDataInternal
            {
                url = url,
                method = method,
                data = data is null ? "" : data,
                headers = headers is null ? "" : headers
            };

            try
            {
                int timeout = 50; // Set timeout to 50 iterations (5000 ms or 5 seconds)
                string json = Json.Stringify(requestData);  // Convert request data to a JSON string.

                // Perform the HTTP request using a custom API method and get a unique token.
                int token = API.PerformHttpRequestInternal(json, json.Length);

                // Poll the response dictionary for the result, checking every 100ms.
                while (!_responseDictionary.ContainsKey(token) && timeout > 0)
                {
                    timeout--; // Decrease timeout counter
                    await Delay(100); // Wait for 100ms before trying again
                }

                // If the response was not found after the timeout expired, log the issue and return null.
                if (!_responseDictionary.ContainsKey(token) && timeout == 0)
                {
                    Debug.WriteLine($"HTTP request to {url} timed out after 10 seconds. Request body:");
                    Debug.WriteLine(data); // Log the request data for debugging purposes.
                    return null;
                }

                // Retrieve the response dictionary associated with the token.
                Dictionary<string, dynamic> res = _responseDictionary[token];

                // Clean up the dictionary by removing the processed token.
                _responseDictionary.Remove(token);

                // Return the response dictionary.
                return BuildHttpResponse(ParseRequestResponseInternal(res));
            }
            catch
            {
                // In case of an exception (e.g., network failure), return null.
                return null;
            }
        }

        /// <summary>
        /// Builds an <see cref="HttpResponseMessage"/> from the provided <see cref="RequestResponse"/> data.
        /// </summary>
        /// <param name="data">The <see cref="RequestResponse"/> object containing the status code, headers, and content.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> representing the HTTP response.</returns>
        internal static HttpResponseMessage BuildHttpResponse(RequestResponse data)
        {
            try
            {
                // Create a new HttpResponseMessage with the status code from the RequestResponse data.
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(data.status);

                try
                {
                    // Check if there are any headers in the RequestResponse data.
                    if (data.headers != null && data.headers.Count > 0)
                    {
                        // Iterate through each header in the collection.
                        for (int i = 0; i < data.headers.Count; i++)
                        {
                            // Get the key and value of the current header.
                            string key = data.headers.GetKey(i);
                            string val = data.headers.Get(i);

                            // Check if the header key is not null and contains the specific string "Phoenix".
                            if (key != null && key.Contains("Phoenix"))
                            {
                                // Add the header to the HttpResponseMessage's headers collection.
                                httpResponseMessage.Headers.Add(key, val);
                            }
                            // Headers that do not contain "Phoenix" in their key are intentionally skipped.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log any error that occurs while trying to read or process the response headers.
                    Log.Error($"HTTP Error - Error reading response headers: {ex.Message}");
                    Log.Error($"HTTP Error - {ex.Message}\n{ex.ToString()}");
                }

                // Set the content of the HttpResponseMessage. If data.content is not null, create a StringContent; otherwise, create an empty StringContent.
                httpResponseMessage.Content = (data.content is not null) ? new StringContent(data.content) : new(string.Empty);
                return httpResponseMessage; // Return the constructed HttpResponseMessage.
            }
            catch (Exception ex)
            {
                // Log any general exception that occurs during the process of building the HttpResponseMessage.
                Log.Error($"HTTP Error - {ex.Message}\n{ex.ToString()}");

                // If an exception occurs, return an HttpResponseMessage with an InternalServerError status code and the error message as content.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
        #endregion
    }
}
