using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sme.BankingApi.Data.Model;

namespace Sme.BankingApi.Data.Configuration
{

    public class AccountConfiguration : IEntityTypeConfiguration<AccountModel>
    {
        public void Configure(EntityTypeBuilder<AccountModel> builder)
        {
            builder.HasKey(x => x.Number);
            builder.Property(x => x.Balance).HasPrecision(20, 2);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.Property(x => x.Number).HasMaxLength(34);

            builder.ToTable("Accounts");

            builder.HasIndex(x => x.CustomerId);
            
        }
    }
}
