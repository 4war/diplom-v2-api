using System;
using System.Linq;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class MatchController : Controller
  {
    private readonly ApiContext _context;
    private readonly Random _random = new Random();

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
    [Authorize(Roles = "org")]
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

    [HttpGet("{rni:int}")]
    public IActionResult GetPlayerMatches([FromRoute] int rni)
    {
      var contextPlayer = _context.Players.FirstOrDefault(x => x.Rni == rni);
      if (contextPlayer is null) return NotFound();

      var matches = _context.Matches
        .Where(x => x.Player1Rni == rni || x.Player2Rni == rni)
        .Where(x => x.Player1Rni != null && x.Player2Rni != null)
        .Where(x => x.WinnerRni != null)
        .OrderByDescending(x => x.Start);

      return Ok(matches);
    }

    [HttpGet("day/{id:int}")]
    public IActionResult GetDay([FromRoute] int id)
    {
      var contextMatch = _context.Matches.FirstOrDefault(x => x.Id == id);
      if (contextMatch is null) return NotFound();
      if (contextMatch.Schedule is null)
      {
        var stage = contextMatch.Round.Stage;
        var daysToAdd = (int)Math.Log2(stage);
        var dayStart = contextMatch.Round.Bracket.Tournament.DateStart;
        var result = dayStart.AddDays(daysToAdd);
        return Ok(result);
      }

      return Ok(contextMatch.Schedule.Day);
    }

    [HttpGet("{id:int}/randomize/duration")]
    public IActionResult RandomizeDuration(int id)
    {
      var match = _context.Matches.Find(id);
      if (match is null)
      {
        return NotFound();
      }

      if (!match.Start.HasValue)
      {
        match.Start = _random.RandomizeStartDateTime(match);
      }

      var duration = _random.RandomizeDurationInSeconds(match);
      match.End = match.Start.Value.AddSeconds(duration);
      _context.Matches.Update(match);
      _context.SaveChanges();
      return Ok();
    }
  }
}
