using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalBankEF.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Managers",
            columns: table => new
            {
                ManagerId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                HireDate = table.Column<DateTime>(type: "date", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_Managers", x => x.ManagerId));

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                CustomerId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                NationalId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                HomeAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                CustomerType = table.Column<int>(type: "int", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_Customers", x => x.CustomerId));

        migrationBuilder.CreateTable(
            name: "Branches",
            columns: table => new
            {
                BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                BranchName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                ManagerId = table.Column<int>(type: "int", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Branches", x => x.BranchCode);
                table.ForeignKey("FK_Branches_Managers_ManagerId", x => x.ManagerId, "Managers", "ManagerId", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                AccountType = table.Column<int>(type: "int", nullable: false),
                OpeningDate = table.Column<DateTime>(type: "date", nullable: false),
                Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                BranchCode = table.Column<string>(type: "nvarchar(20)", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.AccountNumber);
                table.ForeignKey("FK_Accounts_Branches_BranchCode", x => x.BranchCode, "Branches", "BranchCode", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CustomerAccounts",
            columns: table => new
            {
                AccountNumber = table.Column<string>(type: "nvarchar(30)", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                OwnershipStartDate = table.Column<DateTime>(type: "date", nullable: false),
                OwnershipRole = table.Column<int>(type: "int", nullable: false),
                AccountStatus = table.Column<int>(type: "int", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_CustomerAccounts", x => new { x.AccountNumber, x.CustomerId });
                table.ForeignKey("FK_CustomerAccounts_Accounts_AccountNumber", x => x.AccountNumber, "Accounts", "AccountNumber", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_CustomerAccounts_Customers_CustomerId", x => x.CustomerId, "Customers", "CustomerId", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Transactions",
            columns: table => new
            {
                TransactionId = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TransactionType = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                AccountNumber = table.Column<string>(type: "nvarchar(30)", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                table.ForeignKey("FK_Transactions_Accounts_AccountNumber", x => x.AccountNumber, "Accounts", "AccountNumber", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Branches_ManagerId", table: "Branches", column: "ManagerId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Customers_NationalId", table: "Customers", column: "NationalId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Accounts_BranchCode", table: "Accounts", column: "BranchCode");
        migrationBuilder.CreateIndex(name: "IX_CustomerAccounts_CustomerId", table: "CustomerAccounts", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_Transactions_AccountNumber", table: "Transactions", column: "AccountNumber");

        migrationBuilder.InsertData("Managers", new[] { "ManagerId", "FullName", "Email", "Phone", "HireDate" },
            new object[] { 1, "Ahmed Ali", "ahmed.ali@nationalbank.com", "01000000001", new DateTime(2020, 1, 15) });
        migrationBuilder.InsertData("Managers", new[] { "ManagerId", "FullName", "Email", "Phone", "HireDate" },
            new object[] { 2, "Mona Hassan", "mona.hassan@nationalbank.com", "01000000002", new DateTime(2021, 3, 10) });
        migrationBuilder.InsertData("Branches", new[] { "BranchCode", "BranchName", "Address", "Phone", "ManagerId" },
            new object[] { "CAI-01", "Cairo Main Branch", "123 Tahrir Street, Cairo", "0220000001", 1 });
        migrationBuilder.InsertData("Branches", new[] { "BranchCode", "BranchName", "Address", "Phone", "ManagerId" },
            new object[] { "ALX-01", "Alexandria Branch", "45 Saad Zaghloul Street, Alexandria", "0330000002", 2 });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CustomerAccounts");
        migrationBuilder.DropTable(name: "Transactions");
        migrationBuilder.DropTable(name: "Customers");
        migrationBuilder.DropTable(name: "Accounts");
        migrationBuilder.DropTable(name: "Branches");
        migrationBuilder.DropTable(name: "Managers");
    }
}
