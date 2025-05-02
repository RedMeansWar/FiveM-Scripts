using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Server.Models;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Economy.Server
{
    internal class HttpHandler
    {
        #region Variables
        internal static string _jwt = "";
        internal static Character _character;
        internal static string _apiKey = GetConvar("api_key", "");
        internal static string _apiUrl = GetConvar("api_url", "http://localhost:5000");
        
        internal static Dictionary<string, string> _headers = new()
        {
            { "Content-Type", "application/json" },
            { "ApiUrl", _apiUrl },
            { "X-ApiKey-X", _apiKey }
        };
        #endregion
        
        #region Methods
        public static async Task<bool> LoginAsync(string username, string password)
        {
            var data = JsonConvert.SerializeObject(new
            {
                username = username,
                password = password
            });

            var response = await HttpHelper.PostAsync($"{_apiUrl}/api/auth/login", data, _headers);
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
        
        public static async Task<BankAccount> GetBankAccountByCharacterIdAsync(int characterId, string accountType)
        {
            var response = await HttpHelper.GetAsync($"{_apiUrl}/api/economy/getBankAccountByCharacterId/{characterId}/{accountType}");
            if (response is null || !response.IsSuccessStatusCode)
            {
                Log.InfoOrError($"Failed to fetch bank account from API for Character ID: {characterId}.", "API");
                return null;
            }
            
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BankAccount>(json);
        }

        public static async Task<BankAccount> CreateBankAccountAsync(BankAccount bankAccount)
        {
            string data = JsonConvert.SerializeObject(bankAccount);
            
            var response = await HttpHelper.PostAsync($"{_apiUrl}/economy/api/createBankAccount", data, _headers);
            if (response is null || !response.IsSuccessStatusCode)
            {
                Log.InfoOrError($"Failed to create bank account using API due to a failed invalid data or a bad respond code. {response.StatusCode}", "API");
                return null;
            }
            
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BankAccount>(json);
        }

        public static async Task<BankAccount> UpdateBankAccountAsync(int accountId, BankAccount bankAccount)
        {
            string data = JsonConvert.SerializeObject(bankAccount);
            
            var response = await HttpHelper.PostAsync($"{_apiUrl}/api/economy/updateBankAccount/{accountId}", data, _headers);
            if (response is null || !response.IsSuccessStatusCode)
            {
                Log.InfoOrError($"Failed to update bank account using API due to a invalid data or a bad respond code. {response.StatusCode}", "API");
                return null;
            }
            
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BankAccount>(json);
        }

        public static async Task<bool> DeleteBankAccountAsync(int accountId)
        {
            var response = await HttpHelper.DeleteAsync($"{_apiUrl}/api/economy/deleteBankAccount/{accountId}");
            if (response is not null && response.IsSuccessStatusCode) return true;
            
            Log.InfoOrError($"Failed to delete bank account {accountId} using API due to a invalid data or a bad respond code. {response.StatusCode}", "API");
            return false;
        }

        public static async Task<BankAccount> GetBankAccountByAccountIdAsync(int accountId)
        {
            var response = await HttpHelper.GetAsync($"{_apiUrl}/api/economy/getBankAccountByAccountId/{accountId}");
            if (response is null || !response.IsSuccessStatusCode)
            {
                Log.InfoOrError($"Failed to get bank account {accountId} using API due to a invalid data or a bad response code. {response.StatusCode}", "API");
                return null;
            }
            
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BankAccount>(json);
        }

        public static async Task<bool> AddMoneyAsync(int accountId, string accountType, int amount)
        {
            var payload = new { amount = amount, accountType = accountType };
            string data = JsonConvert.SerializeObject(payload);
            
            var response = await HttpHelper.PostAsync($"{_apiUrl}/api/economy/addMoney", data, _headers);
            if (response is not null && response.IsSuccessStatusCode) return true;
            
            Log.InfoOrError($"Failed to add money to account {accountId} using API due to a invalid data or a bad response code. {response.StatusCode}");
            return false;
        }

        public static async Task<bool> RemoveMoneyAsync(int accountId, string accountType, int amount)
        {
            var payload = new { amount = amount, accountType = accountType };
            string data = JsonConvert.SerializeObject(payload);
            
            var response = await HttpHelper.PostAsync($"{_apiUrl}/api/economy/removeMoney", data, _headers);
            if (response is not null && response.IsSuccessStatusCode) return true;
            
            Log.InfoOrError($"Failed to remove money from account {accountId} using API due to a invalid data or a bad response code. {response.StatusCode}");
            return false;
        }

        public static async Task<BankAccount> GetBankAccountByAccountType(string accountType)
        {
            string data = JsonConvert.SerializeObject(accountType);

            var response = await HttpHelper.GetAsync($"{_apiUrl}/api/economy/getBankAccountByAccountType/{accountType}");
            if (response is null || !response.IsSuccessStatusCode)
            {
                Log.InfoOrError($"Failed to get bank account from type {accountType} using API due to invalid data or a bad response code. {response.StatusCode}");
                return null;
            }
            
            return JsonConvert.DeserializeObject<BankAccount>(data);
        }
        #endregion
    }
}