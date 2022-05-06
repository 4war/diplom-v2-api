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
      if (bracket is null) return NotFound();

      foreach (var round in bracket.Rounds)
        round.Matches = round.Matches.OrderBy(m => m.PlaceInRound).ToList();

      bracket.Rounds = bracket.Rounds.OrderByDescending(x => x.Stage).ToList();

      return Ok(bracket);
    }


    [HttpPost("{id:int}/single")]
    public IActionResult Create([FromRoute] int id)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null) return NotFound();

      var bracket = _bracketBuilder.CreateBracket(tournament);

      _context.Brackets.Add(bracket);
      _context.SaveChanges();

      return Ok(bracket);
    }

    [HttpGet("{id:int}/uniquePlayers")]
    public IActionResult GetUniquePlayers([FromRoute] int id)
    {
      var bracket = _context.Brackets.FirstOrDefault(x => x.TournamentId == id);
      if (bracket is null) return NotFound();

      var hashSet = new HashSet<int>();
      var result = new List<Player>();
      foreach (var round in bracket.Rounds)
      foreach (var match in round.Matches)
      foreach (var player in new List<Player>() { match.Player1, match.Player2 })
        if (player is not null)
          if (!hashSet.Contains(player.Rni))
          {
            result.Add(player);
            hashSet.Add(player.Rni);
          }

      return Ok(result);
    }

    [HttpGet("{id:int}/missingPlayers")]
    public IActionResult GetMissingPlayers([FromRoute] int id)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null) return NotFound();
      var playerListRni = new HashSet<int>(tournament.Players.Select(x => x.Rni));

      var bracket = _context.Brackets.FirstOrDefault(x => x.TournamentId == id);
      if (bracket is null) return NotFound();

      var alreadyExistInBracketHashSet = GetRniHashSet(bracket).Select(x => x.Rni);
      var resultListRni = playerListRni.Except(alreadyExistInBracketHashSet);
      var result = resultListRni.Select(x => tournament.Players.First(y => y.Rni == x));

      return Ok(result);
    }


    /// <param name="bracket"></param>
    /// <returns>Unique players rni from bracket from any round and match</returns>
    private static List<Player> GetRniHashSet(Bracket bracket)
    {
      var hashSet = new HashSet<int>();
      var result = new List<Player>();
      foreach (var round in bracket.Rounds)
      foreach (var match in round.Matches)
      foreach (var player in new List<Player>() { match.Player1, match.Player2 })
        if (player is not null)
          if (!hashSet.Contains(player.Rni))
          {
            result.Add(player);
            hashSet.Add(player.Rni);
          }

      return result;
    }

    [HttpPatch]
    public IActionResult Update([FromBody] Bracket bracket)
    {
      if (bracket is null) return BadRequest();

      foreach (var match in bracket.Rounds.SelectMany(round => round.Matches))
      {
        if (match.Player1 is not null) match.Player1Rni = match.Player1.Rni;
        if (match.Player2 is not null) match.Player2Rni = match.Player2.Rni;
        if (match.Winner is not null) match.WinnerRni = match.Winner.Rni;
        _context.Entry(match).State = EntityState.Modified;
        _context.Matches.Update(match);
        _context.SaveChanges();
      }

      _context.Entry(bracket).State = EntityState.Modified;
      _context.Brackets.Update(bracket);
      _context.SaveChanges();
      return Ok(bracket);
    }

    [HttpPatch("move")]
    public IActionResult MoveWinnerInBracket([FromBody] Match wonMatch)
    {
      var wonRound = _context.Rounds.FirstOrDefault(x => x.Id == wonMatch.RoundId);
      if (wonRound is null) return BadRequest();

      if (wonRound.Type == "Final") throw new NotImplementedException();

      var nextRound = wonRound.Bracket.Rounds.FirstOrDefault(x => x.Stage == wonRound.Stage / 2);
      if (nextRound is null) throw new NotImplementedException();

      var nextMatchOrder = wonMatch.PlaceInRound / 2;
      var nextMatch = nextRound.Matches.FirstOrDefault(x => x.PlaceInRound == nextMatchOrder);
      if (nextMatch is null) throw new NotImplementedException();

      var playerPositionInMatch = wonMatch.PlaceInRound % 2;

      switch (playerPositionInMatch)
      {
        case 0:
          nextMatch.Player1 = wonMatch.Winner;
          nextMatch.Player1Rni = wonMatch.Winner.Rni;
          break;
        case 1:
          nextMatch.Player2 = wonMatch.Winner;
          nextMatch.Player2Rni = wonMatch.Winner.Rni;
          break;
      }

      _context.Matches.Update(nextMatch);
      _context.SaveChanges();
      return Ok();
    }
  }
}
