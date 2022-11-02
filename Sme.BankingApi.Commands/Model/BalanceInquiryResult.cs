using Sme.BankingApi.Data.Model;

namespace Sme.BankingApi.Commands.Model
{
    public class BalanceInquiryResult
    {
        public string AccountNumber { get; set; }

        public Currency Currency { get; set; }

        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; }
    }
}