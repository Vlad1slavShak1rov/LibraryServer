namespace LibraryServer.DTO.Event
{
    public class CreateEventDto
    {
        public string NameEvent { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int CreaterID { get; set; }
    }
}
