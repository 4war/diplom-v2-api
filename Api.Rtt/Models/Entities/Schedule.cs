using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Rtt.Models.Entities
{
  [Table("schedule")]
  public class Schedule
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Factory")]
    public int FactoryId { get; set; }

    [JsonIgnore]
    public virtual TournamentFactory Factory { get; set; }

    [Required]
    public DateTime Day { get; set; }

    public virtual List<Match> Matches { get; set; } = new List<Match>();
  }
}
