using NationalBankEF.Enums;

namespace NationalBankEF.Models;

public class CustomerAccount
{
    public string AccountNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime OwnershipStartDate { get; set; }
    public OwnershipRole OwnershipRole { get; set; }
    public AccountStatus AccountStatus { get; set; }

    public Account Account { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
