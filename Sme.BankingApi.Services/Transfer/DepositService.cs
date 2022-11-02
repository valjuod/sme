using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Models;

namespace Sme.BankingApi.Services.Transfer
{
    public interface IDepositService
    {
        void DoTransfer(TransferRequest operation);
    }

    public sealed class DepositService : TransferService, IDepositService
    {

        public DepositService(IAccountRepository accountRepository,
            ITransactionRepository transactionRepository): base(accountRepository, transactionRepository)
        {
        }

        public void DoTransfer(TransferRequest operation)
        {
            base.DoTransfer(TransactionType.Deposit, operation);
        }

    }
}