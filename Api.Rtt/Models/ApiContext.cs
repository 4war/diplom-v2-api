﻿using System.Configuration;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.ManyWithMany;
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

      modelBuilder.Entity<PlayerTournament>().HasKey(x => new { x.Rni, x.Id });
    }

    public DbSet<Tournament> Tournaments { get; set; }

    public DbSet<TennisCenter> TennisCenters { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<City> Cities { get; set; }

    public DbSet<PlayerTournament> PlayerTournaments { get; set; }
  }
}
