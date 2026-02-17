namespace LibraryServer.Service
{
    using System.Net.WebSockets;
    using System.Text;

    public static class ChatHandler
    {
        private static List<WebSocket> _clients = new List<WebSocket>();

        public static async Task HandleAsync(WebSocket socket)
        {
            _clients.Add(socket);

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _clients.Remove(socket);
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed",
                        CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await Broadcast(message);
                }
            }
        }

        private static async Task Broadcast(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }
    }

}
