using System.Collections.Generic;

namespace Api.Rtt.Models
{
    public class Ages
    {
        public static Dictionary<int, Age> Dictionary { get; set; } = new Dictionary<int, Age>()
        {
            [10] = new (){Number = 10, ViewValue = "9-10 лет"},
            [12] = new (){Number = 12, ViewValue = "до 13 лет"},
            [14] = new (){Number = 14, ViewValue = "до 15 лет"},
            [16] = new (){Number = 16, ViewValue = "до 17 лет"},
            [18] = new (){Number = 18, ViewValue = "до 19 лет"},
            [100] = new (){Number = 100, ViewValue = "Взрослые"},
        };
    }

    public class Age
    {
        public string ViewValue { get; set; }
        public int Number { get; set; }
    }
}