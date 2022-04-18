using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.ManyWithMany;
using ExcelDataReader;
using NUnit.Framework;

namespace Api.Rtt.Excel
{
  public class PlayerTournamentReader : IReader<PlayerTournament>
  {
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

      var name = table.Rows[0].ItemArray.First()?.ToString();
      var dateString = table.Rows[1].ItemArray.Last(x => x.ToString().Length > 0).ToString()?.Split().Where(x => x.Contains("."));
      var dateStart = dateString.First();
      var category = table.Rows[3].ItemArray.Last(x => x.ToString().Length > 0)?.ToString();
      var age = table.Rows[4].ItemArray.Last(x => x.ToString().Length > 0)?.ToString();
      var gender = table.Rows[5].ItemArray.Last(x => x.ToString().Length > 0)?.ToString();
      var netRange = table.Rows[6].ItemArray.Last(x => x.ToString().Length > 0)?.ToString();

      var message = table.Rows[11].ItemArray;
//todo: Add automatically players to seed
      throw new NotImplementedException();
    }
  }
}
