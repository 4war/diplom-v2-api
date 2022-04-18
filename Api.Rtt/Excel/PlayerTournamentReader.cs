using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.ManyWithMany;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Api.Rtt.Excel
{
  public class PlayerTournamentReader : IReader<PlayerTournament>
  {
    private ApiContext _context;

    public PlayerTournamentReader(ApiContext context)
    {
      _context = context;
    }

    public List<PlayerTournament> Copy()
    {
      var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
      var tournamentDirectory = Path.Combine(projectDirectory, "Excel", "Tournaments");
      Assert.True(Directory.Exists(tournamentDirectory));

      var result = new List<PlayerTournament>();
      var directories = Directory.GetDirectories(tournamentDirectory);
      foreach (var directory in directories)
      {
        var files = Directory.GetFiles(directory).Where(x => x.Split(".").Last().ToLower() == "xlsx");
        foreach (var file in files)
        {
          result = result.Concat(Copy(file)).ToList();
        }
      }

      return result;
    }

    public List<PlayerTournament> Copy(string path)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      var result = new List<PlayerTournament>();

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

      var tournamentId = FindTournament(tournamentName,
        dateStart, city, category, age, gender);

      if (!tournamentId.HasValue)
      {
        throw new NotImplementedException();
      }

      for (var i = 14;; i++)
      {
        var row = table.Rows[i].ItemArray;

        if (row[1] is DBNull || row[1] is null || row[1]?.ToString()?.Length == 0)
          break;

        if (!int.TryParse(row[4]?.ToString(), out var rni))
          continue;

        result.Add(new PlayerTournament() { Id = tournamentId.Value, Rni = rni });
      }

      return result;
    }

    private int? FindTournament(string tournamentName,
      string dateStart, string city, string category, string age, string gender)
    {
      var split = dateStart.Split(".").Select(int.Parse).ToList();
      var date = new DateTime(split[2], split[1], split[0]);
      var genderInt = gender.StartsWith("Ж") ? 1 : 0;
      var ageInt = Ages.Dictionary.FirstOrDefault(x
        => string.Equals(x.Value.ViewValue, age, StringComparison.CurrentCultureIgnoreCase)).Key;

      if (!string.IsNullOrEmpty(tournamentName))
      {
        var tournament = _context.Tournaments
          .Where(x => x.DateStart > date.AddDays(-7) && x.DateStart < date.AddDays(7))
          .Where(x =>
            string.Equals(x.Name, tournamentName, StringComparison.CurrentCultureIgnoreCase))
          .Where(x => x.Category == category)
          .Where(x => x.Age == ageInt)
          .FirstOrDefault(x => x.Gender == genderInt);

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
