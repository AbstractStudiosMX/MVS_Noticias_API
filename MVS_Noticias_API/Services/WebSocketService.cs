using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace MVS_Noticias_API.Services
{
    public class WebSocketService
    {
        private static readonly List<WebSocket> _connectedSockets = new();

        public static async Task HandleWebSocketConnection(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _connectedSockets.Add(webSocket);

                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        _connectedSockets.Remove(webSocket);
                        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        public static async Task NotifyClientsAsync(object data)
        {
            var message = JsonSerializer.Serialize(data);
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _connectedSockets.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    _connectedSockets.Remove(socket);
                }
            }
        }
    }
}
