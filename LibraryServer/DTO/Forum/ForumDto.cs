namespace LibraryServer.DTO.Forum
{
    public class ForumDto
    {
        public int Id { get; set; }
        public int CreaterID { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string AdditionalInfo { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
    }
}
