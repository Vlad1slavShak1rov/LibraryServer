namespace LibraryServer.DTO.BookingBook
{
    public class CreateBookingDto
    {
        public int? UserId { get; set;  }
        public int? BookId { get; set;  }
        public DateTime? DateStart { get; set;  } = DateTime.Now.Date;
        public DateTime? DateEnd { get; set;  }
    }
}
