namespace LibraryServer.DTO.Event
{
    public class UpdateEventDto
    {
        public int Id { get; set; }
        public string NameEvent { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}
