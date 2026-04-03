namespace LibraryServer.DTO.BookingBook
{
    public class BookReservationGetAll
    {
        public int RentId { get; set; }
        public int BookId { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool InStock { get; set; }
        public float TotalRate { get; set; }
        public int Count { get; set; }
        public Enums.RentStatus RentStatus { get; set; }
        public string? ImagePath { get; set; }
        public string UserName { get; set; }
        public DateTime StartReservation { get; set; }
        public DateTime EndReservation { get; set; }
    }
}
