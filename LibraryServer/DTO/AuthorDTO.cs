using System.ComponentModel.DataAnnotations;

namespace LibraryServer.DTO
{
    public class AuthorDTO
    {
        public int? Id { get; set; } = null;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
    }
}
