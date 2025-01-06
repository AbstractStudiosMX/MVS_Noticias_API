using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace MVS_Noticias_API.Services
{
    public static class WebSocketService
    {
        private static readonly ConcurrentBag<WebSocket> _connectedSockets = new();
        public static int MaxConnections { get; set; } = 100; // Valor por defecto
        public static TimeSpan InactivityTimeout { get; set; } = TimeSpan.FromMinutes(5); // Valor por defecto

        public static async Task HandleWebSocketConnection(HttpContext context)
        {
            if (_connectedSockets.Count >= MaxConnections)
            {
                context.Response.StatusCode = 503; // Servicio no disponible
                await context.Response.WriteAsync("Max WebSocket connections reached.");
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _connectedSockets.Add(webSocket);

                var lastActivity = DateTime.UtcNow;

                try
                {
                    var buffer = new byte[1024 * 4];
                    while (webSocket.State == WebSocketState.Open)
                    {
                        // Cerrar la conexión si está inactiva por demasiado tiempo
                        if (DateTime.UtcNow - lastActivity > InactivityTimeout)
                        {
                            break;
                        }

                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.CloseStatus.HasValue)
                        {
                            break;
                        }

                        lastActivity = DateTime.UtcNow; // Actualiza la última actividad
                    }
                }
                finally
                {
                    _connectedSockets.TryTake(out var _); // Elimina la conexión de la lista
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
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
                    try
                    {
                        await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        _connectedSockets.TryTake(out var _);
                    }
                }
                else
                {
                    _connectedSockets.TryTake(out var _);
                }
            }
        }
    }
}
