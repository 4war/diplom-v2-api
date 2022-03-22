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
                new() { Name = "ДЮСШ №1", Address = "ул. Ново-Садовая, 32А, Самара, Самарская обл., 443110", },
                new() { Name = "Тригон", Address = "ул. Дачная, 4А, Самара, Самарская обл., 443096", },
                new() { Name = "СДЮСШОР по теннису", Address = "Шушенская ул., Самара, Самарская обл., 443011", },
                new() { Name = "Тольятти Теннис Центр", Address = "ул. Баныкина, 19А, Тольятти, Самарская обл., 445021", },
                new() { Name = "Davis", Address = "ул. Спортивная, 19, Тольятти, Самарская обл., 445057", },
            };

            foreach (var center in list)
                _context.TennisCenters.Add(center);
            
            _context.SaveChanges();
        }

        private void SeedTournaments()
        {
            var factoryList = new List<TournamentFactory>();
            var newFactory = new TournamentFactory()
            {
                Ages = new List<int>() { 14 },
                Category = "IV A",
                Name = "САМАРСКИЙ КУБОК",
                HasQualification = true,
                TennisCenter = _context.TennisCenters.First(x => x.Name == "ДЮСШ №1"),
                NumberOfQualificationWinners = 4,
                DateStart = new DateTime(2022, 1, 3),
            };
            newFactory.SetDate();
            factoryList.Add(newFactory);

            foreach (var factory in factoryList)
            {
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