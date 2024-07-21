using HRMS_Web.DataAccess.Data;
using HRMS_Web.Hubs;
using HRMS_Web.IService;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS_Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotiService _notiService = null;
        List<UsersNotification> _oNotifications = new List<UsersNotification>();

        public NotificationController(ApplicationDbContext dbContext, IHubContext<NotificationHub> hubContext, INotiService notiService)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _notiService = notiService;
        }
        
        public async Task<IActionResult> AllNotificationsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _dbContext.UsersNotifications
                                  .Include(l => l.Notification)
                                  .Include(l => l.User) // Include the ApplicationUser
                                  .Where(l => l.RecieverId == userId)
                                  .ToListAsync();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Notifications", Url = Url.Action("AllNotificationsAsync", "Notification") }
            };

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user from the database
            var user = await _dbContext.ApplicationUser
                .Where(u => u.Id == userId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync();

            var notification = new Notification();

            if (user != null)
            {
                notification.SenderUsername = $"{user.FirstName} {user.LastName}";
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Notifications", Url = Url.Action("AllNotificationsAsync", "Notification") },
                new BreadcrumbItem { Title = "Create Notification", Url = Url.Action("Create", "Notification") }
            };

            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }
                //return View(notification);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user from the database
            var user = await _dbContext.ApplicationUser
                .Where(u => u.Id == userId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync();

            notification.SenderId = userId;
            notification.SenderUsername = $"{user.FirstName} {user.LastName}";

            notification.NotificationDateTime = notification.NotificationDateTime.ToUniversalTime();

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
            await SendNotification(notification);

            // If the MessageType is General, create UsersNotification entries for all users
            //if (notification.MessageType == "General")
            //{
            //    var allUsers = await _dbContext.ApplicationUser.ToListAsync();
            //    foreach (var appUser in allUsers)
            //    {
            //        var usersNotification = new UsersNotification
            //        {
            //            RecieverId = appUser.Id,
            //            NotificationId = notification.NotificationId,
            //            IsRead = false
            //        };
            //        _dbContext.UsersNotifications.Add(usersNotification);
            //    }
            //    await _dbContext.SaveChangesAsync();
            //}


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(Notification notification)
        {
            if (notification.MessageType == "General")
            {
                var allUsers = await _dbContext.ApplicationUser.ToListAsync();
                foreach (var appUser in allUsers)
                {
                    var usersNotification = new UsersNotification
                    {
                        RecieverId = appUser.Id,
                        NotificationId = notification.NotificationId,
                        IsRead = false
                    };
                    _dbContext.UsersNotifications.Add(usersNotification);
                }
                await _dbContext.SaveChangesAsync();
                await SendNotificationToAll(notification.SenderUsername, notification.Message);
            }
            else if (notification.MessageType == "Personal")
            {
                var usersNotification = new UsersNotification
                {
                    RecieverId = notification.ReciverCode,
                    NotificationId = notification.NotificationId,
                    IsRead = false
                };
                _dbContext.UsersNotifications.Add(usersNotification);
                await _dbContext.SaveChangesAsync();
                await SendNotificationToClient(notification.SenderUsername, notification.Message, notification.ReciverCode);
            }
            else if (notification.MessageType == "Group")
            {
                var departmentUsers = await _dbContext.ApplicationUser
                    .Include(l => l.Department)
                    .Where(u => u.DepartmentID == notification.ReciverCode)
                    .ToListAsync();
                var departmentName = departmentUsers.First().Department.DepartmentName;
                foreach (var appUser in departmentUsers)
                {
                    var usersNotification = new UsersNotification
                    {
                        RecieverId = appUser.Id,
                        NotificationId = notification.NotificationId,
                        IsRead = false
                    };
                    _dbContext.UsersNotifications.Add(usersNotification);
                }
                await _dbContext.SaveChangesAsync();
                await SendNotificationToGroup(notification.SenderUsername, notification.Message, notification.ReciverCode, departmentName); // assuming Username holds the group name
            }

            return Ok();
        }

        public async Task<IActionResult> SendNotificationToAll(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
            return Ok();
        }

        public async Task<IActionResult> SendNotificationToClient(string user, string message, string userId)
        {
            var hubConnections = _dbContext.HubConnections.Where(con => con.UserId == userId).ToList();
            foreach (var hubConnection in hubConnections)
            {
                await _hubContext.Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, user);
            }
            return Ok();
        }

        public async Task<IActionResult> SendNotificationToGroup(string user, string message, string departmentId, string departmentName)
        {
            var hubConnections = _dbContext.HubConnections.Join(_dbContext.ApplicationUser, c => c.UserId, o => o.Id, (c, o) => new { c.UserId, c.ConnectionId, o.DepartmentID }).Where(o => o.DepartmentID == departmentId).ToList();
            foreach (var hubConnection in hubConnections)
            {
                await _hubContext.Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedGroupNotification", message, departmentName);
            }
            return Ok();
        }


        [HttpGet]
        public JsonResult GetNotifications(bool bIsGetOnlyUnread = false)
        {
            string nToUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _oNotifications = _notiService.GetNotifications(nToUserId, bIsGetOnlyUnread);
            return Json(_oNotifications.Select(n => new {
                n.Notification.SenderUsername,
                n.Notification.MessageType,
                n.Notification.Message,
                n.IsRead,
                n.Notification.NotificationDateTime
            }));
        }
    }
}
