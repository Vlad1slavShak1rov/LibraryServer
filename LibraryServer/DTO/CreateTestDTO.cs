namespace LibraryServer.DTO
{
    public class CreateTestDTO
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int QuestQuantity { get; set; } = 5;
        /// <summary>
        /// Описание работы, для чего работа
        /// </summary>
        public string Description { get; set; } = "Без описания";
    }
}
