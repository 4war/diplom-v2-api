﻿using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models
{
    public class TournamentFactory
    {
        public string Name { get; set; }
        public List<int> Ages { get; set; }
        public string Category { get; set; }
        public bool HasQualification { get; set; }
        public Tournament Qualification { get; set; }
        public int NumberOfQualificationWinners { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateRequest { get; set; }
        public int NetRange { get; set; } = 32;
        public TennisCenter TennisCenter { get; set; }
        public List<Gender> Genders { get; set; } = new List<Gender>() { Gender.Male, Gender.Female };

        public void SetDate()
        {
            DateEnd = DateStart.AddDays(Math.Log2(NetRange));
            DateRequest = DateStart.AddDays(-14);
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

            return list;
        }

        public Tournament GenerateMain(Gender gender, int age)
        {
            var tournament = GenerateBasedTournament(gender, age);
            tournament.Stage = Stage.Main;

 
            
            return tournament;
        }

        public Tournament GenerateQualification(Gender gender, int age)
        {
            var tournament = GenerateBasedTournament(gender, age);
            tournament.Stage = Stage.Qual;
            tournament.NetRange /= tournament.NumberOfQualificationWinners;
            var days = Math.Log2(tournament.NetRange);
            var qualificationStartDate = tournament.DateStart.AddDays(-days);
            tournament.DateEnd = tournament.DateStart.AddDays(-1);
            tournament.DateStart = qualificationStartDate;

            return tournament;
        }

        private Tournament GenerateBasedTournament(Gender gender, int age)
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
                IdTennisCenter = TennisCenter.Id,
                NumberOfQualificationWinners = NumberOfQualificationWinners,
            };
        }
    }
}