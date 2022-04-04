using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
    public class DataSeed
    {
        private ApiContext _context;
        
        public DataSeed(ApiContext apiContext)
        {
            _context = apiContext;
        }

        public void SeedData()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            if (!_context.TennisCenters.Any())
            {
                SeedTennisCenters();
            }

            if (!_context.Tournaments.Any())
            {
                SeedTournaments();
            }
        }

        private void SeedTennisCenters()
        {
            var list = new List<TennisCenter>()
            {
                new() { Name = "ДЮСШ №1", Address = "ул. Ново-Садовая, 32А, Самара, Самарская обл., 443110", City = "Самара"},
                new() { Name = "Тригон", Address = "ул. Дачная, 4А, Самара, Самарская обл., 443096", City = "Самара"},
                new() { Name = "СДЮСШОР по теннису", Address = "Шушенская ул., Самара, Самарская обл., 443011", City = "Самара"},
                new() { Name = "Тольятти Теннис Центр", Address = "ул. Баныкина, 19А, Тольятти, Самарская обл., 445021", City = "Тольятти"},
                new() { Name = "Davis", Address = "ул. Спортивная, 19, Тольятти, Самарская обл., 445057",City = "Тольятти" },
            };

            foreach (var center in list)
                _context.TennisCenters.Add(center);
            
            _context.SaveChanges();
        }

        private void SeedTournaments()
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

            foreach (var factory in factoryList)
            {
                factory.SetDate();
                var result = factory.Generate();
                foreach (var tournament in result)
                {
                    _context.Tournaments.Add(tournament);
                }
            }

            _context.SaveChanges();
        }
    }
}