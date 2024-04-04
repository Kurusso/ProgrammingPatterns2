using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace CoreApplication.Hubs
{
    public class CustomWebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task HandleWebSocket(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                string userId = context.Request.Query["userId"];
                if (!string.IsNullOrEmpty(userId))
                {
                    _sockets.TryAdd(userId, socket);
                    await ListenSocket(socket, userId);
                }
                else
                {
                    context.Response.StatusCode = 400; // Bad Request
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
            }
        }

        private async Task ListenSocket(WebSocket socket, string userId)
        {
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {

                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            }

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
            WebSocket removedSocket;
            _sockets.TryRemove(userId, out removedSocket);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (_sockets.TryGetValue(userId, out WebSocket socket))
            {
                var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
        }
    }
}
