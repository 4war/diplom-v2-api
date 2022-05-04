using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class PlayerTournamentController : Controller
  {
    private readonly ApiContext _context;

    public PlayerTournamentController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
      var playerList = _context.Tournaments
        .FirstOrDefault(x => x.Id == id);

      if (playerList is null)
        return NotFound();

      var result = playerList.Players
        .OrderByDescending(x => x.Point)
        .AsQueryable();

      return Ok(result);
    }

    [HttpPost("{id}")]
    public IActionResult Post([FromRoute] int id, [FromBody] Player player)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null)
      {
        return NotFound();
      }

      if (tournament.Players.FirstOrDefault(x => x.Rni == player.Rni) is not null)
      {
        return BadRequest();
      }

      tournament.Players.Add(player);
      return Ok(player);
    }
  }
}
