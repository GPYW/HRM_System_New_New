using Microsoft.AspNetCore.Identity;

namespace HRMS_Web.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // New fields
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? join_date { get; set; }
    }
}
