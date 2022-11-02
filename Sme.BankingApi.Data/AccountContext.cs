using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sme.BankingApi.Data.Model;
using System.Reflection;

namespace Sme.BankingApi.Data
{
    public class AccountContext: DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }

        public DbSet<CustomerModel> Customers { get; set; }

        public DbSet<TransactionModel> Transactions { get; set; }

        public AccountContext() : this(new DbContextOptionsBuilder<AccountContext>().Options)
        {

        }

        public AccountContext(DbContextOptions<AccountContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("AccountConnection");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var customers = new List<CustomerModel>() { 
                new CustomerModel()
                {
                    Id = 1,
                    Title = "Mr simple",
                    IsVip = false
                },
                new CustomerModel()
                {
                    Id = 2,
                    Title = "Mr VIP",
                    IsVip = true
                }
            };


            var accounts = new List<AccountModel>();

            int id = 1;
            foreach (var customer in customers)
            {
                foreach (var currency in (Currency[])Enum.GetValues(typeof(Currency)))
                {
                    accounts.Add(new AccountModel()
                    {
                        Currency = currency,
                        CustomerId = customer.Id,
                        Number = "LT11" + String.Format("{0:D30}", id)
                    });

                    id++;
                }
            }

            modelBuilder.Entity<CustomerModel>().HasData(customers);
            modelBuilder.Entity<AccountModel>().HasData(accounts);
        }
    }
}
