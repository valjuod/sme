namespace Sme.BankingApi.Data.Repository
{

    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        protected AccountContext _context;

        public BaseRepository(AccountContext context)
        {
            _context = context;
        }

        public T Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return entity;
        }

        public T Remove(T entity)
        {

            _context.Remove(entity);
            _context.SaveChanges();

            return entity;
        }

        public IQueryable<T> GetAll()
        {

            return _context.Set<T>().AsQueryable();
        }

        public T Find(object id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Attach(T entity)
        {
            _context.Attach(entity);
        }
    }

    public interface IBaseRepository<T> where T : class, new()
    {
        T Find(object id);

        IQueryable<T> GetAll();

        T Insert(T entity);

        T Remove(T entity);

        T Update(T entity);

        void Attach(T entity);
    }
}
