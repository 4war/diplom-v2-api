using System.Configuration;
using Api.Rtt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Models
{
  public class ApiContext : DbContext
  {
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Round>()
        .HasOne(r => r.Bracket)
        .WithMany(b => b.Rounds)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Match>()
        .HasOne(m => m.Round)
        .WithMany(r => r.Matches)
        .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<TournamentFactory> TournamentFactories { get; set; }

    public DbSet<TennisCenter> TennisCenters { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<City> Cities { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<Bracket> Brackets { get; set; }
    public DbSet<Round> Rounds { get; set; }
  }
}
