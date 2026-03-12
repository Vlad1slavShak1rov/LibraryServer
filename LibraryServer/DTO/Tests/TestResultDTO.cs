namespace LibraryServer.DTO.Tests
{
    public class TestResultDTO
    {
        public int TestId { get; set; }
        public int UserId { get; set; }
        public double PercentSuccess { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }
}
