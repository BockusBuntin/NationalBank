using NationalBankEF.Data;
using NationalBankEF.Services;
using NationalBankEF.UI;

using var db = new BankDbContext();
var app = new BankMenu(db);
await app.RunAsync();
