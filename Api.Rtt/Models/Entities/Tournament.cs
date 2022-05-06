﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Api.Rtt.Models.Entities
{
  [Table("tournament")]
  public class Tournament
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Category { get; set; }
    public int Age { get; set; }
    public int Stage { get; set; }
    public int Gender { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public DateTime DateRequest { get; set; }
    public int NetRange { get; set; }

    public virtual TennisCenter TennisCenter { get; set; }

    [ForeignKey("TennisCenter")] public int TennisCenterId { get; set; }

    public virtual Tournament Qualification { get; set; }

    [ForeignKey("Qualification")] public int? QualificationId { get; set; }

    public int NumberOfQualificationWinners { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual List<Player> Players { get; set; } = new();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual TournamentFactory Factory { get; set; }

    [ForeignKey("Factory")]
    public int FactoryId { get; set; }
  }

  public enum Stage
  {
    Main,
    Qual,
  }

  public enum Gender
  {
    Male,
    Female
  }
}
