# Intro

- .NET 6 based
- MS Sql Server based
- Libraries: EF, Mediatr, NUnit
- Security layer is out of scope 

# Configuration
## Sme.BankingApi/appsettings.json
- ConnectionStrings.AccountConnection db connection string
- CustomerSettings.VipRewardPercentage set reward percentage

# Setup

- Script migrations and apply using any sql server management tool: 
   Script-Migration -StartupProject Sme.BankingApi -Project Sme.BankingApi.Data 
- Or run Update-Database with proper connection string: 
  Update-Database -StartupProject Sme.BankingApi -Project Sme.BankingApi.Data -Connection "xxxxxx"

# Launch
- Run Sme.BankingApi project in Visual Studio 2022