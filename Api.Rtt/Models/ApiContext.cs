﻿using System.Configuration;
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

        public DbSet<Tournament> Tournaments { get; set; }
        
        public DbSet<TennisCenter> TennisCenters { get; set; }
    }
}