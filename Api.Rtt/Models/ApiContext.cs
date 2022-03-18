using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Models
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            
        }
        
        public DbSet<Tournament> Tournaments { get; set; }
    }
}