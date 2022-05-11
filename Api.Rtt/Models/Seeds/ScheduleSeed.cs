using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Models.Seeds
{
  public class ScheduleSeed
  {
    private ApiContext _context;

    public ScheduleSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Schedule> GetList()
    {
      var factory = _context.TournamentFactories.First(tf => tf.Id == 10);
      var list = new List<Schedule>();
      for (var i = 0; i < 5; i++)
      {
        var day = factory.Tournaments.First(x => x.Stage == (int)Stage.Main).DateStart.AddDays(i);
        var schedule = GetDaySchedule(day, factory, i);
        list.Add(schedule);
      }

      return list;
    }

    private Schedule GetDaySchedule(DateTime day, TournamentFactory factory, int dayNumber)
    {
      var schedule = new Schedule
      {
        Day = day,
        Factory = factory
      };

      var courts = _context.TennisCenters
        .First(x => x.Name == "Тольятти Теннис Центр")
        .Courts.Where(c => !c.Opened && c.Surface == "hard").ToList();

      foreach (var tournament in factory.Tournaments.Where(x => x.Stage == (int)Stage.Main))
      {
        var bracket = _context.Brackets.First(x => x.TournamentId == tournament.Id);
        var stage = (int)Math.Pow(2, 4 - dayNumber);
        var round = bracket.Rounds.First(x => x.Stage == stage);

        var courtCounter = 0;
        var order = 1;

        foreach (var match in round.Matches.Where(x => x.Player1 is not null && x.Player2 is not null))
        {
          match.Court = courts[courtCounter];
          match.OrderInSchedule = order;
          courtCounter++;
          if (courtCounter >= courts.Count)
          {
            order++;
            courtCounter = 0;
          }

          schedule.Matches.Add(match);
        }
      }

      return schedule;
    }
  }
}
