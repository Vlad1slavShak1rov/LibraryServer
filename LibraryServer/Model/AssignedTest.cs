using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class AssignedTest
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int TestId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        [ForeignKey(nameof(StudentId))]
        public virtual User Student { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual User Teacher { get; set; }
        [ForeignKey(nameof(TestId))]
        public virtual Test Test { get; set; }
    }
}
