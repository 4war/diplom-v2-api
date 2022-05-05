using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities
{
  [Table("bracket")]
  public class Bracket
  {
    [ForeignKey("Tournament"), Key]
    public int TournamentId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Tournament Tournament { get; set; }

    public virtual List<Round> Rounds { get; set; } = new List<Round>();
  }
}
