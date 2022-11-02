using Sme.BankingApi.Data.Model;

namespace Sme.BankingApi.Data.Repository
{
    public interface ITransactionRepository : IBaseRepository<TransactionModel>
    {
    }

    public class TransactionRepository : BaseRepository<TransactionModel>, ITransactionRepository
    {
        public TransactionRepository(AccountContext context) : base(context)
        {
        }

    }
}
