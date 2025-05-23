using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class Character
    {
        public int CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DoB { get; set; }
        public int Cash {  get; set; }
        public int Bank { get; set; }
        public string Department { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
        
    }
}