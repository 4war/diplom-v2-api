using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Filter;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class PlayerController : Controller
  {
    private readonly ApiContext _context;
    public PlayerController(ApiContext context) => _context = context;

    [HttpGet("list")]
    public IActionResult Get() => Ok(_context.Players.Take(40));

    [HttpPost("filter")]
    public async Task<IActionResult> GetFilteredList([FromBody] PlayerFilterOptions filterOptions)
    {
      var list = await _context.Players.ToListAsync();
      var queryable = list
        .Where(x => string.IsNullOrEmpty(filterOptions.Surname) ||
                    x.Surname.StartsWith(filterOptions.Surname, StringComparison.InvariantCultureIgnoreCase))
        .Where(x => filterOptions.City is null || string.IsNullOrEmpty(filterOptions.City) ||
                    x.City.StartsWith(filterOptions.City, StringComparison.InvariantCultureIgnoreCase))
        .Where(x => !filterOptions.Gender.HasValue || x.Gender == filterOptions.Gender.Value)
        .Where(x => filterOptions.PointsUntil == 0 ||
                    x.Point >= filterOptions.PointsFrom && x.Point <= filterOptions.PointsUntil)
        .Where(x => filterOptions.DobYearUntil == 0 || x.DateOfBirth.Year > filterOptions.DobYearFrom &&
          x.DateOfBirth.Year < filterOptions.DobYearUntil);

      return Ok(queryable);
    }

    [HttpGet("{rni:int}")]
    public IActionResult Get([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      return player is null ? NotFound(rni) : Ok(player);
    }

    [HttpGet("{rni:int}/tournaments")]
    public IActionResult GetTournaments([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      return player is null ? NotFound(rni) : Ok(player.Tournaments);
    }

    [HttpGet("winRate/{rni:int}")]
    public IActionResult GetWinRate([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      if (player is null) return NotFound();

      var matches = _context.Matches.Where(x => x.Player1Rni == rni || x.Player2Rni == rni).ToList();
      var winRate = matches.Count(x => x.WinnerRni == rni) / (double)matches.Count;
      return Ok((int)Math.Round(winRate * 100, 0));
    }

    [HttpGet("{rni:int}/tournamentResults")]
    public IActionResult GetTournamentResults([FromRoute] int rni)
    {
      var player = _context.Players.Find(rni);
      if (player is null) return NotFound();

      var result = new List<TournamentResult>();
      foreach (var tournament in player.Tournaments)
      {
        var tuple = DefinePlaceAndPoint(tournament, rni);
        if (tuple is null) continue;

        result.Add(new TournamentResult
        {
          Tournament = tournament,
          Point = tuple.Item2,
          Place = tuple.Item1
        });
      }

      return Ok(result.OrderBy(x => x.Tournament.DateStart));
    }


    private readonly Dictionary<int, int> _pointDictionary = new Dictionary<int, int>()
    {
      [1] = 150,
      [2] = 105,
      [4] = 53,
      [8] = 34,
      [16] = 22,
      [32] = 16,
    };

    private Tuple<string, int> DefinePlaceAndPoint(Tournament tournament, int rni)
    {
      var bracket = _context.Brackets.Find(tournament.Id);
      var rounds = bracket.Rounds.OrderBy(x => x.Stage).ToList();
      var final = rounds.First().Matches.Single();
      if (final.WinnerRni is null) return null;

      var place = 1;
      if (final.WinnerRni == rni)
      {
        return new Tuple<string, int>(place.ToString(), _pointDictionary[place]);
      }

      foreach (var round in rounds)
      {
        place *= 2;
        foreach (var match in round.Matches)
          if (match.Player1Rni == rni || match.Player2Rni == rni)
            return new Tuple<string, int>($"{place / 2 + 1}-{place}", _pointDictionary[place]);
      }

      return new Tuple<string, int>("16-32", 16);
    }
  }
}
