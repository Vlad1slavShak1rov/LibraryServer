namespace LibraryServer.DTO.BookingBook
{
    public class BookReservationGetAll
    {
        public string UserName { get; set; }
        public string BookTitle { get; set;  }
        public DateTime StartReservation { get; set; }
        public DateTime EndReservation { get; set; }
    }
}
