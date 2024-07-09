using HRMS_Web.Models;

namespace HRMS_Web.iService
{
    public interface iUserService
    {
        ApplicationUser Save(ApplicationUser user);
        ApplicationUser GetSavedUser();
    }
}
