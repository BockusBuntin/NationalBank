using NationalBankEF.Enums;

namespace NationalBankEF.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string HomeAddress { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; }

    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
}
