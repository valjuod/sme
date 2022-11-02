using Sme.BankingApi.Common;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Models;

namespace Sme.BankingApi.Services.Transfer
{
    public interface IPaymentService
    {
        void DoTransfer(TransferRequest operation);
    }

    public sealed class PaymentService : TransferService, IPaymentService
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly CustomerSettings _settings;

        public PaymentService(IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ICustomerRepository customerRepository,
            CustomerSettings settings) : base(accountRepository, transactionRepository)
        {
            _customerRepository = customerRepository;
            _settings = settings;
        }

        public void DoTransfer(TransferRequest operation)
        {
            base.DoTransfer(TransactionType.Payment, operation);
        }

        protected override void PostTransfer(AccountModel account, TransactionModel transaction)
        {
            var customer = _customerRepository.Find(account.CustomerId);

            if (!customer.IsVip)
            {
                return;
            }

            if (_settings.VipRewardPercentage <= 0)
            {
                return;
            }

            GiveRewardIfNeeded(account, transaction);
        }

        private void GiveRewardIfNeeded(AccountModel account, TransactionModel paymentTransaction)
        {
            var rewardAmount = Math.Round(paymentTransaction.Amount * _settings.VipRewardPercentage * 0.01m, 2);
            if (rewardAmount > 0)
            {
                ExecuteTransfer(TransactionType.Deposit,
                     account,
                     rewardAmount,
                     paymentTransaction.Currency,
                     DateTime.Now
                     );
            }
        }
    }
}