using System.Collections.Generic;
using Api.Rtt.Excel;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class PlayerTournamentSeed
  {
    private ApiContext _context;

    public PlayerTournamentSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Tournament> GetList()
    {
      var playerTournamentReader = new PlayerTournamentReader(_context);
      var result = playerTournamentReader.Copy();
      return result;
    }
  }
}
