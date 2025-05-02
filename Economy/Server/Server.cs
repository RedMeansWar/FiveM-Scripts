using CitizenFX.Core;
using Common.Models;
using Common.Server;
using static CitizenFX.Core.Native.API;

namespace Economy.Server
{
    public class Server : ServerCommonScript
    {
        #region Variables
        internal bool _usingApi = true;
        #endregion
        
        #region Event Handlers
        [EventHandler("Economy:Server:CreateBankAccount")]
        private async void OnCreateBankAccount(BankAccount account) => await HttpHandler.CreateBankAccountAsync(account);
        
        [EventHandler("Economy:Server:DeleteBankAccount")]
        private async void OnDeleteBankAccount(int accountId) => await HttpHandler.DeleteBankAccountAsync(accountId);
        
        [EventHandler("Economy:Server:AddMoneyAsync")]
        private async void OnAddMoneyAsync(int accountId, string accountType, int money) => await HttpHandler.AddMoneyAsync(accountId, accountType, money);
        
        [EventHandler("Economy:Server:RemoveMoneyAsync")]
        private async void OnRemoveMoneyAsync(int accountId, string accountType, int money) => await HttpHandler.RemoveMoneyAsync(accountId, accountType, money);
        
        [EventHandler("Economy:Server:UpdateBankAccount")]
        private async void OnUpdateBankAccount(int accountId, BankAccount bankAccount) => await HttpHandler.UpdateBankAccountAsync(accountId, bankAccount);
        
        [EventHandler("Economy:Server:GetBankAccountByCharacterId")]
        private async void OnGetBankAccountByCharacterId(int characterId, string accountType) => await HttpHandler.GetBankAccountByCharacterIdAsync(characterId, accountType);
        
        [EventHandler("Economy:Server:GetBankAccountById")]
        private async void OnGetBankAccountById(int accountId) => await HttpHandler.GetBankAccountByAccountIdAsync(accountId);
        
        [EventHandler("Economy:Server:GetBankAccountByAccountType")]
        private async void OnGetBankAccountByAccountType(string accountType) => await HttpHandler.GetBankAccountByAccountType(accountType);
        
        [EventHandler("Economy:Server:UsingApi")]
        private void OnUseApi(bool usingApi) => _usingApi = usingApi;
        #endregion
    }
}