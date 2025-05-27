using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Models
{
    public class LogiTrackContext : DbContext
    {
        public LogiTrackContext() { }
        public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options) { }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=logitrack.db");
            }
        }
    }
}
