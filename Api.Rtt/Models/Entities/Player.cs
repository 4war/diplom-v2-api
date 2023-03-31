using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Rtt.Models.Entities
{
    [Table("player")]
    public class Player
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Rni { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public int Point { get; set; }
        public int Gender { get; set; }
        public string Style { get; set; }
        public virtual List<PsychTestResult> TestResults { get; set; }

        [JsonIgnore]
        public virtual List<Tournament> Tournaments { get; set; } = new();
    }

    public enum Style
    {
        Defensive,
        Active,
        Reactive
    }
}