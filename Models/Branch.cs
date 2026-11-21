namespace NationalBankEF.Models;

public class Branch
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ManagerId { get; set; }

    public Manager Manager { get; set; } = null!;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
