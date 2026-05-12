namespace LibraryServer.DTO.ForumMessage
{
    public class ForumMessageDto
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int SenderId { get; set; }
        public string Message { get; set; }
        public DateTime DateSend { get; set; }
    }
}
