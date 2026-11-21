using Microsoft.EntityFrameworkCore;
using NationalBankEF.Enums;
using NationalBankEF.Models;

namespace NationalBankEF.Data;

public class BankDbContext : DbContext
{
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<BankTransaction> Transactions => Set<BankTransaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(x => x.ManagerId);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Phone).IsRequired().HasMaxLength(30);
            entity.Property(x => x.HireDate).HasColumnType("date");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(x => x.BranchCode);
            entity.Property(x => x.BranchCode).HasMaxLength(20);
            entity.Property(x => x.BranchName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Address).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Phone).IsRequired().HasMaxLength(30);

            entity.HasOne(x => x.Manager)
                .WithOne(x => x.Branch)
                .HasForeignKey<Branch>(x => x.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.CustomerId);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.NationalId).IsRequired().HasMaxLength(30);
            entity.HasIndex(x => x.NationalId).IsUnique();
            entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Phone).IsRequired().HasMaxLength(30);
            entity.Property(x => x.HomeAddress).IsRequired().HasMaxLength(250);
            entity.Property(x => x.CustomerType).HasConversion<int>();
            entity.Property(x => x.DateOfBirth).HasColumnType("date");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(x => x.AccountNumber);
            entity.Property(x => x.AccountNumber).HasMaxLength(30);
            entity.Property(x => x.AccountType).HasConversion<int>();
            entity.Property(x => x.OpeningDate).HasColumnType("date");
            entity.Property(x => x.Balance).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.Branch)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.BranchCode)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CustomerAccount>(entity =>
        {
            entity.HasKey(x => new { x.AccountNumber, x.CustomerId });
            entity.Property(x => x.OwnershipRole).HasConversion<int>();
            entity.Property(x => x.AccountStatus).HasConversion<int>();
            entity.Property(x => x.OwnershipStartDate).HasColumnType("date");

            entity.HasOne(x => x.Account)
                .WithMany(x => x.CustomerAccounts)
                .HasForeignKey(x => x.AccountNumber)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerAccounts)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasKey(x => x.TransactionId);
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Description).HasMaxLength(300);
            entity.Property(x => x.TransactionType).HasConversion<int>();
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Required seed: at least Branch and Manager tables.
        modelBuilder.Entity<Manager>().HasData(
            new Manager { ManagerId = 1, FullName = "Ahmed Ali", Email = "ahmed.ali@nationalbank.com", Phone = "01000000001", HireDate = new DateTime(2020, 1, 15) },
            new Manager { ManagerId = 2, FullName = "Mona Hassan", Email = "mona.hassan@nationalbank.com", Phone = "01000000002", HireDate = new DateTime(2021, 3, 10) }
        );

        modelBuilder.Entity<Branch>().HasData(
            new Branch { BranchCode = "CAI-01", BranchName = "Cairo Main Branch", Address = "123 Tahrir Street, Cairo", Phone = "0220000001", ManagerId = 1 },
            new Branch { BranchCode = "ALX-01", BranchName = "Alexandria Branch", Address = "45 Saad Zaghloul Street, Alexandria", Phone = "0330000002", ManagerId = 2 }
        );
    }
}
