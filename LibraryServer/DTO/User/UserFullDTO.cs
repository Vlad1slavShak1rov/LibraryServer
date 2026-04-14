namespace LibraryServer.DTO.User
{
    public class UserFullDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public Enums.Role Role { get; set; }

        public string FullName { get; set; } = string.Empty;

        // student
        public string? ClassNum { get; set; }

        // teacher
        public string? Contact { get; set; }
    }
}
