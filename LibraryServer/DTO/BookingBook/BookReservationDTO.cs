namespace LibraryServer.DTO.BookingBook
{
    public class BookReservationDTO
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime StartReservation { get; set; }
        public DateTime EndReservation { get; set; }
    }
}
