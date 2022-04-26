using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class BracketController : Controller
  {
    private readonly ApiContext _context;

    public BracketController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
      var result = _context.Brackets.FirstOrDefault(x => x.TournamentId == id);
      return result is null ? NotFound() : Ok(result);
    }
  }
}
