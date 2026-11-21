using Microsoft.EntityFrameworkCore;
using NationalBankEF.Data;
using NationalBankEF.Enums;
using NationalBankEF.Models;

namespace NationalBankEF.Services;

public class BankService
{
    private readonly BankDbContext _db;

    public BankService(BankDbContext db) => _db = db;

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FullName)) throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(customer.NationalId)) throw new ArgumentException("National ID is required.");
        if (await _db.Customers.AnyAsync(x => x.NationalId == customer.NationalId))
            throw new InvalidOperationException("A customer with this National ID already exists.");

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<string> OpenOrLinkAccountAsync(string accountNumber, AccountType accountType, string branchCode, int customerId, OwnershipRole role)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.");

        var branch = await _db.Branches.FindAsync(branchCode);
        if (branch is null) throw new InvalidOperationException($"Branch '{branchCode}' does not exist.");

        var customer = await _db.Customers.FindAsync(customerId);
        if (customer is null) throw new InvalidOperationException($"Customer #{customerId} does not exist.");

        var account = await _db.Accounts
            .Include(x => x.CustomerAccounts)
            .SingleOrDefaultAsync(x => x.AccountNumber == accountNumber);
        var created = account is null;

        if (account is null)
        {
            account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                OpeningDate = DateTime.Today,
                Balance = 0m,
                BranchCode = branchCode
            };
            _db.Accounts.Add(account);
        }
        else if (account.BranchCode != branchCode)
        {
            throw new InvalidOperationException("The existing account belongs to a different branch.");
        }

        if (await _db.CustomerAccounts.AnyAsync(x => x.AccountNumber == accountNumber && x.CustomerId == customerId))
            throw new InvalidOperationException("This customer is already linked to the account.");

        if (role == OwnershipRole.Primary && await _db.CustomerAccounts.AnyAsync(x => x.AccountNumber == accountNumber && x.OwnershipRole == OwnershipRole.Primary))
            throw new InvalidOperationException("This account already has a Primary owner.");

        _db.CustomerAccounts.Add(new CustomerAccount
        {
            AccountNumber = accountNumber,
            CustomerId = customerId,
            OwnershipStartDate = DateTime.Today,
            OwnershipRole = role,
            AccountStatus = AccountStatus.Active
        });

        await _db.SaveChangesAsync();
        return created ? $"Account '{accountNumber}' created and linked successfully." : $"Account '{accountNumber}' linked successfully to customer #{customerId}.";
    }

    public async Task UpdateAccountStatusAsync(string accountNumber, int customerId, AccountStatus newStatus)
    {
        var link = await _db.CustomerAccounts
            .SingleOrDefaultAsync(x => x.AccountNumber == accountNumber && x.CustomerId == customerId);

        if (link is null) throw new InvalidOperationException("The account/customer ownership link was not found.");
        link.AccountStatus = newStatus;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAccountFromCustomerAsync(string accountNumber, int customerId)
    {
        var link = await _db.CustomerAccounts
            .SingleOrDefaultAsync(x => x.AccountNumber == accountNumber && x.CustomerId == customerId);

        if (link is null) throw new InvalidOperationException("The account/customer ownership link was not found.");

        _db.CustomerAccounts.Remove(link);
        await _db.SaveChangesAsync();

        var hasOwners = await _db.CustomerAccounts.AnyAsync(x => x.AccountNumber == accountNumber);
        if (!hasOwners)
        {
            var account = await _db.Accounts.FindAsync(accountNumber);
            if (account is not null)
            {
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();
            }
            return true;
        }

        return false;
    }

    public async Task<List<Customer>> GetCustomersWithAccountsAsync()
    {
        return await _db.Customers
            .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
                    .ThenInclude(a => a.Branch)
            .AsNoTracking()
            .OrderBy(c => c.CustomerId)
            .ToListAsync();
    }

    public async Task AddTransactionAsync(string accountNumber, decimal amount, TransactionType type, string description)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

        await using var transaction = await _db.Database.BeginTransactionAsync();
        var account = await _db.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == accountNumber);
        if (account is null) throw new InvalidOperationException("Account not found.");

        if (type is TransactionType.Withdrawal or TransactionType.Payment && account.Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");

        account.Balance += type == TransactionType.Deposit ? amount : -amount;
        _db.Transactions.Add(new BankTransaction
        {
            AccountNumber = accountNumber,
            Amount = amount,
            TransactionDate = DateTime.Now,
            TransactionType = type,
            Description = description
        });

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
