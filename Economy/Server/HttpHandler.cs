using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Server.Models;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Economy.Server
{
    public class HttpHandler
    {
        #region Variables
        internal string _jwt = "";
        internal Character _character;
        internal static string _apiKey = GetConvar("api_key", "");
        internal static string _apiUrl = GetConvar("api_url", "http://localhost:5000/");

        internal Dictionary<string, string> _headers = new()
        {
            { "Content-Type", "application/json" },
            { "ApiUrl", _apiUrl },
            { "X-ApiKey-X", _apiKey }
        };
        #endregion
        
        #region Methods
        public async Task<bool> LoginAsync(string username, string password)
        {
            var data = JsonConvert.SerializeObject(new
            {
                username = username,
                password = password
            });

            var response = await HttpHelper.PostAsync("http://localhost:3000/auth/login", data, _headers);
            if (response?.IsSuccessStatusCode == true)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                _jwt = result.access_token;
                Log.InfoOrError($"Login: Token received: {_jwt}", "API");
                return true;
            }
            else
            {
                Log.InfoOrError("Login: Failed to authenticate.", "API");
                return false;
            }
        }


        public async Task<List<BankAccount>> GetBankAccountsByCharIdAsync(int characterId)
        {
            return _character.BankAccounts;
        }
        #endregion
    }
}