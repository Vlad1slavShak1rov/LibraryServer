namespace LibraryServer.DTO
{
    public class RegistrationDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Enums.Role? Role { get; set; } = null;
    }
}
