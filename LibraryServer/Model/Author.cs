using System.ComponentModel.DataAnnotations;

namespace LibraryServer.Model
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }

        public string FullName { get => $"{SecondName} {FirstName} {LastName}"; }

        public virtual List<Book> Books { get; set; }
    }
}
