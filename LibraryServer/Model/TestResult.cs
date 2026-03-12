using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class TestResult
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public double PercentSuccess { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(TestId))]
        public virtual Test Test { get; set; }
    }
}
