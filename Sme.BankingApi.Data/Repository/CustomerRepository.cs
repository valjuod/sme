using Sme.BankingApi.Data.Model;

namespace Sme.BankingApi.Data.Repository
{
    public interface ICustomerRepository: IBaseRepository<CustomerModel>
    {
    }

    public class CustomerRepository: BaseRepository<CustomerModel>, ICustomerRepository
    {
        public CustomerRepository(AccountContext context) : base(context)
        {
        }

    }
}
