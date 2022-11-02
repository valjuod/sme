using Sme.BankingApi.Data.Model;

namespace Sme.BankingApi.Data.Repository
{
    public interface IAccountRepository : IBaseRepository<AccountModel>
    {
    }

    public class AccountRepository : BaseRepository<AccountModel>, IAccountRepository
    {
        public AccountRepository(AccountContext context) : base(context)
        {
        }

    }
}
