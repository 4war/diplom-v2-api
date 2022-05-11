using System;
using System.Collections.Generic;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class MatchSeed
  {
    private readonly ApiContext _context;

    public MatchSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Match> GetList()
    {
      return new List<Match>();
    }
  }
}
