using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
  [Route("api/test")]
  public class PsychTestController : Controller
  {
    private readonly ApiContext _context;

    public PsychTestController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{rni:int}")]
    public IActionResult GetTestResults([FromRoute] int rni)
    {
      var contextPlayer = _context.Players.FirstOrDefault(x => x.Rni == rni);
      if (contextPlayer is null) return NotFound();
      return Ok(contextPlayer.TestResults.OrderByDescending(x => x.LastTimeCompleted));
    }

    [HttpPost("{rni:int}")]
    public IActionResult AddTestResults([FromRoute] int rni, [FromBody] PsychTestResult testResult)
    {
      var contextPlayer = _context.Players.FirstOrDefault(x => x.Rni == rni);
      if (contextPlayer is null) return NotFound();

      if (contextPlayer.TestResults is null)
        contextPlayer.TestResults = new List<PsychTestResult>();

      contextPlayer.TestResults.Add(testResult);
      _context.SaveChanges();
      return Ok(contextPlayer.TestResults);
    }
  }
}
