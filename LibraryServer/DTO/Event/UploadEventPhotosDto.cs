namespace LibraryServer.DTO.Event
{
    public class UploadEventPhotosDto
    {
        public int EventId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
