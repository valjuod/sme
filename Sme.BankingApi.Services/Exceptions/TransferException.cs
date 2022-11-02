using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sme.BankingApi.Services.Exceptions
{
    public class TransferException: Exception
    {
        public string Code { get; }

        public TransferException(string code, string message): base(message)
        {
            Code = code;
        }
    }
}
