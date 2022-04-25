using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Models;
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
  }
}
