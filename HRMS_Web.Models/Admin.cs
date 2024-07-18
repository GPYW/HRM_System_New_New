using System.ComponentModel.DataAnnotations;

namespace HRMS_Web.Models;

public class Admin
{
    [Key]
    [Required]
    public int SmtpId { get; set; }
    public string AdminId { get; set; }
    public string Email { get; set; }
    public string SmtpServer { get; set; }
    public string SmtpPassword { get; set; }
    public int SmtpPort { get; set; }
    public ApplicationUser? User { get; set; }
}