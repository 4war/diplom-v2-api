using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities.ManyWithMany
{
  [Table("player_tournament")]
  public class PlayerTournament
  {
    [Key]
    public int Rni { get; set; }

    [Key]
    public int Id { get; set; }
  }
}
