using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Api.Rtt.Models.Entities;
using ExcelDataReader;
using NUnit.Framework;

namespace Api.Rtt.Excel
{
  public class PlayerReader : IReader<Player>
  {
    public List<Player> Copy()
    {
      var directory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
      var path = Path.Combine(directory, "Excel", "PlayersRussia.xlsx");
      return Copy(path);
    }

    public List<Player> Copy(string path)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      var result = new Dictionary<int, Player>();

      using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
      var reader = ExcelReaderFactory.CreateReader(stream);
      var dataSet = reader.AsDataSet();
      var table = dataSet.Tables[0];

      var gender = 0;

      for (var i = 4; i < table.Rows.Count; i++)
      {
        var row = table.Rows[i];
        var fio = row[1]?.ToString()?.Split();
        var dobSplit = row[3]?.ToString()?.Split().First().Split('.');
        var dobValue = new DateTime(int.Parse(dobSplit?[2] ?? string.Empty),
          int.Parse(dobSplit?[1] ?? string.Empty),
          int.Parse(dobSplit?[0] ?? string.Empty));
        var rni = int.Parse(row[2]?.ToString() ?? string.Empty);

        var player = new Player()
        {
          Rni = rni,
          Point = int.Parse(row[7]?.ToString() ?? string.Empty),
          City = row[4].ToString(),
          DateOfBirth = dobValue,
          Gender = 0,
          Surname = fio?[0],
          Name = fio?.Length >= 2 ? fio[1] : "",
          Patronymic = fio?.Length >= 3 ? fio[2] : "",
        };

        result[rni] = player;
      }

      return result.Values.ToList();
    }
  }
}
