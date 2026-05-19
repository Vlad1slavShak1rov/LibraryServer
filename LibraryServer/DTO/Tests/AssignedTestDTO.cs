namespace LibraryServer.DTO.Tests
{
    public class AssignedTestDTO
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public string TestName { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int TeacherId { get; set; }
        public string TeacherName { get; set; }

        public DateTime AssignedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }
        public double? Percent { get; set; }
    }
}
