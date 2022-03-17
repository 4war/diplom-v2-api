using System;

namespace Api.Rtt.Models
{
    public class Tournament
    {
        public string Name { get; set; }
        public string CategoryDigit { get; set; }
        public string CategoryLetter { get; set; }
        public string Age { get; set; }
        
        //public DateTime DateStart { get; set; }
        //public DateTime DateEnd { get; set; }
    }
}