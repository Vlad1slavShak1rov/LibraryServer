namespace LibraryServer.DTO
{
    public class AddBookDTO
    {
        public int? AuthorID { get; set; }
        public string? Genre { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; } = string.Empty;
    }
}
