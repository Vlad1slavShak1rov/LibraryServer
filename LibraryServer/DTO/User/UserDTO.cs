using LibraryServer.Model;
using System.ComponentModel.DataAnnotations;

namespace LibraryServer.DTO.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public Enums.Role Role { get; set; }

    }
}
