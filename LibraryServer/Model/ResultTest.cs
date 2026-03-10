using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class ResultTest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set;}
        public string Description { get; set; }
        public int? Score { get; set; }
        public bool IsSuccess { get; set; } = false;
        public int TotalQuest { get; set; }
        public int CorrectAnswers { get; set; } = 0;
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}
