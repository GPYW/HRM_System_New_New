using HRMS_Web.Models;

namespace HRMS_Web.IService
{
    public interface INotiService
    {
        List<UsersNotification> GetNotifications(string nToUserId, bool bIsGetOnlyUnread);
    }
}
