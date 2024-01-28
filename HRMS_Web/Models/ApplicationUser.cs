using Microsoft.AspNetCore.Identity;

namespace HRMS_Web.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
