using System.Net.WebSockets;

namespace LibraryServer.Model
{
    public class ChatClient
    {
        public int UserId { get; set; }
        public int ForumId { get; set; }
        public WebSocket Socket { get; set; }
    }
}
