using System;
using System.Collections.Generic;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Helpers
{
  public class BracketBuilder
  {
    public List<Bracket> CreateBracketsForFactory(TournamentFactory factory)
    {
      var list = new List<Bracket>();
      foreach (var tournament in factory.Tournaments)
      {
        if (tournament.Stage == (int)Stage.Qual) continue;
        var bracket = CreateBracket(tournament);
        list.Add(bracket);
      }

      return list;
    }

    public Bracket CreateBracket(Tournament tournament)
    {
      if (tournament.NetRange != 32)
      {
        throw new NotImplementedException();
      }

      var bracket = new Bracket()
      {
        Tournament = tournament,
      };

      var final = CreateFinal();
      bracket.Rounds.Add(final);

      for (var stage = 2; stage < tournament.NetRange; stage *= 2)
      {
        var round = CreateRound(stage);
        bracket.Rounds.Add(round);
      }

      return bracket;
    }

    private Round CreateFinal()
    {
      var final = new Round()
      {
        Matches = new List<Match>() { new() { PlaceInRound = 0, } },
        Stage = 1,
        Type = "Final",
      };

      return final;
    }

    private Round CreateRound(int stage)
    {
      var round = new Round()
      {
        Type = "Winnerbracket",
        Stage = stage,
      };

      for (var i = 0; i < stage; i++)
      {
        var match = new Match() { PlaceInRound = i, };
        round.Matches.Add(match);
      }

      return round;
    }
  }
}
