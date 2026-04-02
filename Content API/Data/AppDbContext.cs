using Content_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Content_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; }
    }
}
