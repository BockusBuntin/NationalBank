namespace NationalBankEF.Models;

public class Manager
{
    public int ManagerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }

    public Branch? Branch { get; set; }
}
