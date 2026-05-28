using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class QuestionTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TestId { get; set; }
        public int Number { get; set; }
        [Required]
        public string Text { get; set; }
        public int CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public virtual List<QuestionOption> Options { get; set; } = new();

        [ForeignKey(nameof(TestId))]
        public virtual Test Test { get; set; }
    }
}
