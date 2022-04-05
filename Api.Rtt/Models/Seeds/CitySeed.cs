using System.Collections.Generic;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class CitySeed
  {
    private ApiContext _context;

    public CitySeed(ApiContext context)
    {
      _context = context;
    }

    public List<City> GetList()
    {
      return new List<City>()
      {
        new City() { Name = "Самара" },
        new City() { Name = "Тольятти" },
      };
    }
  }
}
