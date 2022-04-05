using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Rtt.Models.Seeds
{
  public class TournamentFactorySeed
  {
    private ApiContext _context;

    public TournamentFactorySeed(ApiContext context)
    {
      _context = context;
    }

    public List<TournamentFactory> GetList()
    {
      var factoryList = new List<TournamentFactory>();

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV A",
        Name = "САМАРСКИЙ КУБОК",
        TennisCenter = _context.TennisCenters.First(x => x.Name == "ДЮСШ №1"),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2022, 1, 3),
        Ages = new List<int>() { 14 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV В",
        Name = "Зимний Кубок ФТСО",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 4),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 1, 6),
        Ages = new List<int>() { 12, 16 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV В",
        Name = "Зимний Кубок МОО ФТ г. о. Самара",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 1, 12),
        Ages = new List<int>() { 12 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV В",
        Name = "Турнир на призы Tecnifibre",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 1, 19),
        Ages = new List<int>() { 14 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "III В",
        Name = "Первенство Самарской области",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 4),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 1, 25),
        Ages = new List<int>() { 18 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "III В",
        Name = "Первенство Самарской области",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 4),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 2, 8),
        Ages = new List<int>() { 12 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "III В",
        Name = "Первенство Самарской области",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 2, 23),
        Ages = new List<int>() { 14 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "III В",
        Name = "Первенство Самарской области",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 4),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 3, 22),
        Ages = new List<int>() { 16 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV В",
        Name = "Весенний кубок МОО ФТ г.о.Самара",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 3, 22),
        Ages = new List<int>() { 12 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "III В",
        Name = "Первенство Самарской области",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 5, 4),
        Ages = new List<int>() { 12 },
        HasQualification = true,
      });

      factoryList.Add(new TournamentFactory()
      {
        Category = "IV А",
        Name = "Мемориал А.Япрынцева",
        TennisCenter = _context.TennisCenters.First(x => x.Id == 1),
        NumberOfQualificationWinners = 4,
        DateStart = new DateTime(2021, 5, 11),
        Ages = new List<int>() { 16 },
        HasQualification = true,
      });

      return factoryList;
    }
  }
}

