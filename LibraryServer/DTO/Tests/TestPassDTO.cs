namespace LibraryServer.DTO.Tests
{
    public class TestPassDTO
    {
        public int Id { get; set; }
        public string TestName { get; set; }
        public string? TestDescription { get; set; }
        public List<QuestionPassDTO> Questions { get; set; }
    }
}
