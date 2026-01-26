using LibraryServer.Model;
using System.ComponentModel.DataAnnotations;

namespace LibraryServer.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public Enums.Role Role { get; set; }

    }
}
