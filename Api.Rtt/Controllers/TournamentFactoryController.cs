using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Seeds;
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

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
      var factory = _context.TournamentFactories.FirstOrDefault(x => x.FirstTournamentId == id);
      if (factory is null)
      {
        return NotFound();
      }

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

      var factory = _context.TournamentFactories.FirstOrDefault(x => x.FirstTournamentId == tournament.Factory.FirstTournamentId);
      if (factory is null)
      {
        return NotFound();
      }

      return Ok(factory);
    }


    // private List<Tournament> GetFactoryAsList(string name, bool mainOnly = false)
    // {
    //   var list = _context.Tournaments
    //     .Where(x => x.Name == name)
    //     .Where(x => !mainOnly || x.Stage == (int)Stage.Main)
    //     .OrderBy(x => x.Gender)
    //     .ThenByDescending(x => x.Age)
    //     .ToList();
    //
    //   return list;
    // }
    //
    // private TournamentFactory GetTournamentFactoryFromList(List<Tournament> notFilteredList,
    //   Tournament tournament)
    // {
    //   var list = notFilteredList.Where(x =>
    //     x.DateStart.AddDays(-7) < tournament.DateStart && x.DateStart.AddDays(7) > tournament.DateStart).ToList();
    //
    //   var factory = new TournamentFactory()
    //   {
    //     FirstTournamentId = tournament.Id,
    //     Name = tournament.Name,
    //     Category = tournament.Category,
    //     Ages = list.Select(x => x.Age).Distinct().ToList(),
    //     Genders = list.Select(x => x.Gender).Distinct().ToList(),
    //     DateStart = list.First(x => x.Stage == (int)Stage.Main).DateStart,
    //     DateEnd = list.First(x =>  x.Stage == (int)Stage.Main).DateEnd,
    //     DateRequest = list.First(x =>  x.Stage == (int)Stage.Main).DateRequest,
    //     HasQualification = list.Any(x =>  x.Stage == (int)Stage.Main),
    //     NetRange = list.First(x =>  x.Stage == (int)Stage.Main).NetRange,
    //     TennisCenter = tournament.TennisCenter,
    //     NumberOfQualificationWinners = list.First(x =>  x.Stage == (int)Stage.Main).NumberOfQualificationWinners,
    //     Tournaments = list,
    //   };
    //
    //   return factory;
    // }


    [HttpPost]
    public IActionResult Post([FromBody] TournamentFactory factory)
    {
      if (factory is null)
      {
        return BadRequest();
      }

      var list = factory.Generate();
      foreach (var t in list)
      {
        factory.Tournaments.Add(t);
      }

      _context.TournamentFactories.Add(factory);

      var bracketList = _bracketBuilder.CreateBracketsForFactory(factory);
      foreach (var bracket in bracketList)
      {
        _context.Brackets.Add(bracket);
      }
      _context.SaveChanges();

      return Ok(factory);
    }


    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
      var factory = _context.TournamentFactories.FirstOrDefault(x => x.FirstTournamentId == id);
      if (factory is null)
      {
        return NotFound();
      }

      _context.TournamentFactories.Remove(factory);

      // var list = GetFactoryAsList(tournament.Name, false);
      // var factory = GetTournamentFactoryFromList(list, tournament);
      // foreach (var t in factory.Tournaments.ToList())
      // {
      //   _context.Tournaments.Remove(t);
      // }

      _context.SaveChanges();
      return Ok(factory);
    }
  }
}
