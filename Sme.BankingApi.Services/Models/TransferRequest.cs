using Sme.BankingApi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sme.BankingApi.Services.Models
{
    public class TransferRequest
    {
        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public TransferRequest(string accountNumber, decimal amount, Currency currency, DateTime date)
        {
            AccountNumber = accountNumber;
            Amount = amount;
            Currency = currency;
            Date = date;
        }
    }
}
