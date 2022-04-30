namespace ModTeamManager.Models
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Service { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
        public string Msg { get; set; }
        public string? Flag { get; set; }
        public string? Url { get; set; }


        public int? UserId { get; set; }

        //public UserProfile? UserProfile { get; set; }

    }
}
