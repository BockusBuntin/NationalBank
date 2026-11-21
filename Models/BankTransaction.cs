using NationalBankEF.Enums;

namespace NationalBankEF.Models;

public class BankTransaction
{
    public long TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;

    public Account Account { get; set; } = null!;
}
