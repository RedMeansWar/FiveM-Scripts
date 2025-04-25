using System;

namespace Common.Models
{
    public class Character
    {
        public int CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DoB { get; set; }
        public float CashOnHand {  get; set; }
        public float BankAmount { get; set; }
        public string Department { get; set; }
    }
}