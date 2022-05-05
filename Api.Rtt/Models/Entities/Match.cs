using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

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

    [JsonProperty("end")]
    public DateTime End { get; set; } = DateTime.Now;

    [ForeignKey("Winner")]
    [JsonProperty("winnerId")]
    public int? WinnerId { get; set; }

    [JsonProperty("winner")]
    public virtual Player Winner { get; set; }


    [JsonProperty("score")]
     public string Score { get; set; }

     [JsonProperty("placeInRound")]
     public int PlaceInRound { get; set; }

     [System.Text.Json.Serialization.JsonIgnore]
     public virtual Round Round { get; set; }

     [JsonProperty("roundId"), ForeignKey("Round")]
     public int RoundId { get; set; }
  }
}
