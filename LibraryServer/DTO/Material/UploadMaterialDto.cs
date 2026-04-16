using LibraryServer.Enums;

public class UploadMaterialDto
{
    public int SenderId { get; set; }
    public Subject Subject { get; set; }
    public string Name { get; set; }
    public IFormFile File { get; set; }
}