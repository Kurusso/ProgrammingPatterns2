using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace CoreApplication.Hubs
{
    public class CustomWebSocketManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _sockets = new();

        public async Task HandleWebSocket(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                string userId = context.Request.Query["userId"];
                if (!string.IsNullOrEmpty(userId))
                {
                    if (!_sockets.ContainsKey(userId)) {
                        _sockets[userId] = new();
                    }
                    var socketId = new Guid().ToString();

                    _sockets[userId][socketId] = socket;
                    // _sockets.TryAdd(userId, socket);
                    await ListenSocket(socket, userId, socketId);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                context.Response.StatusCode = 400; 
            }
        }

        private async Task ListenSocket(WebSocket socket, string userId, string socketId)
        {
            byte[] buffer = new byte[4096];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _sockets[userId].Remove(socketId, out _);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
            foreach (var userSocket in _sockets[userId])
            {
                await userSocket.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);  
            }
        }
    }
}
