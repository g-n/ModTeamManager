using Microsoft.EntityFrameworkCore;

namespace ModTeamManager.Models
{
    public class UserInfoViewModel
    {
        public UserProfile UserProfile { get; set; }
        public IEnumerable<ChatMessage> ChatMessages{ get; set; }
    }
}
