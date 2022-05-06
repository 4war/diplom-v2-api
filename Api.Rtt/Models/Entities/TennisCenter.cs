using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities
{
  [Table("tennis_center")]
  public class TennisCenter
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }

    public virtual List<Court> Courts { get; set; } = new ();
  }
}
