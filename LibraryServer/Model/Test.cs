using LibraryServer.Service;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
        
        public int BookId { get; set;  }
        public string Subject { get; set; }
        
        public virtual List<QuestionTest> Questions { get; set; }
        public virtual List<ResultTest> ResultTests { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; }
    }
}
