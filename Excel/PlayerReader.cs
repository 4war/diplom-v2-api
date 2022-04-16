using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Api.Rtt.Models.Entities;
using ExcelDataReader;
using MoreLinq;
using NUnit.Framework;

namespace Excel
{
  public class PlayerReader
  {
    public Dictionary<int, Player> Copy()
    {
      var directory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      var path = Path.Combine(directory, "Players.xlsx");
      var expectedPath = @"C:\Users\fedro\Рабочий стол\Диплом\Dikobraz\Api.Rtt\Excel\Players.xlsx";
      Assert.AreEqual(expectedPath, path);
      return Copy(path);
    }

    public Dictionary<int, Player> Copy(string path)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      var result = new Dictionary<int, Player>();

      //todo: open
      using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
      var reader = ExcelReaderFactory.CreateReader(stream);
      var dataSet = reader.AsDataSet();
      var table = dataSet.Tables[0];

      var gender = 0;

      for (var i = 4; i < table.Rows.Count; i++)
      {
        var row = table.Rows[i];
        var fio = row[1].ToString().Split();
        var dobSplit = row[3].ToString().Split().First().Split('.');
        var dobValue = new DateTime(int.Parse(dobSplit[2]), int.Parse(dobSplit[1]), int.Parse(dobSplit[0]));
        var rni = int.Parse(row[2].ToString());

        var player = new Player()
        {
          Rni = rni,
          Point = int.Parse(row[7].ToString()),
          City = row[4].ToString(),
          DateOfBirth = dobValue,
          Gender = 0,
          Surname = fio[0],
          Name = fio[1],
          Patronymic = fio[2],
        };

        result[rni] = player;
      }

      return result;
    }
  }
}
