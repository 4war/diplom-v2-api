using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class MatchController : Controller
  {
    private readonly ApiContext _context;

    public MatchController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
      var match = _context.Matches.FirstOrDefault(x => x.Id == id);
      if (match is null)
      {
        return NotFound();
      }

      return Ok(match);
    }


    [HttpPatch]
    public IActionResult Patch([FromBody] Match match)
    {
      if (match is null) return BadRequest();

      if (match.Winner is not null) match.WinnerRni = match.Winner.Rni;
      if (match.Player1 is not null) match.Player1Rni = match.Player1.Rni;
      if (match.Player2 is not null) match.Player2Rni = match.Player2.Rni;
      _context.Entry(match).State = EntityState.Detached;
      _context.Matches.Update(match);
      _context.SaveChanges();
      return Ok(match);
    }
  }
}
