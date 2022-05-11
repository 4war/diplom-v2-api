using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Api.Rtt.Models.Entities
{
  [Table("tournament_factory")]
  public class TournamentFactory
  {
    [Key]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("ages")]
    public string Ages { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("hasQualification")]
    public bool HasQualification { get; set; }

    [JsonProperty("numberOfQualificationWinners")]
    public int NumberOfQualificationWinners { get; set; }

    [JsonProperty("dateStart")]
    public DateTime DateStart { get; set; }

    [JsonProperty("dateEnd")]
    public DateTime DateEnd { get; set; }

    [JsonProperty("dateRequest")]
    public DateTime DateRequest { get; set; }

    [JsonProperty("netRange")]
    public int NetRange { get; set; } = 32;

    [JsonProperty("tennisCenter")]
    public virtual TennisCenter TennisCenter { get; set; }


    public int TennisCenterId { get; set; }

    [JsonProperty("genders")]
    public string Genders { get; set; } = string.Join(" ", new List<int> { (int)Gender.Male, (int)Gender.Female });

    public virtual List<Tournament> Tournaments { get; set; } = new List<Tournament>();

    public virtual List<Schedule> Schedules { get; set; } = new List<Schedule>();

    public void SetDate()
    {
      DateEnd = DateStart.AddDays(Math.Log2(NetRange));
      DateRequest = DateStart.AddDays(-14);
    }


    public List<Tournament> Generate()
    {
      var list = new List<Tournament>();

      foreach (var age in Ages.Split().Select(int.Parse))
      {
        foreach (var gender in Genders.Split().Select(int.Parse))
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
        NumberOfQualificationWinners = NumberOfQualificationWinners,
      };
    }
  }
}
