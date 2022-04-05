using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities
{
  [Table("player")]
  public class Player
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Rni { get; set; }

    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string City { get; set; }
    public int Point { get; set; }
    public int Gender { get; set; }
  }
}
