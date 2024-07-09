using HRMS_Web.DataAccess.Data;
using HRMS_Web.iService;
using HRMS_Web.Models;

namespace HRMS_Web.Service
{
    public class UserService : iUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetSavedUser() // Correct method name
        {
            return _context.ApplicationUser.SingleOrDefault();
        }

        public ApplicationUser Save(ApplicationUser user)
        {
            _context.ApplicationUser.Add(user);
            _context.SaveChanges();
            return user;
        }
    }
}
