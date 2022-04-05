using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities
{
  [Table("city")]
  public class City
  {
    [Key]
    public string Name { get; set; }
  }
}
