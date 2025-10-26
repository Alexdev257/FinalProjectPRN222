using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FPP.Presentation.Hubs
{
    public class NotificationBookingHub : Hub
    {
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out string? connectionId);
            return connectionId;
        }
    }
}
