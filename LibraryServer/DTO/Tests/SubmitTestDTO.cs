namespace LibraryServer.DTO.Tests
{
    public class SubmitTestDTO
    {
        public int TestId { get; set; }
        public int UserId { get; set; }
        public List<UserAnswerDTO> Answers { get; set; }
    }
}
