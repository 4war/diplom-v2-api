using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Filter;
using Api.Rtt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost("filter")]
    public async Task<IActionResult> GetFilteredList([FromBody] PlayerFilterOptions filterOptions)
    {
      var list = await _context.Players.ToListAsync();
      var queryable = list
        .Where(x => x.Surname.StartsWith(filterOptions.StartWith, StringComparison.InvariantCultureIgnoreCase))
        .Where(x => string.IsNullOrEmpty(filterOptions.City) || x.City.StartsWith(filterOptions.City, StringComparison.InvariantCultureIgnoreCase))
        .Where(x => !filterOptions.Gender.HasValue || x.Gender == filterOptions.Gender);

      if (filterOptions.Skip.HasValue)
      {
        queryable = queryable.Skip(filterOptions.Skip.Value);
      }

      if (filterOptions.Take.HasValue)
      {
        queryable = queryable.Take(filterOptions.Take.Value);
      }

      if (!queryable.Any())
      {
        return NotFound();
      }

      return Ok(queryable);
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
