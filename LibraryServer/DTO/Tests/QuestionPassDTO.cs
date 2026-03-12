namespace LibraryServer.DTO.Tests
{
    public class QuestionPassDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
        public List<OptionDTO> Options { get; set; }
    }
}
