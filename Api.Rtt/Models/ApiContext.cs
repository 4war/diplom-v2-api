using Api.Rtt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Models
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            
        }
        
        public DbSet<Tournament> Tournaments { get; set; }
        
        public DbSet<TennisCenter> TennisCenters { get; set; }
    }
}