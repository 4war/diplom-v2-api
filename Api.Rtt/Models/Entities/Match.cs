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
    [System.Text.Json.Serialization.JsonIgnore]
    public int? Player1Rni { get; set; }

    [JsonProperty("player1")]
    public virtual Player Player1 { get; set; }

    [ForeignKey("Player2")]
    [System.Text.Json.Serialization.JsonIgnore]
    public int? Player2Rni { get; set; }

    [JsonProperty("player2")]
    public virtual Player Player2 { get; set; }

    [JsonProperty("start")]
    public DateTime? Start { get; set; }

    [JsonProperty("end")]
    public DateTime? End { get; set; }

    [ForeignKey("Winner")]
    [System.Text.Json.Serialization.JsonIgnore]
    public int? WinnerRni { get; set; }

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
