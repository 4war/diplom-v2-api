using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Rtt.Models.Entities
{
  [Table("test_result")]
  public class PsychTestResult
  {
      [Key,  DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [ForeignKey("Player")]
      public int Rni { get; set; }

      [JsonIgnore]
      public virtual Player Player { get; set; }
      public int Defensive { get; set; }
      public int Active { get; set; }
      public int Reactive { get; set; }
      public int Moral { get; set; }
      public DateTime LastTimeCompleted { get; set; }
  }
}
