namespace LibraryServer.DTO.Material
{
    public class UpdateMaterialsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public IFormFile? File { get; set; }  
    }
}
