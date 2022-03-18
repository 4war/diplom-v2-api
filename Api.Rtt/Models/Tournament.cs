using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models
{
    [Table("tournament")]
    public class Tournament
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryDigit { get; set; }
        public string CategoryLetter { get; set; }
        public string Age { get; set; }
        
        //public DateTime DateStart { get; set; }
        //public DateTime DateEnd { get; set; }
    }
}