namespace LibraryServer.Service
{
    using LibraryServer.DbContext;
    using LibraryServer.Model;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;

    public static class ChatHandler
    {
        private static List<ChatClient> _clients = new();

        public static async Task HandleAsync(WebSocket socket, int userId, int forumId, IServiceProvider services)
        {
            var client = new ChatClient
            {
                UserId = userId,
                ForumId = forumId,
                Socket = socket
            };

            _clients.Add(client);

            var buffer = new byte[4096];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    var messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await SaveMessage(services, forumId, userId, messageText);

                    await BroadcastToRoom(forumId, userId, messageText);
                }
            }
            finally
            {
                _clients.Remove(client);

                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closed",
                    CancellationToken.None);
            }
        }

        private static async Task SaveMessage(
            IServiceProvider services,
            int forumId,
            int userId,
            string message)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<LibraryContext>();

            var msg = new ForumMessage
            {
                ForumId = forumId,
                SenderId = userId,
                Message = message,
                DateSend = DateTime.UtcNow,
                ApplicationPath = ""
            };

            db.ForumMessages.Add(msg);

            await db.SaveChangesAsync();
        }

        private static async Task BroadcastToRoom(int forumId, int senderId, string message)
        {
            var payload = new
            {
                forumId,
                senderId,
                message,
                date = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            var roomClients = _clients.Where(c => c.ForumId == forumId);

            foreach (var client in roomClients)
            {
                if (client.Socket.State == WebSocketState.Open)
                {
                    await client.Socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }
    }


}
