using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Common.Http;
using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Common.SignalRCore
{
    [Authorize]
    public abstract class HubBase<T> : Hub<T> where T : class
    {
        protected readonly ILogger<T> Logger;

        protected readonly IConnectionManager ConnectionManager;

        protected HubBase(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        protected virtual string GetUserId(ClaimsPrincipal user)
        {
            Claim userIdClaim = null; // TODO

            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value)) return string.Empty;

            return userIdClaim.Value;
        }

        public async override Task OnConnectedAsync()
        {
            try
            {
                string userId = GetUserId(Context.User);

                if (ConnectionManager.IsOnline(userId, Context.ConnectionId))
                {
                    Logger.LogWarning($"ConnectionId was existing : {Context.ConnectionId}");

                    Context.Abort();
                }

                Connection newConnection = new()
                {
                    ConnectionID = Context.ConnectionId,

                    UserAgent = Context.GetHttpContext().Request.Headers[HeaderKey.UserAgent]
                };

                ConnectionManager.AddConnection(userId, newConnection);

                Heartbeat();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Connection initialization failed : {ex.Message} - {ex.StackTrace} - {ex.InnerException?.Message}");

                Context.Abort();
            }

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.RemoveConnection(GetUserId(Context.User), Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private void Heartbeat()
        {
            var heartbeat = Context.Features.Get<IConnectionHeartbeatFeature>();

            heartbeat.OnHeartbeat(state =>
            {
                (HttpContext context, string connectionId) = ((HttpContext, string))state;

                var connectionManager = (IConnectionManager)context.RequestServices.GetService(typeof(IConnectionManager));

                var existingConnection = connectionManager.GetConnection(GetUserId(context.User), connectionId);

                if (existingConnection != null)
                {
                    existingConnection.LatestPingTime = DateTime.UtcNow;

                    connectionManager.UpdateConnection(GetUserId(context.User), existingConnection);
                }
            }, (Context.GetHttpContext(), Context.ConnectionId));
        }
    }
}