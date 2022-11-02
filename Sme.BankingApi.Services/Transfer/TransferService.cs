using Sme.BankingApi.Common;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Exceptions;
using Sme.BankingApi.Services.Models;
using System.Runtime;
using System.Security.Principal;
using System.Transactions;

namespace Sme.BankingApi.Services.Transfer
{
    public abstract class TransferService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITransactionRepository _transactionRepository;

        private readonly ICustomerRepository _customerRepository;

        private readonly CustomerSettings _settings;

        private const long MIN_ACCOUNT_BALANCE = 0;

        private const long MAX_ACCOUNT_BALANCE = 10000000000000;

        private const decimal MIN_TRANSACTION_AMOUNT = 0.01m;

        public TransferService(IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public void DoTransfer(TransactionType type, TransferRequest operation)
        {
            // TODO: should be a proper lock mechanism for transactions which is out of scope of this assignment

            var account = _accountRepository.Find(operation.AccountNumber);

            if (account is null)
            {
                throw new TransferException("ACCOUNT_NOT_FOUND", "Account not found");
            }

            if (account.Status != AccountStatus.Opened)
            {
                throw new TransferException("ACCOUNT_NOT_OPENED", "Account must be opened");
            }

            if (account.Currency != operation.Currency)
            {
                throw new TransferException("ACCOUNT_CURRENCY_MISMATCH", "Account has a different currency");
            }

            if (operation.Amount < 0.01m)
            {
                throw new TransferException("TRANSACTION_MIN_AMOUNT", "Amount value must be more at least 0.01");
            }

            if (type == TransactionType.Deposit)
            {
                if (account.Balance + operation.Amount > MAX_ACCOUNT_BALANCE)
                {
                    throw new TransferException("ACCOUNT_BALANCE_REACHED", "Maximum balance reached");
                }
            }
            else if (type == TransactionType.Payment)
            {
                if (account.Balance - operation.Amount < MIN_ACCOUNT_BALANCE)
                {
                    throw new TransferException("ACCOUNT_BALANCE_TOO_LOW", "Account balance is too low");
                }
            }

            TransactionModel transaction;

            using (var scope = new TransactionScope())
            {
                transaction = ExecuteTransfer(type, account, operation.Amount, operation.Currency, operation.Date);

                // some events go here
                PostTransfer(account, transaction);

                scope.Complete();
            }
        }

        protected TransactionModel ExecuteTransfer(TransactionType type,
            AccountModel account,
            decimal amount,
            Currency currency,
            DateTime date)
        {
            var actualAmount = amount * (type == TransactionType.Deposit ? 1.0m : -1.0m);

            account.Balance += actualAmount;

            var transaction = new TransactionModel()
            {
                Account = account,
                Amount = amount,
                Balance = account.Balance,
                Timestamp = date,
                Currency = currency,
                Type = type
            };

            _transactionRepository.Insert(transaction);
            _accountRepository.Update(account);

            return transaction;
        }

        /// <summary>
        /// Some custom logic goes here
        /// </summary>
        /// <param name="account"></param>
        /// <param name="transaction"></param>
        protected virtual void PostTransfer(AccountModel account, TransactionModel transaction)
        {

        }
    }
}