namespace LibraryServer.DTO
{
    public class SolvedTestDto
    {
        public int? TestId { get; set;  }
        public int? UserId { get; set;  }
        public List<int>? Answers { get; set;  }
    }
}
