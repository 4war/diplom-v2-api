using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class ScheduleController : Controller
  {
    private readonly ApiContext _context;

    public ScheduleController(ApiContext context)
    {
      _context = context;
    }

    [HttpPost("{factoryId:int}")]
    public IActionResult Get([FromRoute] int factoryId, [FromBody] DateTime dateTime)
    {
      var schedule = _context.Schedules.FirstOrDefault(s => s.Day == dateTime && s.FactoryId == factoryId);
      if (schedule is null)
      {
        return NotFound();
      }

      return Ok(schedule);
    }

    [HttpPost("{factoryId:int}/courts")]
    public IActionResult GetCourts([FromRoute] int factoryId, [FromBody] DateTime dateTime)
    {
      var schedule = _context.Schedules.FirstOrDefault(s => s.Day == dateTime && s.FactoryId == factoryId);
      if (schedule is null)
      {
        return NotFound();
      }

      var courts = schedule.Matches.Select(x => x.Court).Distinct().OrderBy(x => x.Name);
      return Ok(courts);
    }

    [HttpPost("{factoryId:int}/order")]
    public IActionResult GetOrder([FromRoute] int factoryId, [FromBody] DateTime dateTime)
    {
      var schedule = _context.Schedules.FirstOrDefault(s => s.Day == dateTime && s.FactoryId == factoryId);
      if (schedule is null)
      {
        return NotFound();
      }

      var maxOrder = schedule.Matches.Select(x => x.OrderInSchedule)
        .Where(x => x.HasValue)
        .Max(x => x.Value);
      var orders = Enumerable.Range(1, maxOrder);

      return Ok(orders);
    }

    [HttpGet("{factoryId:int}/days")]
    public IActionResult GetDays([FromRoute] int factoryId)
    {
      var schedule = _context.Schedules.Where(s => s.FactoryId == factoryId).ToList();
      if (!schedule.Any())
      {
        return NotFound();
      }

      var days = schedule.Select(x => x.Day).OrderBy(x => x.Date);
      return Ok(days);
    }

    [HttpPost("{factoryId:int}/missing")]
    public IActionResult GetNotScheduledMatches([FromRoute] int factoryId, [FromBody] DateTime dateTime)
    {
      var factory = _context.TournamentFactories.FirstOrDefault(x => x.Id == factoryId);
      if (factory is null) return NotFound();

      var dayNumber = (int)(factory.DateStart - dateTime).TotalDays;
      var stage = (int)Math.Pow(2, 4 - dayNumber);
      var matchList = new List<Match>();
      foreach (var tournament in factory.Tournaments.Where(x => x.Stage == (int)Stage.Main))
      {
        var bracket = _context.Brackets.FirstOrDefault(x => x.TournamentId == tournament.Id);
        var round = bracket.Rounds.FirstOrDefault(x => x.Stage == stage);

        foreach (var match in round.Matches)
          if (match.Player1 is not null && match.Player2 is not null &&
              (match.Court is null || match.OrderInSchedule is null))
            matchList.Add(match);
      }

      return Ok(matchList);
    }

    [HttpPatch]
    [Authorize(Roles = "org")]
    public IActionResult UpdateScheduleMatches([FromBody] Schedule schedule)
    {
      if (schedule is null) return BadRequest();

      var contextSchedule = _context.Schedules
        .Include(x => x.Matches)
        .First(x => x.Id == schedule.Id);
      if (contextSchedule is null) return NotFound();

      var dictionary = contextSchedule.Matches.ToDictionary(x => x.Id, x => x);
      foreach (var match in schedule.Matches.ToList())
      {
        if (match.Player1 is not null) match.Player1Rni = match.Player1.Rni;
        if (match.Player2 is not null) match.Player2Rni = match.Player2.Rni;
        if (match.Winner is not null) match.WinnerRni = match.Winner.Rni;

        match.ScheduleId = schedule.Id;
        match.Schedule = schedule;

        var contextMatch = contextSchedule.Matches.FirstOrDefault(x => x.Id == match.Id);
        if (contextMatch == null)
        {
          contextSchedule.Matches.Add(match);
        }
        else
        {
          contextMatch.CourtId = match.Court.Id;
          contextMatch.OrderInSchedule = match.OrderInSchedule;
          contextMatch.ScheduleId = schedule.Id;
          dictionary.Remove(contextMatch.Id);
        }
      }

      foreach (var kvp in dictionary)
      {
        var contextMatch = contextSchedule.Matches.FirstOrDefault(x => x.Id == kvp.Key);
        contextMatch!.ScheduleId = null;
        contextMatch!.CourtId = null;
        contextMatch!.OrderInSchedule = null;
      }

      _context.SaveChanges();

      return Ok(schedule);
    }

    /// <summary>
    /// adds match is schedule
    /// </summary>
    /// <param name="factoryId"></param>
    /// <param name="day">starts with 1</param>
    /// <param name="match"></param>
    /// <returns></returns>
    [HttpPatch("{factoryId:int}/{day:int}/match")]
    [Authorize(Roles = "org")]
    public IActionResult UpdateMatch([FromRoute] int factoryId, [FromRoute] int day, [FromBody] Match match)
    {
      if (match is null) return BadRequest();

      var factory = _context.TournamentFactories.FirstOrDefault(x => x.Id == factoryId);
      if (factory is null) return NotFound();

      var dateTime = factory.DateStart.AddDays(day - 1);
      var schedule = _context.Schedules.FirstOrDefault(s => s.Day == dateTime && s.FactoryId == factoryId);
      if (schedule is null) return NotFound();

      schedule.Matches.Add(match);
      _context.SaveChanges();

      return Ok(match);
    }
  }
}
