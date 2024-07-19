using HRMS_Web.DataAccess.Data;
using HRMS_Web.IService;
using HRMS_Web.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HRMS_Web.Service
{
    public class NotiService : INotiService
    {
        //private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private List<UsersNotification> _oNotifications = new List<UsersNotification>();

        public NotiService(ApplicationDbContext context)
        {
            //_connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public List<UsersNotification> GetNotifications(string nToUserId, bool bIsGetOnlyUnread)
        {
            var query = _context.UsersNotifications
                .Include(l => l.Notification)
                .Where(l => l.RecieverId == nToUserId);

            if (bIsGetOnlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            return query.ToList();
        }
        //public List<UsersNotification> GetNotifications(string nToUserId, bool bIsGetOnlyUnread)
        //{
        //    _oNotifications = new List<UsersNotification>();
        //    using (IDbConnection con = new SqlConnection(_connectionString))
        //    {
        //        if (con.State == ConnectionState.Closed) con.Open();

        //        var oNotis = con.Query<UsersNotification>("SELECT * FROM UsersNotification WHERE RecieverId=" + nToUserId).ToList();

        //    if (oNotis != null && oNotis.Count() > 0)
        //        {
        //            _oNotifications = oNotis;
        //        }
        //    return _oNotifications;
        //    }
        //}
    }
}
