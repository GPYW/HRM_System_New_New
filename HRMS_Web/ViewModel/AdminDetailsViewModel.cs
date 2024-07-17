using System.ComponentModel.DataAnnotations;

public class AdminDetailsViewModel
{
    public string AdminId { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "SMTP Username (Email)")]
    public string SmtpUsername { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "SMTP Password")]
    public string SmtpPassword { get; set; }

    [Required]
    [Range(1, 65535, ErrorMessage = "Please enter a valid port number.")]
    [Display(Name = "SMTP Port")]
    public int SmtpPort { get; set; }
}
