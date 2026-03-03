using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class QuestionTest
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}
