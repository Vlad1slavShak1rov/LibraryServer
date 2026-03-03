using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.DTO
{
    public class RemoveFavouriteRequestDTO
    {
        public int? UserId { get; set; }
        public int? BookId { get; set;  }
    }
}
