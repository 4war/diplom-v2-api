using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Helpers
{
    public static class RandomExtensions
    {
        public static int GaussianNextInt(this Random random, double mean, double std)
        {
            var u1 = 1.0 - random.NextDouble();
            var u2 = 1.0 - random.NextDouble();
            var randomStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                  Math.Sin(2.0 * Math.PI * u2);
            return (int)(mean + std * randomStdNormal);
        }

        public static int ExponentialNextInt(this Random random, int min, int max, int targetMin, int targetMax)
        {
            return (targetMax - targetMin) * (random.Next(min, max) - min) / (max - min) + targetMin;
        }
        
        public static int RandomizeDurationInSeconds(this Random random, Match match)
        {
            var duration = 0;
            var games = string.Join("", match.Score.Split()).Sum(x => int.Parse(x.ToString()));
            for (var i = 0; i < games; i++)
            {
                var gameDuration = random.GaussianNextInt(210, 60); //in seconds
                foreach (var style in new List<string>() { match.Player1.Style, match.Player2.Style })
                {
                    if (style == "Defensive") gameDuration += 40;
                    if (style == "Reactive") gameDuration += 20;
                }

                duration += gameDuration;
            }

            return duration;
        }
        
        public static DateTime RandomizeStartDateTime(this Random random, Match match)
        {
            var date = match.Round.Bracket.Tournament.DateStart.AddDays(4 - Math.Log2(match.Round.Stage));
            var startDate = date
                .AddHours(random.ExponentialNextInt(8, 20, 8, 12))
                .AddMinutes(random.Next(0, 60));
            return startDate;
        }

    }
}