using HRMS_Web.Models;

namespace HRMS_Web.IService
{
    public interface IUserService
    {
        ApplicationUser Save(ApplicationUser user);
        ApplicationUser GetSavedUser();
    }
}
