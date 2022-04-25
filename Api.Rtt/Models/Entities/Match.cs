using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Api.Rtt.Models.Entities
{
  [Table("match")]
  public class Match
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Player1")]
    public int PlayerId1 { get; set; }
    public virtual Player Player1 { get; set; }

    [ForeignKey("Player2")]
    public int PlayerId2 { get; set; }
    public virtual Player Player2 { get; set; }

    public DateTime Start { get; set; }
    public TimeSpan Duration { get; set; }

    [ForeignKey("Winner")]
    public int? WinnerId { get; set; }
    public virtual Player Winner { get; set; }

    public string Score { get; set; }

    [ForeignKey("Tournament")]
    public int TournamentId { get; set; }
    public virtual Tournament Tournament { get; set; }
  }
}
