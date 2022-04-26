using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Api.Rtt.Models.Entities
{
  [Table("bracket")]
  public class Bracket
  {
    [ForeignKey("Tournament"), Key]
    public int TournamentId { get; set; }

    [JsonIgnore]
    public virtual Tournament Tournament { get; set; }

    public virtual List<Round> Rounds { get; set; }
  }
}
