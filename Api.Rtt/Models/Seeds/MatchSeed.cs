using System;
using System.Collections.Generic;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class MatchSeed
  {
    private readonly ApiContext _context;

    public MatchSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Match> GetList()
    {
      var player1 = _context.Players.Find(40092);
      var player2 = _context.Players.Find(40014);

      var tournament = _context.Tournaments.Find(3);
      var winner = player1;

      var match = new Match()
      {
        Player1 = player1,
        Player2 = player2,
        Winner = winner,
        Tournament = tournament,
        Score = "64 64",
        Duration= new TimeSpan(days: 0, hours: 1, minutes: 40, seconds: 0),
        Start = tournament.DateEnd,
      };

      return new List<Match>() { match };
    }
  }
}
