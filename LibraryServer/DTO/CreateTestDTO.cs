namespace LibraryServer.DTO
{
    public class CreateTestDTO
    {
        public int UserId { get; set; }
        public string Topic { get; set;  }
        /// <summary>
        /// Описание работы, для чего работа
        /// </summary>
        public string Description { get; set; } = "Без описания";
    }
}
