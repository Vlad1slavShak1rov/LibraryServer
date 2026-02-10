namespace LibraryServer.DTO
{
    public class TeacherDTO
    {
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}
