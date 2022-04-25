using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models
{
    public class TournamentFactory
    {
        public int FirstTournamentId { get; set; }
        public string Name { get; set; }
        public List<int> Ages { get; set; }
        public string Category { get; set; }
        public bool HasQualification { get; set; }
        public int NumberOfQualificationWinners { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateRequest { get; set; }
        public int NetRange { get; set; } = 32;
        public TennisCenter TennisCenter { get; set; }
        public List<int> Genders { get; set; } = new() { (int)Gender.Male, (int)Gender.Female };
        public List<Tournament> Tournaments { get; set; } = new List<Tournament>();

        public void SetDate()
        {
            DateEnd = DateStart.AddDays(Math.Log2(NetRange));
            DateRequest = DateStart.AddDays(-14);
        }

        public TournamentFactory(Tournament tournament)
        {
            Category = tournament.Category;
            Name = tournament.Name;
            TennisCenter = tournament.TennisCenter;
            NumberOfQualificationWinners = tournament.NetRange / 8;
            DateStart = tournament.DateStart;
        }

        public TournamentFactory()
        {
        }

        public List<Tournament> Generate()
        {
            var list = new List<Tournament>();

            foreach (var age in Ages)
            {
                foreach (var gender in Genders)
                {
                    var qualificationTournament = GenerateQualification(gender, age);
                    if (HasQualification)
                    {
                        list.Add(qualificationTournament);
                    }

                    var mainTournament = GenerateMain(gender, age);
                    if (HasQualification)
                    {
                        mainTournament.Qualification = qualificationTournament;
                    }

                    list.Add(mainTournament);
                }
            }

            Tournaments = list;
            return list;
        }

        public Tournament GenerateMain(int gender, int age)
        {
            var tournament = GenerateBasedTournament(gender, age);
            tournament.Stage = (int)Stage.Main;

            return tournament;
        }

        public Tournament GenerateQualification(int gender, int age)
        {
            var tournament = GenerateBasedTournament(gender, age);
            tournament.Stage = (int)Stage.Qual;
            tournament.NetRange /= tournament.NumberOfQualificationWinners;
            var qualificationStartDate = tournament.DateStart.AddDays(-2);
            tournament.DateEnd = tournament.DateStart.AddDays(-1);
            tournament.DateStart = qualificationStartDate;

            return tournament;
        }

        private Tournament GenerateBasedTournament(int gender, int age)
        {
            return new Tournament()
            {
                Name = Name,
                Age = age,
                Category = Category,
                Gender = gender,
                DateStart = DateStart,
                DateEnd = DateEnd,
                DateRequest = DateRequest,
                NetRange = NetRange,
                TennisCenterId = TennisCenter.Id,
                NumberOfQualificationWinners = NumberOfQualificationWinners,
            };
        }
    }
}
