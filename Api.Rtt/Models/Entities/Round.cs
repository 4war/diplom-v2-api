using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Api.Rtt.Models.Entities
{
  [Table("rounds")]
  public class Round
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("matches")]
    public virtual List<Match> Matches { get; set; } = new List<Match>();

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("stage")]
    public int Stage { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Bracket Bracket { get; set; }

    [JsonProperty("bracketId"), ForeignKey("Bracket")]
    public int BracketId { get; set; }
  }
}
