namespace Sme.BankingApi.Data.Model
{
    public enum AccountStatus: byte { Opened = 0, Closed = 1 }

    public enum Currency : byte { EUR = 0, USD = 1 }

    public class AccountModel
    {
        public string Number { get; set; }

        public AccountStatus Status { get; set; }

        public Currency Currency { get; set; }

        public decimal Balance { get; set; }

        public int CustomerId { get; set; }

        public virtual CustomerModel Customer { get; set; }
    }
}