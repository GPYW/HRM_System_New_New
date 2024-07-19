using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS_Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var connectionId = Context.ConnectionId;

                // Save the user's connection information to the database
                var hubConnection = new HubConnection
                {
                    ConnectionId = connectionId,
                    UserId = userId
                };

                _dbContext.HubConnections.Add(hubConnection);
                await _dbContext.SaveChangesAsync();
            }

            await Clients.Caller.SendAsync("OnConnected");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var hubConnection = _dbContext.HubConnections.FirstOrDefault(con => con.ConnectionId == connectionId);
            if (hubConnection != null)
            {
                _dbContext.HubConnections.Remove(hubConnection);
                await _dbContext.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
