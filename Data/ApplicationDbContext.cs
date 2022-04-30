using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModTeamManager.Models;

namespace ModTeamManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ModTeamManager.Models.ChatMessage> ChatMessage { get; set; }
        public DbSet<ModTeamManager.Models.UserProfile> UserProfile { get; set; }
        public DbSet<ModTeamManager.Models.ModLog> ModLog { get; set; }
        public ModTeamManager.Models.ModTeamViewModel InfoModel { get; set; }
    }
}