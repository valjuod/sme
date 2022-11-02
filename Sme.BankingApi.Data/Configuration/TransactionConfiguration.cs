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

    public class TransactionConfiguration : IEntityTypeConfiguration<TransactionModel>
    {
        public void Configure(EntityTypeBuilder<TransactionModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AccountNumber).HasMaxLength(34);
            builder.Property(x => x.Balance).HasPrecision(20, 2);
            builder.Property(x => x.Amount).HasPrecision(14, 2);
            builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountNumber);

            builder.ToTable("Transactions");
        }
    }
}