using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class WinPredictController : Controller
  {
    private readonly List<int> _timeRanges = new List<int>()
    {
      8, 10, 12, 14, 16, 18, 20, 22, 24,
    };

    private readonly ApiContext _context;

    public WinPredictController(ApiContext context)
    {
      _context = context;
    }

    [HttpPost("fromMatch")]
    public IActionResult Predict([FromBody] Match match)
    {
      var contextMatch = _context.Matches.FirstOrDefault(x => x.Id == match.Id);
      if (contextMatch?.Player1Rni is null || contextMatch.Player2Rni is null) return NotFound();

      return Ok(GetPrediction(contextMatch.Player1, contextMatch.Player2, contextMatch));
    }

    [HttpPost]
    public IActionResult Predict([FromBody] Prediction prediction)
    {
      var contextSelf = _context.Players.FirstOrDefault(x => x.Rni == prediction.Self.Rni);
      var contextEnemy = _context.Players.FirstOrDefault(x => x.Rni == prediction.Enemy.Rni);

      var newMatch = new Match()
      {
        Player1 = contextSelf,
        Player2 = contextEnemy,
        Court = new Court() { Surface = "hard" },
        Start = DateTime.Now.AddYears(-1),
      };

      var result = GetPrediction(contextSelf, contextEnemy, newMatch);
      return Ok(result);
    }

    private Prediction GetPrediction(Player self, Player enemy, Match match)
    {
      const int kf = 4;
      const int kpr = 3;
      const int kc = 3;
      const int kps = 2;

      var f = (GetFormFactorGroup(self, enemy) * kf
                 * GetPersonalFactorGroup(self, enemy) * kpr
                 * GetConditionFactorGroup(self, enemy, match) * kc
                 * GetPsychFactorGroup(self, enemy) * kps) / (kf + kpr + kc + kps)
              * Math.Pow(self.Point / (double)enemy.Point, 0.25);

      var result = f / (f + 1);
      return new Prediction()
      {
        Self = self,
        Enemy = enemy,
        Win = (int)(Math.Round(result * 100))
      };
    }

    private double GetFormFactorGroup(Player self, Player enemy)
    {
      var selfEfficientFactor = GetEfficiencyFactor(self);
      var enemyEfficientFactor = GetEfficiencyFactor(enemy);
      var selfDiversity = GetDiversityFactor(self);
      var enemyDiversity = GetDiversityFactor(enemy);

      return Math.Sqrt(selfEfficientFactor / enemyEfficientFactor)
             * GetCountFactor(self, enemy)
             * Math.Sqrt(selfDiversity / enemyDiversity);
    }

    private double GetPersonalFactorGroup(Player self, Player enemy)
    {
      var matches = _context.Matches
        .Where(x =>
          (x.Player1Rni == self.Rni || x.Player2Rni == self.Rni) &&
          (x.Player1Rni == enemy.Rni || x.Player2Rni == enemy.Rni)).ToList();

      var selfEfficiency = GetEfficiencyFactorFromMatches(self, matches);
      var enemyEfficiency = GetEfficiencyFactorFromMatches(enemy, matches);
      return Math.Sqrt(selfEfficiency / enemyEfficiency);
    }

    private double GetConditionFactorGroup(Player self, Player enemy, Match match)
    {
      var selfSurfaceCount = GetSurfaceMatchesFactorCount(self, match.Court.Surface);
      var enemySurfaceCount = GetSurfaceMatchesFactorCount(enemy, match.Court.Surface);
      var fSurface = 1 / (1 + Math.Exp((enemySurfaceCount - selfSurfaceCount) / (double)20)) + 0.5;

      var selfTimeCount = (match.Start == null) ? 1 : GetDaytimeMatchesFactorCount(self, match.Start.Value);
      var enemyTimeCount = (match.Start == null) ? 1 : GetDaytimeMatchesFactorCount(enemy, match.Start.Value);
      var fCount = 1 / (1 + Math.Exp((enemyTimeCount - selfTimeCount) / (double)20)) + 0.5;

      var selfFatigue = GetFatigueFactor(self, match);
      var enemyFatigue = GetFatigueFactor(enemy, match);
      var fFatigue = Math.Sqrt(enemyFatigue / selfFatigue);

      return fSurface * fCount * fFatigue;
    }

    private double GetPsychFactorGroup(Player self, Player enemy)
    {
      if (!self.TestResults.Any() || !enemy.TestResults.Any()) return 1d;
      return GetTestFactor(self, enemy) * GetStyleFactor(self, enemy);
    }

    private double GetTestFactor(Player self, Player enemy)
    {
      var selfMoral = self.TestResults.ToList().Last().Moral;
      var enemyMoral = enemy.TestResults.ToList().Last().Moral;
      return Math.Sqrt(selfMoral / (double)enemyMoral);
    }

    private double GetStyleFactor(Player self, Player enemy)
    {
      var selfResult = self.TestResults.ToList().Last();
      var selfStyle = new List<int> { selfResult.Defensive, selfResult.Active, selfResult.Reactive };
      var enemyResult = enemy.TestResults.ToList().Last();
      var enemyStyle = new List<int> { enemyResult.Defensive, enemyResult.Active, enemyResult.Reactive };

      var selfTotal = GetTotalStyle(selfStyle, enemyStyle);
      var enemyTotal = GetTotalStyle(enemyStyle, selfStyle);

      return selfTotal / enemyTotal;
    }

    private double GetTotalStyle(List<int> selfStyle, List<int> enemyStyle)
    {
      var weights = new List<double> { 0.8, 1, 1.25 };

      var y = 2;
      var total = 0d;
      for (var i = 0; i < 3; i++)
      {
        var sum = 0d;
        var currentY = (y + i) % 3;
        var x = currentY;
        for (var w = 0; w < 3; w++)
        {
          var currentX = (x + w) % 3;
          sum += selfStyle[currentX] * weights[w];
        }

        total += sum * enemyStyle[i];
      }

      return total / (enemyStyle.Sum());
    }

    private int GetSurfaceMatchesFactorCount(Player player, string surface)
    {
      return _context.Matches
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.CourtId.HasValue)
        .Count(x => x.Court.Surface == surface);
    }

    private int GetDaytimeMatchesFactorCount(Player player, DateTime start)
    {
      var range = start.Hour < _timeRanges[0] ? _timeRanges[0] : _timeRanges.TakeWhile(x => x < start.Hour).Max();

      var matches = _context.Matches
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
        .ToList()
        .Count(x => x.Start.HasValue && x.Start.Value.Hour >= range && x.Start.Value.Hour < range + 2);
      return matches;
    }

    private double GetFatigueFactor(Player player, Match match)
    {
      if (!match.Start.HasValue || match.Round?.Bracket == null) return 1d;

      var games = _context.Matches.Where(x => x.Round.BracketId == match.Round.BracketId)
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
        .ToList();
      var result = games
        .Where(x => x.Start.HasValue && x.End.HasValue)
        .Select(x => new
        {
          Duration = (x.End.Value - x.Start.Value).TotalMinutes,
          TimePassed = (match.Start.Value - x.End.Value).TotalHours,
        }).ToList();

      return result.Select(x => x.Duration * Math.Exp(-0.01 * x.TimePassed)).Sum();
    }

    private double GetEfficiencyFactor(Player player)
    {
      var selfMatches = _context.Matches
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.WinnerRni.HasValue).ToList();

      return GetEfficiencyFactorFromMatches(player, selfMatches);
    }

    private double GetEfficiencyFactorFromMatches(Player player, List<Match> selfMatches)
    {
      var fResArray = new List<double>();
      var fTimeArray = new List<double>();
      foreach (var match in selfMatches.ToList())
      {
        if (!match.Start.HasValue) continue;
        var selfInMatch = match.Player1Rni == player.Rni ? match.Player1 : match.Player2;
        var enemyInMatch = match.Player1Rni == player.Rni ? match.Player2 : match.Player1;
        var days = (DateTime.Now - match.Start.Value).TotalDays;
        fResArray.Add((double)enemyInMatch.Point / selfInMatch.Point * CalculateScore(match, player));
        fTimeArray.Add((double)Math.Exp(-0.01 * days));
      }

      var fEf = fResArray.Zip(fTimeArray, (fRes, fTime) => fRes * fTime).Sum()
                / fTimeArray.Sum();

      return fEf;
    }

    private double GetCountFactor(Player self, Player enemy)
    {
      var selfMatchCount = _context.Matches
        .Where(x => x.Player1Rni == self.Rni || x.Player2Rni == self.Rni)
        .Count(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.WinnerRni.HasValue);

      var enemyMatchCount = _context.Matches
        .Where(x => x.Player1Rni == enemy.Rni || x.Player2Rni == enemy.Rni)
        .Count(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.WinnerRni.HasValue);

      var difference = selfMatchCount - enemyMatchCount;
      return 1 / (1 + (double)Math.Exp(-difference / (double)20)) + 0.5;
    }

    private double GetDiversityFactor(Player player)
    {
      var playerUniqueMatches = _context.Matches
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
        .Select(x => x.Player1Rni == player.Rni ? x.Player2Rni.Value : x.Player1Rni.Value)
        .GroupBy(x => x).Select(x => x.Key).ToList();

      var playerDiversity = (double)playerUniqueMatches.Average();

      var tcUniqueMatches = _context.Matches
        .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
        .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
        .Select(x => x.Round.Bracket.Tournament.Factory.TennisCenterId)
        .GroupBy(x => x).Select(x => x.Key).ToList();

      var techniqueDiversity = (double)tcUniqueMatches.Average();
      return playerDiversity + techniqueDiversity;
    }

    private double CalculateScore(Match match, Player player)
    {
      var self = player.Rni == match.Player1Rni ? match.Player1 : match.Player2;
      var score = match.Score;
      if (self.Rni != match.WinnerRni) score = ReverseScore(score);
      var sets = score.Split();

      var winCounter = 0;
      var loseCounter = 0;
      var setCounter = 0;
      foreach (var set in sets)
      {
        var selfGame = int.Parse(set[0].ToString());
        var enemyGame = int.Parse(set[1].ToString());
        winCounter += selfGame;
        loseCounter += enemyGame;

        if (winCounter > loseCounter)
        {
          winCounter += 3;
          setCounter += 1;
        }
        else
        {
          loseCounter += 3;
          setCounter -= 1;
        }
      }

      if (setCounter > 0) winCounter += 4;
      else loseCounter += 4;

      return (double)winCounter / (winCounter + loseCounter) + 0.5;
    }

    private string ReverseScore(string score)
    {
      return string.Join(" ", score.Split().Select(x => string.Join("", new List<char> { x[1], x[0] })));
    }
  }

  public class Prediction
  {
    public Player Self { get; set; }
    public Player Enemy { get; set; }
    public int Win { get; set; }
  }
}
