using MediatR;
using Microsoft.EntityFrameworkCore;
using Sme.BankingApi.Commands.Customer;
using Sme.BankingApi.Common;
using Sme.BankingApi.Data;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Transfer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AccountContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountConnection"));
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

// mediatr
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(UpgradeCustomerToVipCommand)));

// repositories 
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// settings
builder.Services.AddSingleton<CustomerSettings>(builder.Configuration.GetSection("CustomerSettings").Get<CustomerSettings>());

// go
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    // swagger only for dev mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();