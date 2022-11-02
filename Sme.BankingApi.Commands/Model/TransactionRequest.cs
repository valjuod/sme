using Sme.BankingApi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sme.BankingApi.Commands.Model
{
    public class TransactionRequest
    {
        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }
    }
}
