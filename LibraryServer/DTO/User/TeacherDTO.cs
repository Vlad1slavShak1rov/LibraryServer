namespace LibraryServer.DTO.User
{
    public class TeacherDTO
    {
        public int? UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; }
    }
}
