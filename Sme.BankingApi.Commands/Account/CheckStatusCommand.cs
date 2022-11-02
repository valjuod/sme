using MediatR;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sme.BankingApi.Commands.Account
{
    public class CheckStatusCommand : IRequest<CommandResult<AccountStatus>>
    {
        public string AccountNumber { get; }

        public CheckStatusCommand(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }

    public class CheckStatusCommandHandler : IRequestHandler<CheckStatusCommand, CommandResult<AccountStatus>>
    {
        private readonly IAccountRepository _accountRepository;

        public CheckStatusCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<CommandResult<AccountStatus>> Handle(CheckStatusCommand message, CancellationToken cancellationToken)
        {
            var account = _accountRepository.Find(message.AccountNumber);

            if (account is null)
            {
                return CommandResult.Failed<AccountStatus>("ACCOUNT_NOT_FOUND", "Account not found");
            }

            return CommandResult.Ok(account.Status);
        }
    }
}
