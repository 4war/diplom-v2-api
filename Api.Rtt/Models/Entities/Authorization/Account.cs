using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Api.Rtt.Models.Entities.Authorization
{
  [Table("account")]
  public class Account
  {
    [Key,  DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; }
    public int Password { get; set; }
    public string Roles { get; set; }

    public bool ConfirmedEmail { get; set; }

    public virtual Player Player { get; set; }

    [ForeignKey("Player")]
    public int? Rni { get; set; }
  }
}
