using MediatR;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Data.Repository;

namespace Sme.BankingApi.Commands.Customer
{
    public class UpgradeCustomerToVipCommand : IRequest<CommandResult>
    {
        public int CustomerId { get; }

        public UpgradeCustomerToVipCommand(int customerId)
        {
            CustomerId = customerId;
        }
    }

    public class UpgradeCustomerToVipCommandHandler : IRequestHandler<UpgradeCustomerToVipCommand, CommandResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpgradeCustomerToVipCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CommandResult> Handle(UpgradeCustomerToVipCommand message, CancellationToken cancellationToken)
        {
            var customer = _customerRepository.Find(message.CustomerId);

            if (customer is null)
            {
                return CommandResult.Failed<CommandResult>("CUSTOMER_NOT_FOUND", "Customer not found");
            }

            if (customer.IsVip)
            {
                return CommandResult.Failed<CommandResult>("INVALID_OPERATION", "Customer alread has VIP status");
            }

            customer.IsVip = true;

            try
            {
                _customerRepository.Update(customer);

                return CommandResult.Ok(true);
            }
            catch (Exception e)
            {
                return CommandResult.Failed<CommandResult>("FATAL_ERROR", e.Message);
            }
        }
    }
}
