using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Identity;

namespace Talabat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppIdentityDbContext _dbContext;

        public ChatHub(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Send(string recipientUserId, string message)
        {
            //await Clients.All.SendAsync("ReceiveMessage",userName, message);
            var senderUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var chatMessage = new ChatMessage
            {
                SenderUserId = senderUserId,
                RecipientUserId = recipientUserId,
                Message = message,
                Timestamp = DateTimeOffset.UtcNow,
                ConnectionId = Context.ConnectionId
            };

            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();

            await Clients.User(recipientUserId).SendAsync("ReceiveMessage", senderUserId, message, Context.ConnectionId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var userConnection = new UserConnection
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId
                };

                _dbContext.UserConnections.Add(userConnection);
                await _dbContext.SaveChangesAsync();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var userConnection = await _dbContext.UserConnections
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConnectionId == Context.ConnectionId);

                if (userConnection != null)
                {
                    _dbContext.UserConnections.Remove(userConnection);
                    await _dbContext.SaveChangesAsync();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
