using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Helpers;
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
    private readonly BracketBuilder _bracketBuilder = new();

    public BracketController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
      var bracket = _context.Brackets.FirstOrDefault(x => x.TournamentId == id);

      if (bracket is null)
        return NotFound();

      foreach (var round in bracket.Rounds)
        round.Matches = round.Matches.OrderBy(m => m.PlaceInRound).ToList();

      bracket.Rounds = bracket.Rounds.OrderByDescending(x => x.Stage).ToList();

      return Ok(bracket);
    }


    [HttpPost("{id:int}/single")]
    public IActionResult Create([FromRoute] int id)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null)
      {
        return NotFound();
      }

      var bracket = _bracketBuilder.CreateBracket(tournament);

      _context.Brackets.Add(bracket);
      _context.SaveChanges();

      return Ok(bracket);
    }
  }
}
