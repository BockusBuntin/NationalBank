# National Bank - EF Core Code First Assignment

This project implements the requested EF Core Code-First data layer and interactive console menu from the assignment.

## Requirements
- .NET 8 SDK
- SQL Server LocalDB (or change the connection string in `appsettings.json` and `Data/BankDbContext.cs`)
- EF Core CLI: `dotnet tool install --global dotnet-ef --version 8.0.19`

## Run

```bash
dotnet restore
dotnet ef database update
dotnet run
```

The database is created from the EF Core migration. Seed data inserts two managers and two branches.

## Menu

1. Add a new Customer
2. Open a new Account for a Customer
3. Update Account Status (Active / Closed)
4. Remove an Account from a Customer
5. List all Customers (with accounts)
0. Exit

## Notes
- `CustomerAccount` is the join entity for joint accounts and stores ownership role, ownership start date, and account status.
- Option 2 can be used again with the same account number to attach another customer as a co-holder.
- If the last ownership link is removed, the account itself is removed, matching the provided expected output.
- `BankService.AddTransactionAsync` implements the case-study balance rule for deposits, withdrawals, and payments. The assignment's required menu does not expose a transaction option.
