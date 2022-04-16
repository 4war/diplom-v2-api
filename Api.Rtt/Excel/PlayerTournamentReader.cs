using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Api.Rtt.Models.Entities.ManyWithMany;

namespace Api.Rtt.Excel
{
  public class PlayerTournamentReader : IReader<PlayerTournament>
  {
    public List<PlayerTournament> Copy()
    {
      var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
      var tournamentDirectory = Path.Combine(projectDirectory, "Excel", "Tournaments");

      var result = new List<PlayerTournament>();
      var files = Directory.GetFiles(tournamentDirectory)
        .Where(x => x.Split(".").Last().ToLower() == "xlsx");

      foreach (var file in files)
      {
        result = result.Concat(Copy(file)).ToList();
      }

      return result;
    }

    public List<PlayerTournament> Copy(string path)
    {
      throw new System.NotImplementedException();
    }
  }
}
