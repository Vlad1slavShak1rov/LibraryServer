
using System.ComponentModel.DataAnnotations;

namespace LibraryServer.DTO
{
    public class StudentDTO
    {
        public int? Id { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ClassNum { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; }
    }
}
