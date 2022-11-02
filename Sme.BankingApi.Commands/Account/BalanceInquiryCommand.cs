using MediatR;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Commands.Model;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sme.BankingApi.Commands.Account
{
    public class BalanceInquiryCommand : IRequest<CommandResult<BalanceInquiryResult>>
    {
        public string AccountNumber { get; }

        public BalanceInquiryCommand(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }

    public class BalanceInquiryCommandHandler : IRequestHandler<BalanceInquiryCommand, CommandResult<BalanceInquiryResult>>
    {
        private readonly IAccountRepository _accountRepository;

        public BalanceInquiryCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<CommandResult<BalanceInquiryResult>> Handle(BalanceInquiryCommand message, CancellationToken cancellationToken)
        {
            var account = _accountRepository.Find(message.AccountNumber);

            if (account is null)
            {
                return CommandResult.Failed<BalanceInquiryResult>("ACCOUNT_NOT_FOUND", "Account not found");
            }

            var result = new BalanceInquiryResult()
            {
                AccountNumber = account.Number,
                Currency = account.Currency,
                Status = account.Status,
                Balance = account.Balance
            };

            return CommandResult.Ok(result);
        }

    }
}
