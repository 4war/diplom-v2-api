using System.ComponentModel.DataAnnotations;

namespace Api.Rtt.Models.Entities.Authorization
{
  public class Credentials
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}
