using LibraryServer.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool InStock { get; set; }
        public float TotalRate { get; set; }
        public string? ImagePath { get; set; }
    }
}
