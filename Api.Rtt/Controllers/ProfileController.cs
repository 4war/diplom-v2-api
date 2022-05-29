using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class ProfileController : Controller
  {
    private readonly ApiContext _context;

    public ProfileController(ApiContext context)
    {
      _context = context;
    }

    [HttpPatch("connect/{rni:int}")]
    public IActionResult Connect([FromRoute] int rni, [FromBody] Account account)
    {
      if (account is null || rni == 0)
      {
        return BadRequest();
      }

      var contextAccount = _context.Accounts.FirstOrDefault(x => x.Email == account.Email);
      if (contextAccount is null)
      {
        return NotFound();
      }

      var player = _context.Players.FirstOrDefault(x => x.Rni == account.Player.Rni);
      if (player is null)
      {
        return NotFound();
      }

      contextAccount.Rni = rni;
      _context.SaveChanges();
      return Ok();
    }
  }
}
