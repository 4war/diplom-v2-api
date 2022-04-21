using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Api.Rtt.Excel
{
  public class PlayerTournamentReader
  {
    private ApiContext _context;

    public PlayerTournamentReader(ApiContext context)
    {
      _context = context;
    }

    public List<Tournament> Copy()
    {
      var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
      var tournamentDirectory = Path.Combine(projectDirectory, "Excel", "Tournaments");
      Assert.True(Directory.Exists(tournamentDirectory));

      var result = new List<Tournament>();
      var directories = Directory.GetDirectories(tournamentDirectory);
      foreach (var directory in directories)
      {
        var files = Directory.GetFiles(directory).Where(x => x.Split(".").Last().ToLower() == "xlsx");
        foreach (var file in files)
          result = result.Concat(Copy(file)).ToList();
      }

      return result;
    }

    public List<Tournament> Copy(string path)
    {
      var result = new List<Tournament>();
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

      using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
      var reader = ExcelReaderFactory.CreateReader(stream);
      var dataSet = reader.AsDataSet();
      var table = dataSet.Tables[0];

      var tournamentName = table.Rows[0].ItemArray.First()?.ToString();
      var dateString = table.Rows[1].ItemArray.Last(x => x?.ToString()?.Length > 0).ToString()?.Split()
        .Where(x => x.Contains("."));
      var city = table.Rows[2].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString()?.Split().First();
      var dateStart = dateString?.First();
      var category = table.Rows[3].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString();
      var age = table.Rows[4].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString();
      var gender = table.Rows[5].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString();
      var netRange = table.Rows[6].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString();
      var genderInt = gender.StartsWith("Ж") ? 1 : 0;

      var stageString = table.Rows[11].ItemArray.Last(x => x?.ToString()?.Length > 0)?.ToString();
      var stageInt = GetStage(stageString);

      var tournamentId = FindTournament(tournamentName,
        dateStart, city, category, age, genderInt, stageInt);
      var tournament = _context.Tournaments.Find(tournamentId);

      if (!tournamentId.HasValue)
      {
        throw new NotImplementedException();
      }

      var i = 13;
      for (;; i++)
      {
        var row = table.Rows[i].ItemArray;
        var player = GetOrAddPlayer(row, genderInt);
        if (player is null) break;
        tournament.Players.Add(player);
      }

      result.Add(tournament);

      int? nextTournamentStage = null;
      for (var j = i;; i++)
      {
        var possibleStageString = table.Rows[i]?.ItemArray?.LastOrDefault(x => x?.ToString()?.Length > 0)?.ToString();

        if (string.IsNullOrEmpty(possibleStageString))
          continue;

        if (possibleStageString.Contains("отбор") ||
            possibleStageString.Contains("основ") ||
            possibleStageString.Contains("турнир"))
        {
          nextTournamentStage = GetStage(possibleStageString);
          break;
        }

        if (j > i + 5) break;
      }

      if (nextTournamentStage.HasValue)
      {
        var nextTournamentId = FindTournament(tournamentName, dateStart, city, category, age, genderInt,
          nextTournamentStage.Value);

        if (nextTournamentId.HasValue)
        {
          var nextTournament = _context.Tournaments.Find(nextTournamentId.Value);
          i += 2;
          for (;; i++)
          {
            var row = table.Rows[i].ItemArray;
            var player = GetOrAddPlayer(row, genderInt);
            if (player is null) break;
            nextTournament.Players.Add(player);
          }

          result.Add(nextTournament);
        }
      }


      return result;
    }

    private Player GetOrAddPlayer(object[] row, int genderInt)
    {
      if (row[1] is DBNull || row[1] is null || row[1]?.ToString()?.Length == 0)
        return null;

      if (!int.TryParse(row[4]?.ToString(), out var rni))
        return null;

      return _context.Players.Find(rni) ?? AddPlayerIfMissing(row, genderInt);
    }

    private Player AddPlayerIfMissing(object[] itemArray, int genderInt)
    {
      if (!int.TryParse(itemArray[4].ToString(), out var rni))
        return null;

      var fioSplit = itemArray[3]?.ToString()?.Split();
      var surname = fioSplit?.First();
      var name = fioSplit?.Length > 1 ? fioSplit[1] : string.Empty;
      var patronymic = fioSplit?.Length > 2 ? fioSplit[2] : string.Empty;

      var dobString = itemArray[5]?.ToString()?.Split(".");
      var dob = new DateTime(int.Parse(dobString?[2].Split().First()!), int.Parse(dobString?[1]!),
        int.Parse(dobString?[0]!));

      var city = itemArray[6]?.ToString();
      if (!int.TryParse(itemArray[2].ToString(), out var points))
        points = 0;

      var player = new Player()
      {
        Rni = rni,
        Surname = surname,
        Name = name,
        Patronymic = patronymic,
        Gender = genderInt,
        DateOfBirth = dob,
        City = city,
        Point = points,
      };

      _context.Players.Add(player);
      // _context.SaveChanges();
      return player;
    }

    private int GetStage(string stageString)
    {
      return stageString.ToLower().Contains("основ")
        ? (int)Stage.Main
        : stageString.ToLower().Contains("отбор")
          ? (int)Stage.Qual
          : -1;
    }

    private int? FindTournament(string tournamentName,
      string dateStart, string city, string category, string age, int genderInt, int stageInt)
    {
      if (stageInt is < 0 or > 2)
        throw new ArgumentException("Стадия турнира не определена");

      var split = dateStart.Split(".").Select(int.Parse).ToList();
      var date = new DateTime(split[2], split[1], split[0]);

      var ageInt = Ages.Dictionary.FirstOrDefault(x
        => string.Equals(x.Value.ViewValue, age, StringComparison.CurrentCultureIgnoreCase)).Key;

      if (!string.IsNullOrEmpty(tournamentName))
      {
        var dataBaseTournamentName = tournamentName.GetMostSimilar(_context.Tournaments.Select(x => x.Name).Distinct());

        var list = _context.Tournaments.ToList();

        var tournament = list.Where(
          x => string.Equals(x.Name, dataBaseTournamentName.Value, StringComparison.CurrentCultureIgnoreCase)
               && x.DateStart > date.AddDays(-7) && x.DateStart < date.AddDays(7)
               && x.Category.Replace(" ", "") == category
               && x.Stage == stageInt
               && x.Age == ageInt).FirstOrDefault(x => x.Gender == genderInt);

        if (tournament is null)
          return null;

        return tournament.Id;
      }

      var cityInContext = _context.Cities.Find(city);
      if (cityInContext is null)
      {
        _context.Cities.Add(new City() { Name = city });
      }

      if (!string.IsNullOrEmpty(city))
      {
        var tournament = _context.Tournaments
          .Where(x => x.TennisCenter.City.ToLower() == city.ToLower())
          .Where(x => x.Category == category)
          .Where(x => x.DateStart > date.AddDays(-7) && x.DateStart < date.AddDays(7))
          .Where(x => x.Gender == genderInt)
          .FirstOrDefault(x => x.Age == ageInt);

        if (tournament is null)
          return null;

        return tournament.Id;
      }

      return null;
    }
  }
}
