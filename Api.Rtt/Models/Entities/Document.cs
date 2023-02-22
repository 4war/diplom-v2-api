using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Models.Entities
{
  [Table("document")]
  public class Document
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public byte[] File { get; set; }
    
    public int TournamentId { get; set; }
    
    [ForeignKey(nameof(TournamentId))]
    public virtual Tournament Tournament { get; set; }
    
    public string Status { get; set; }
  }
}
