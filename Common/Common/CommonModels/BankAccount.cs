using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class BankAccount
    {
        public int AccountId { get; set; }
        public int CharacterId { get; set; }
        public string BankAccountId { get; set; }
        public int Balance { get; set; }
        public AccountType AccountType { get; set; }

        public BankAccount(int characterId, int accountId, string bankAccountId, AccountType accountType)
        {
            AccountId = accountId;
            BankAccountId = bankAccountId;
            CharacterId = characterId;
            AccountType = accountType;
            Balance = 0;
        }

        
    }

    public enum AccountType
    {
        Saving,
        Checking
    }
}