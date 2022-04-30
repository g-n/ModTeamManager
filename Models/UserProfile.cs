namespace ModTeamManager.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Login { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }


    }
}

