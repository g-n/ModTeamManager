namespace ModTeamManager.Models
{
    public class ModLog
    {
        public int Id { get; set; }
        public string? Moderator { get; set; }
        public int? UserId { get; set; }
        public string? Login { get; set; }
        public string? Action { get; set; }
        public DateTime? Date { get; set; }
    }
}
