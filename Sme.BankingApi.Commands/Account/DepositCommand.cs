using MediatR;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Commands.Model;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Services.Exceptions;
using Sme.BankingApi.Services.Models;
using Sme.BankingApi.Services.Transfer;

namespace Sme.BankingApi.Commands.Account
{
    public class DepositCommand : IRequest<CommandResult>
    {
        public TransactionRequest Request { get; }

        public DepositCommand(TransactionRequest request)
        {
            Request = request;
        }
    }

    public class DepositCommandHandler : IRequestHandler<DepositCommand, CommandResult>
    {
        private readonly IDepositService _transferService;

        public DepositCommandHandler(IDepositService transferService)
        {
            _transferService = transferService;
        }

        public async Task<CommandResult> Handle(DepositCommand message, CancellationToken cancellationToken)
        {
            if (message.Request is null)
            {
                return CommandResult.Failed<CommandResult>("EMPTY_REQUEST", "Invalid data");
            }

            try
            {
                _transferService.DoTransfer(new TransferRequest(
                    message.Request.AccountNumber,
                    message.Request.Amount,
                    message.Request.Currency,
                    message.Request.Date)
                    );

                return CommandResult.Ok(true);
            } 
            catch (TransferException e)
            {
                return CommandResult.Failed<CommandResult>(e.Code, e.Message);
            }
            catch (Exception e)
            {
                return CommandResult.Failed<CommandResult>("FATAL_ERROR", e.Message);
            }
        }
    }
}
