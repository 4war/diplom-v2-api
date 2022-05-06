using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Api.Rtt.Models.Entities
{
  [Table("court")]
  public class Court
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name"), Required]
    public string Name { get; set; }

    [JsonProperty("surface"), Required]
    public string Surface { get; set; }

    [JsonProperty("opened"), Required]
    public bool Opened { get; set; }
  }
}
