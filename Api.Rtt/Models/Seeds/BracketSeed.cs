using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;
using MoreLinq;

namespace Api.Rtt.Models.Seeds
{
  public class BracketSeed
  {
    private ApiContext _context;

    public BracketSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Bracket> GetList()
    {
      var result = new List<Bracket>()
      {
        new Bracket()
        {
          Tournament = _context.Tournaments.Find(28),
          Rounds = new List<Round>()
          {
            // 1/16
            new Round()
            {
              Stage = 16,
              Type = "Winnerbracket",
              Matches = new List<Match>()
              {
                new Match()
                {
                  Player1 = _context.Players.Find(31157),
                  Player2 = null,
                  Score = null,
                  Winner = _context.Players.Find(31157),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37826),
                  Player2 = _context.Players.Find(42631),
                  Score = "62 61",
                  Winner = _context.Players.Find(37826),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(39892),
                  Player2 = _context.Players.Find(35354),
                  Score = "60 61",
                  Winner = _context.Players.Find(35354),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(43071),
                  Player2 = _context.Players.Find(33188),
                  Score = "60 60",
                  Winner = _context.Players.Find(33188),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(32007),
                  Player2 = null,
                  Score = null,
                  Winner = _context.Players.Find(32007),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(34373),
                  Player2 = _context.Players.Find(33600),
                  Score = "61 60",
                  Winner = _context.Players.Find(33600),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37795),
                  Player2 = _context.Players.Find(40128),
                  Score = "63 64",
                  Winner = _context.Players.Find(37795),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(35361),
                  Player2 = _context.Players.Find(35398),
                  Score = "60 46 63",
                  Winner = _context.Players.Find(35398),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(31798),
                  Player2 = _context.Players.Find(38002),
                  Score = "61 60",
                  Winner = _context.Players.Find(31798),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(38745),
                  Player2 = _context.Players.Find(35415),
                  Score = "60 60",
                  Winner = _context.Players.Find(38745),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37768),
                  Player2 = _context.Players.Find(37666),
                  Score = "60 60",
                  Winner = _context.Players.Find(37666),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(35161),
                  Player2 = _context.Players.Find(32133),
                  Score = "62 60",
                  Winner = _context.Players.Find(32133),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(33213),
                  Player2 = _context.Players.Find(37818),
                  Score = "61 60",
                  Winner = _context.Players.Find(37818),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(38248),
                  Player2 = _context.Players.Find(33330),
                  Score = "60 60",
                  Winner = _context.Players.Find(33330),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(38144),
                  Player2 = _context.Players.Find(31164),
                  Score = "60 62",
                  Winner = _context.Players.Find(31164),
                },
                new Match()
                {
                  Player1 = null,
                  Player2 = null, //Мартышков Зимний Кубок ФТСО
                  Score = null,
                  Winner = null,
                },
              }
            },

            // 1/8
            new Round()
            {
              Stage = 8,
              Type = "Winnerbracket",
              Matches = new List<Match>()
              {
                new Match()
                {
                  Player1 = _context.Players.Find(31157),
                  Player2 = _context.Players.Find(37826),
                  Score = "61 61",
                  Winner = _context.Players.Find(31157),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(35354),
                  Player2 = _context.Players.Find(33188),
                  Score = "60 60",
                  Winner = _context.Players.Find(33188),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(32007),
                  Player2 = _context.Players.Find(33600),
                  Score = "62 60",
                  Winner = _context.Players.Find(32007),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37795),
                  Player2 = _context.Players.Find(35398),
                  Score = "63 64",
                  Winner = _context.Players.Find(35398),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(31798),
                  Player2 = _context.Players.Find(38745),
                  Score = "63 62",
                  Winner = _context.Players.Find(38745),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37666),
                  Player2 = _context.Players.Find(32133),
                  Score = "61 63",
                  Winner = _context.Players.Find(32133),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(37818),
                  Player2 = _context.Players.Find(33330),
                  Score = "62 61",
                  Winner = _context.Players.Find(33330),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(31164),
                  Player2 = null, //Мартышков
                  Score = "60 60",
                  Winner = null, //Мартышков
                },
              }
            },

            // 1/4
            new Round()
            {
              Stage = 4,
              Type = "Winnerbracket",
              Matches = new List<Match>()
              {
                new Match()
                {
                  Player1 = _context.Players.Find(31157),
                  Player2 = _context.Players.Find(33188),
                  Score = "60 62",
                  Winner = _context.Players.Find(31157),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(30027),
                  Player2 = _context.Players.Find(35398),
                  Score = "61 62",
                  Winner = _context.Players.Find(30027),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(38745),
                  Player2 = _context.Players.Find(32133),
                  Score = "61 61",
                  Winner = _context.Players.Find(32133),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(33330),
                  Player2 = null, //Мартышков
                  Score = "61 61",
                  Winner = null, //Мартышков
                },
              }
            },

            // 1/2
            new Round()
            {
              Stage = 2,
              Type = "Winnerbracket",
              Matches = new List<Match>()
              {
                new Match()
                {
                  Player1 = _context.Players.Find(32007),
                  Player2 = _context.Players.Find(31157),
                  Score = "26 76(5) 60",
                  Winner = _context.Players.Find(32007),
                },
                new Match()
                {
                  Player1 = _context.Players.Find(32133),
                  Player2 = null, //Мартышков
                  Score = "63 63",
                  Winner = null, //Мартышков
                },
              }
            },

            // Финал (1/1)
            new Round()
            {
              Stage = 1,
              Type = "Final",
              Matches = new List<Match>()
              {
                new Match()
                {
                  Player1 = _context.Players.Find(32007),
                  Player2 = null, //Мартышков
                  Score = "64 46 63",
                  Winner = _context.Players.Find(32007),
                  PlaceInRound = 0,
                }
              }
            }
          }
        },
      };

      foreach (var bracket in result)
      {
        foreach (var round in bracket.Rounds)
          for (var i = 0; i < round.Matches.Count; i++)
            round.Matches[i].PlaceInRound = i;

        bracket.Rounds = bracket.Rounds.OrderBy(x => x.Stage).ToList();
      }

      return result;
    }
  }
}
