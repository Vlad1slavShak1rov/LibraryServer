namespace LibraryServer.DTO.Tests
{
    public class UserTestResultDTO
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public double PercentSuccess { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
