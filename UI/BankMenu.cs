using System.Globalization;
using NationalBankEF.Data;
using NationalBankEF.Enums;
using NationalBankEF.Models;
using NationalBankEF.Services;

namespace NationalBankEF.UI;

public class BankMenu
{
    private readonly BankService _service;

    public BankMenu(BankDbContext db) => _service = new BankService(db);

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            PrintMenu();
            var choice = ReadInt("Enter choice: ");

            try
            {
                switch (choice)
                {
                    case 1: await AddCustomerAsync(); break;
                    case 2: await OpenAccountAsync(); break;
                    case 3: await UpdateStatusAsync(); break;
                    case 4: await RemoveAccountAsync(); break;
                    case 5: await ListCustomersAsync(); break;
                    case 0: return;
                    default: Error("Invalid choice. Please select 0-5."); break;
                }
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                Error(ex.Message);
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("       National Bank - Management");
        Console.WriteLine("=============================================");
        Console.WriteLine("  1) Add a new Customer");
        Console.WriteLine("  2) Open a new Account for a Customer");
        Console.WriteLine("  3) Update Account Status (Active / Closed)");
        Console.WriteLine("  4) Remove an Account from a Customer");
        Console.WriteLine("  5) List all Customers (with accounts)");
        Console.WriteLine("  0) Exit");
        Console.WriteLine("---------------------------------------------");
    }

    private async Task AddCustomerAsync()
    {
        Console.WriteLine("\n--- Add New Customer ---");
        var customer = new Customer
        {
            FullName = ReadRequired("Full Name").Trim(),
            NationalId = ReadRequired("National ID").Trim(),
            DateOfBirth = ReadDate("Date of Birth (yyyy-MM-dd)"),
            Email = ReadRequired("Email").Trim(),
            Phone = ReadRequired("Phone").Trim(),
            HomeAddress = ReadRequired("Address").Trim(),
            CustomerType = ReadCustomerType()
        };

        var created = await _service.AddCustomerAsync(customer);
        Success($"Customer created successfully. CustomerId = {created.CustomerId}");
    }

    private async Task OpenAccountAsync()
    {
        Console.WriteLine("\n--- Open New Account ---");
        var accountNumber = ReadRequired("Account Number").Trim();
        var accountType = ReadAccountType();
        var branchCode = ReadRequired("Branch Code").Trim();
        var customerId = ReadInt("Customer Id: ");
        var role = ReadOwnershipRole();

        Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");
        var message = await _service.OpenOrLinkAccountAsync(accountNumber, accountType, branchCode, customerId, role);
        Success(message + $" Linked to customer #{customerId} as {role} owner.");
    }

    private async Task UpdateStatusAsync()
    {
        Console.WriteLine("\n--- Update Account Status ---");
        var accountNumber = ReadRequired("Account Number").Trim();
        var customerId = ReadInt("Customer Id: ");
        var status = ReadAccountStatus();

        await _service.UpdateAccountStatusAsync(accountNumber, customerId, status);
        Success($"Status updated to {status}.");
    }

    private async Task RemoveAccountAsync()
    {
        Console.WriteLine("\n--- Remove Account From Customer ---");
        var accountNumber = ReadRequired("Account Number").Trim();
        var customerId = ReadInt("Customer Id: ");

        var accountRemoved = await _service.RemoveAccountFromCustomerAsync(accountNumber, customerId);
        Success("Ownership link deleted.");
        if (accountRemoved)
            Console.WriteLine($"That was the last owner - account '{accountNumber}' was also removed.");
    }

    private async Task ListCustomersAsync()
    {
        Console.WriteLine("\n--- All Customers ---");
        var customers = await _service.GetCustomersWithAccountsAsync();

        if (customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
            return;
        }

        foreach (var customer in customers)
        {
            Console.WriteLine($"#{customer.CustomerId} {customer.FullName} ({customer.CustomerType})");

            if (customer.CustomerAccounts.Count == 0)
            {
                Console.WriteLine("    (no accounts)");
                continue;
            }

            foreach (var link in customer.CustomerAccounts)
            {
                var account = link.Account;
                Console.WriteLine($"    {account.AccountNumber,-8} {account.AccountType,-8} Balance: {account.Balance,12:N2}  {link.OwnershipRole,-9} {link.AccountStatus,-6}  @ {account.Branch.BranchName}");
            }
        }
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label,-16}: ");
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value)) return value;
            Error($"{label} is required.");
        }
    }

    private static int ReadInt(string label)
    {
        while (true)
        {
            Console.Write(label);
            if (int.TryParse(Console.ReadLine(), out var value)) return value;
            Error("Please enter a valid number.");
        }
    }

    private static DateTime ReadDate(string label)
    {
        while (true)
        {
            Console.Write($"{label,-16}: ");
            if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            Error("Invalid date. Use yyyy-MM-dd.");
        }
    }

    private static CustomerType ReadCustomerType()
    {
        Console.WriteLine("Customer Type:\n    1) Individual\n    2) Business");
        while (true)
        {
            var choice = ReadInt("  Choice: ");
            if (choice is 1 or 2) return (CustomerType)choice;
            Error("Choose 1 or 2.");
        }
    }

    private static AccountType ReadAccountType()
    {
        Console.WriteLine("Account Type:\n    1) Savings\n    2) Current\n    3) Business");
        while (true)
        {
            var choice = ReadInt("  Choice: ");
            if (choice is >= 1 and <= 3) return (AccountType)choice;
            Error("Choose 1, 2, or 3.");
        }
    }

    private static OwnershipRole ReadOwnershipRole()
    {
        Console.WriteLine("Ownership Role:\n    1) Primary\n    2) CoHolder");
        while (true)
        {
            var choice = ReadInt("  Choice: ");
            if (choice is 1 or 2) return (OwnershipRole)choice;
            Error("Choose 1 or 2.");
        }
    }

    private static AccountStatus ReadAccountStatus()
    {
        Console.WriteLine("New Status:\n    1) Active\n    2) Closed");
        while (true)
        {
            var choice = ReadInt("  Choice: ");
            if (choice is 1 or 2) return (AccountStatus)choice;
            Error("Choose 1 or 2.");
        }
    }

    private static void Success(string message) => Console.WriteLine($"\n{message}");
    private static void Error(string message) => Console.WriteLine($"\nError: {message}");
}
