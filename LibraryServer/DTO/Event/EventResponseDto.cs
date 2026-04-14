namespace LibraryServer.DTO.Event
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string NameEvent { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int CreaterID { get; set; }
        public string CreaterName { get; set; }
    }
}
