namespace Sme.BankingApi.Data.Model
{
    public enum TransactionType: byte { Deposit = 0, Payment = 1 }

    public class TransactionModel
    {
        public long Id { get; set; }

        public TransactionType Type { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public decimal Balance { get; set; }

        public string AccountNumber { get; set; }

        public virtual AccountModel Account { get; set; }
    }
}