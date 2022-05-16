using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
  [Authorize (Roles = "admin")]
  [Route("api/[controller]")]
  public class PlayerTournamentController : Controller
  {
    private readonly ApiContext _context;

    public PlayerTournamentController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id:int}")]
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

    [HttpPost("{id:int}")]
    public IActionResult Post([FromRoute] int id, [FromBody] Player player)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null) return NotFound();

      //todo: check authorization

      if (tournament.Players.FirstOrDefault(x => x.Rni == player.Rni) is not null)
        return BadRequest();

      tournament.Players.Add(player);
      _context.SaveChanges();
      return Ok(player);
    }

    [HttpDelete("{idTournament:int}/{rni:int}")]
    public IActionResult Post([FromRoute] int idTournament, [FromRoute] int rni)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == idTournament);
      if (tournament is null)
      {
        return NotFound();
      }

      var player = tournament.Players.FirstOrDefault(x => x.Rni == rni);
      if (player is null)
      {
        return NotFound();
      }

      tournament.Players.Remove(player);
      _context.SaveChanges();
      return Ok(player);
    }
  }
}
