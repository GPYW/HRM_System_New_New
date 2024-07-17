namespace HRMS_Web.Models;

public class Admin
{
    public int AdminId { get; set; }
    public string Email { get; set; }
    public string SmtpPassword { get; set; }
    public int SmtpPort { get; set; }
}
