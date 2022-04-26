using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Api.Rtt.Models.Entities
{
  [Table("match")]
  public class Match
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int Id { get; set; }

    [ForeignKey("Player1")]
    [JsonProperty("playerId1")]
    public int? PlayerId1 { get; set; }

    [JsonProperty("player1")]
    public virtual Player Player1 { get; set; }

    [ForeignKey("Player2")]
    [JsonProperty("playerId2")]
    public int? PlayerId2 { get; set; }

    [JsonProperty("player2")]
    public virtual Player Player2 { get; set; }

    [JsonProperty("start")]
    public DateTime Start { get; set; } = DateTime.Today;

    [JsonProperty("duration")]
    public DateTime Duration { get; set; } = DateTime.Now;

    [ForeignKey("Winner")]
    [JsonProperty("winnerId")]
    public int? WinnerId { get; set; }

    [JsonProperty("winner")]
    public virtual Player Winner { get; set; }


    [JsonProperty("start")]
     public string Score { get; set; }
  }
}
