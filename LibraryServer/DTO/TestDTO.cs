using LibraryServer.Model;

namespace LibraryServer.DTO
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public virtual List<QuestionTest> Questions { get; set; }
    }
}
