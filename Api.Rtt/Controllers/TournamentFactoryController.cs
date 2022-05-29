using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class TournamentFactoryController : Controller
  {
    private readonly ApiContext _context;
    private readonly BracketBuilder _bracketBuilder = new();

    public TournamentFactoryController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
      return Ok(_context.TournamentFactories.OrderBy(x => x.DateStart));
    }

    [HttpGet("future")]
    public IActionResult GetFuture()
    {
      var list = _context.TournamentFactories.ToList();
      return Ok(list.Where(x => x.DateStart > DateTime.Now)
        .OrderBy(x => x.DateStart).Take(5));
    }

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
      var factory = _context.TournamentFactories.FirstOrDefault(x => x.Id == id);
      if (factory is null)
      {
        return NotFound();
      }

      factory.Tournaments = factory.Tournaments.OrderBy(x => x.Gender)
        .ThenBy(x => x.Age)
        .ThenBy(x => x.Stage)
        .ToList();
      return Ok(factory);
    }

    [HttpGet("fromTournament/{id:int}")]
    public IActionResult GetFromTournament([FromRoute] int id)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null)
      {
        return NotFound();
      }

      var factory = _context.TournamentFactories.FirstOrDefault(x => x.Id == tournament.Factory.Id);
      if (factory is null)
      {
        return NotFound();
      }

      factory.Tournaments = factory.Tournaments.OrderBy(x => x.Gender)
        .ThenBy(x => x.Age)
        .ThenBy(x => x.Stage)
        .ToList();
      return Ok(factory);
    }


    [HttpPost]
    [Authorize(Roles = "org")]
    public IActionResult Post([FromBody] TournamentFactory factory)
    {
      if (factory is null) return BadRequest();

     var list = factory.Generate().ToList();

      foreach (var t in list)
      {
        t.Factory = factory;
        _context.Tournaments.Add(t);
      }

      _context.TennisCenters.Attach(factory.TennisCenter);
      foreach (var court in factory.TennisCenter.Courts)
        _context.Courts.Attach(court);

      _context.TournamentFactories.Add(factory);
      _context.SaveChanges();

      var bracketList = _bracketBuilder.CreateBracketsForFactory(factory);
      foreach (var bracket in bracketList)
        _context.Brackets.Add(bracket);

      _context.SaveChanges();

      return Ok(factory);
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "org")]
    public IActionResult Delete([FromRoute] int id)
    {
      var factory = _context.TournamentFactories.FirstOrDefault(x => x.Id == id);
      if (factory is null)return NotFound();

      foreach (var tournament in factory.Tournaments.ToList())
        _context.Tournaments.Remove(tournament);

      _context.TournamentFactories.Remove(factory);

      _context.SaveChanges();
      return Ok(factory);
    }
  }
}
