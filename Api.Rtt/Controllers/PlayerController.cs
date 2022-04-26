using System.Linq;
using Api.Rtt.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class PlayerController : Controller
  {
    private readonly ApiContext _context;

    public PlayerController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
      return Ok(_context.Players.Take(40));
    }

    [HttpGet("{rni}")]
    public IActionResult Get([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      if (player is null)
      {
        return NotFound(rni);
      }

      return Ok(player);
    }

    [HttpGet("{rni}/tournaments")]
    public IActionResult GetTournaments([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      if (player is null)
      {
        return NotFound(rni);
      }

      return Ok(player.Tournaments);
    }
  }
}
