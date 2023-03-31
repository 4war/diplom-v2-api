using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.MachineLearning;
using Api.Rtt.Models.JsModels;

namespace Api.Rtt.Helpers
{
    public class MathModel
    {
        public readonly double NormalMatchCount = 10;
        private readonly ApiContext _context;
        private readonly Random _random = new Random();
        private readonly Dictionary<int, int> _averageRatingPerAge;

        private readonly List<int> _timeRanges = new List<int>()
        {
            8, 10, 12, 14, 16, 18, 20, 22, 24,
        };

        public MathModel(ApiContext context)
        {
            _context = context;
            _averageRatingPerAge = _context.Players.AsEnumerable()
                .GroupBy(x => DateTime.Today.Year - x.DateOfBirth.Year)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x =>
                    x.Key > 32
                        ? 2000
                        : (int)x
                            .Where(y => y.Point > 0)
                            .Average(y => y.Point));
        }

        public Prediction GetMultiLinearRegressionPredictionNoRating(Match match)
        {
            var regressionData = GetRegressionData(match);
            var coefficientArray = new double[]
            {
                -7.83181626, 18.18779916, -5.02137477, 57.60481569,
                -16.53513389, 24.20931883, -2.05427604, 13.3723608
            };

            var factorArray = new double[]
            {
                regressionData.FactorScoreTimeEffect,
                regressionData.FactorCount,
                regressionData.FactorDiversity,
                regressionData.FactorScoreTimeEffectPersonal,
                regressionData.FactorDayTime,
                regressionData.FactorFatigue,
                regressionData.FactorMoral,
                regressionData.FactorStyle
            };

            var form = coefficientArray[0] * factorArray[0] +
                       coefficientArray[1] * factorArray[1] +
                       coefficientArray[2] * factorArray[2];
            var personal = coefficientArray[3] * factorArray[3];
            var condition = coefficientArray[4] * factorArray[4] + coefficientArray[5] * factorArray[5];
            var psych = coefficientArray[6] * factorArray[6] + coefficientArray[7] * factorArray[7];
            var resultWin = coefficientArray.Zip(factorArray, (a, x) => a * x).Sum();
            var result = new Prediction()
            {
                Form = Math.Max(Math.Min(form, 100), 0),
                Personal = Math.Max(Math.Min(personal, 100), 0),
                Condition = Math.Max(Math.Min(condition, 100), 0),
                Psych = Math.Max(Math.Min(psych, 100), 0),
                Date = match.Start!.Value,
                Self = match.Player1,
                Enemy = match.Player2,
                Rating = 1,
                Win = (int)Math.Round(resultWin, 0),
            };

            return result;
        }

        public Regression GetRegressionData(Match match)
        {
            var scoreTimeEfficiencyFactor =
                GetScoreTimeEfficiencyFactor(match.Player1, match.Player2, match.Start!.Value);
            var diversityFactor = GetDiversityFactor(match.Player1, match.Player2);
            var countFactor = GetCountFactor(match.Player1, match.Player2, match.Start.Value);
            var personalFactor = GetPersonalFactorGroup(match, match.Start.Value);
            var selfDayTimeFactor = GetDaytimeMatchesFactorCount(match.Player1, match.Start.Value);
            var enemyDayTimeFactor = GetDaytimeMatchesFactorCount(match.Player2, match.Start.Value);
            var dayTimeCountFactor =
                1 / (1 + Math.Exp((enemyDayTimeFactor - selfDayTimeFactor) / NormalMatchCount)) + 0.5;
            var fatigueFactor = GetFatigueFactor(match.Player1, match.Player2, match);
            var moralFactor = GetMoralFactor(match.Player1, match.Player2);
            var styleFactor = GetStyleFactor(match.Player1, match.Player2);
            var ratingFactor = Math.Pow((double)(match.Player1.Point + 100) / (match.Player2.Point + 100), 0.25);

            var result = new Regression()
            {
                MatchId = match.Id,
                Match = match,
                FactorScoreTimeEffect = Math.Round(scoreTimeEfficiencyFactor, 2),
                FactorCount = Math.Round(countFactor, 2),
                FactorDiversity = Math.Round(diversityFactor, 2),
                FactorScoreTimeEffectPersonal = Math.Round(personalFactor, 2),
                FactorFatigue = Math.Round(fatigueFactor, 2),
                FactorDayTime = Math.Round(dayTimeCountFactor, 2),
                FactorMoral = Math.Round(moralFactor, 2),
                FactorStyle = Math.Round(styleFactor, 2),
                FactorRating = Math.Round(ratingFactor, 2),
            };
            return result;
        }

        public Prediction GetPrediction(Player self, Player enemy, Match match)
        {
            const int kf = 4;
            const int kpr = 5;
            const int kc = 1;
            const int kps = 1;

            if (!match.Start.HasValue)
            {
                var startDate = _random.RandomizeStartDateTime(match);
                match.Start = startDate;
                _context.Matches.Update(match);
                _context.SaveChanges();
            }

            // ReSharper disable InconsistentNaming
            var Ff = GetFormFactorGroup(self, enemy, match.Start.Value);
            var Fpr = GetPersonalFactorGroup(match, match.Start.Value);
            var Fc = GetConditionFactorGroup(self, enemy, match);
            var Fps = GetPsychFactorGroup(self, enemy);
            var Fr = Math.Pow(self.Point / (double)enemy.Point, 0.25);
            var f = (Ff * kf + Fpr * kpr + Fc * kc + Fps * kps) / (kf + kpr + kc + kps) * Fr;

            var result = f / (f + 1);
            return new Prediction()
            {
                Self = self,
                Enemy = enemy,
                Date = match.Start.Value,
                Form = Math.Round(Ff, 2),
                Personal = Math.Round(Fpr, 2),
                Condition = Math.Round(Fc, 2),
                Psych = Math.Round(Fps, 2),
                Rating = Math.Round(Fr, 2),
                Win = (int)(Math.Round(result * 100))
            };
        }

        public double GetFormFactorGroup(Player self, Player enemy, DateTime dateTime)
        {
            var scoreTimeEfficiencyFactor = GetScoreTimeEfficiencyFactor(self, enemy, dateTime);
            var diversityFactor = GetDiversityFactor(self, enemy);
            var countFactor = GetCountFactor(self, enemy, dateTime);

            return scoreTimeEfficiencyFactor
                   * countFactor
                   * diversityFactor;
        }

        public double GetScoreTimeEfficiencyFactor(Player self, Player enemy, DateTime dateTime)
        {
            var selfEfficientFactor = GetEfficiencyFactor(self, dateTime);
            var enemyEfficientFactor = GetEfficiencyFactor(enemy, dateTime);
            return Math.Pow(selfEfficientFactor / enemyEfficientFactor, 2);
        }

        public double GetDiversityFactor(Player self, Player enemy)
        {
            var selfPlayerDiversity = GetPlayerDiversityFactor(self);
            var enemyPlayerDiversity = GetPlayerDiversityFactor(enemy);
            return selfPlayerDiversity / enemyPlayerDiversity;
        }

        public double GetPersonalFactorGroup(Match match, DateTime dateTime)
        {
            var matches = _context.Matches
                .Where(x => x.Id != match.Id &&
                    (x.Player1Rni == match.Player1Rni || x.Player2Rni == match.Player1Rni) &&
                    (x.Player1Rni == match.Player2Rni || x.Player2Rni == match.Player2Rni) &&
                    x.Score != null && x.WinnerRni.HasValue)
                .Where(x => x.Start.HasValue && x.Start < dateTime)
                .ToList();

            var selfEfficiency = GetEfficiencyFactorFromMatches(match.Player1, matches, dateTime);
            var enemyEfficiency = GetEfficiencyFactorFromMatches(match.Player2, matches, dateTime);
            return Math.Sqrt((selfEfficiency + 1) / (enemyEfficiency + 1));
        }

        public double GetConditionFactorGroup(Player self, Player enemy, Match match)
        {
            if (match.Court is null)
            {
                var tennisCenterCourts = match.Round.Bracket.Tournament.Factory.TennisCenter.Courts;
                match.Court = tennisCenterCourts[_random.Next(tennisCenterCourts.Count)];
                _context.Matches.Update(match);
                _context.SaveChanges();
            }

            var selfSurfaceCount = GetSurfaceMatchesFactorCount(self, match.Court.Surface);
            var enemySurfaceCount = GetSurfaceMatchesFactorCount(enemy, match.Court.Surface);
            var fSurface = 1 / (1 + Math.Exp((enemySurfaceCount - selfSurfaceCount) / NormalMatchCount)) + 0.5;
            var fDayTime = GetDaytimeFactor(self, enemy, match);
            var fFatigue = GetFatigueFactor(self, enemy, match);
            return fSurface * fDayTime * fFatigue;
        }

        public double GetDaytimeFactor(Player self, Player enemy, Match match)
        {
            var selfTimeCount = match.Start == null ? 1 : GetDaytimeMatchesFactorCount(self, match.Start.Value);
            var enemyTimeCount = match.Start == null ? 1 : GetDaytimeMatchesFactorCount(enemy, match.Start.Value);
            return 1 / (1 + Math.Exp((enemyTimeCount - selfTimeCount) / NormalMatchCount)) + 0.5;
        }

        public double GetFatigueFactor(Player self, Player enemy, Match match)
        {
            var selfFatigue = GetFatigueFactor(self, match) + 1;
            var enemyFatigue = GetFatigueFactor(enemy, match) + 1;
            return Math.Pow(selfFatigue / enemyFatigue, 0.5);
        }

        public double GetPsychFactorGroup(Player self, Player enemy)
        {
            var styleFactor = GetStyleFactor(self, enemy);
            var moralFactor = !self.TestResults.Any() || !enemy.TestResults.Any()
                ? GetMoralFactor(self, enemy)
                : GetTestFactor(self, enemy);

            return moralFactor * styleFactor;
        }

        public double GetPsychFactorGroupIfNoTestPassed(Player self)
        {
            var age = DateTime.Today.Year - self.DateOfBirth.Year;
            var minEnemyPoint = _averageRatingPerAge[age] / 2;
            var selfMatches = GetPlayerMatches(self)
                .Where(x => (x.Player1Rni != self.Rni && x.Player1.Point > minEnemyPoint)
                            || x.Player2Rni != self.Rni && x.Player2.Point > minEnemyPoint).ToList();

            return selfMatches.Count > 5 ? CalculateWinRate(selfMatches, self) + 0.5 : 1;
        }


        public double GetMoralFactor(Player self, Player enemy)
        {
            return GetPsychFactorGroupIfNoTestPassed(self) / GetPsychFactorGroupIfNoTestPassed(enemy);
        }

        public double CalculateWinRate(List<Match> matches, Player player)
        {
            return matches.Count(x => x.WinnerRni == player.Rni) / (double)matches.Count;
        }

        public List<Match> GetPlayerMatches(Player self)
        {
            var selfMatches = _context.Matches.AsEnumerable()
                .Where(x => x.Player1Rni == self.Rni || x.Player2Rni == self.Rni)
                .Where(x => x.Player1Rni.HasValue
                            && x.Player2Rni.HasValue
                            && x.WinnerRni.HasValue
                            && x.Score != null)
                .ToList();
            return selfMatches;
        }

        public double GetTestFactor(Player self, Player enemy)
        {
            var selfMoral = self.TestResults.ToList().Last().Moral;
            var enemyMoral = enemy.TestResults.ToList().Last().Moral;
            return Math.Sqrt(selfMoral / (double)enemyMoral);
        }

        public double GetStyleFactor(Player self, Player enemy)
        {
            var selfStyle = self.TestResults.Any()
                ? GetStyleList(self.TestResults.ToList().Last())
                : GetStyleListIfNoTestPassed(self);

            var enemyStyle = enemy.TestResults.Any()
                ? GetStyleList(enemy.TestResults.ToList().Last())
                : GetStyleListIfNoTestPassed(enemy);

            var selfTotal = GetTotalStyle(selfStyle, enemyStyle);
            var enemyTotal = GetTotalStyle(enemyStyle, selfStyle);

            return selfTotal / enemyTotal;
        }

        public List<int> GetStyleList(PsychTestResult result)
        {
            return new List<int> { result.Defensive, result.Active, result.Reactive };
        }

        public List<int> GetStyleListIfNoTestPassed(Player self)
        {
            return new List<int>()
            {
                self.Style == "Defensive" ? 70 : 30,
                self.Style == "Active" ? 70 : 30,
                self.Style == "Reactive" ? 70 : 30,
            };
        }

        public double GetTotalStyle(List<int> selfStyle, List<int> enemyStyle)
        {
            var weights = new List<double> { 0.8, 1, 1.25 };

            var y = 2;
            var total = 0d;
            for (var i = 0; i < 3; i++)
            {
                var sum = 0d;
                var currentY = (y + i) % 3;
                var x = currentY;
                for (var w = 0; w < 3; w++)
                {
                    var currentX = (x + w) % 3;
                    sum += selfStyle[currentX] * weights[w];
                }

                total += sum * enemyStyle[i];
            }

            return total / (enemyStyle.Sum());
        }

        public int GetSurfaceMatchesFactorCount(Player player, string surface)
        {
            return _context.Matches
                .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.CourtId.HasValue)
                .Count(x => x.Court.Surface == surface);
        }

        public int GetDaytimeMatchesFactorCount(Player player, DateTime start)
        {
            var startUtcHour = start.Hour + (int)(DateTime.Now.Subtract(DateTime.UtcNow).TotalHours);
            var range = startUtcHour <= _timeRanges[0]
                ? _timeRanges[0]
                : _timeRanges.TakeWhile(x => x < startUtcHour).Max();

            // ReSharper disable once RemoveToList.2
            var matches = _context.Matches
                .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .ToList()
                .Count(x => x.Start.HasValue && x.Start.Value.Hour >= range && x.Start.Value.Hour < range + 2);
            return matches;
        }

        public double GetFatigueFactor(Player player, Match match)
        {
            if (!match.Start.HasValue || match.Round?.Bracket == null) return 1d;

            var games = _context.Matches.Where(x => x.Round.BracketId == match.Round.BracketId)
                .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .ToList();
            var result = games
                .Where(x => x.Start.HasValue && x.End.HasValue)
                .Select(x => new
                {
                    Duration = (x.End.Value - x.Start.Value).TotalMinutes,
                    TimePassed = (match.Start.Value - x.End.Value).TotalHours,
                }).ToList();

            return result.Select(x => x.Duration * Math.Exp(-0.01 * x.TimePassed)).Sum();
        }

        public double GetEfficiencyFactor(Player player, DateTime dateTime)
        {
            var selfMatches = GetPlayerMatches(player);
            return GetEfficiencyFactorFromMatches(player, selfMatches, dateTime);
        }

        public double GetEfficiencyFactorFromMatches(Player player, List<Match> selfMatches, DateTime dateTime)
        {
            var fResArray = new List<double>();
            var fTimeArray = new List<double>();
            foreach (var match in selfMatches.ToList())
            {
                if (!match.Start.HasValue) continue;
                var selfInMatch = (match.Player1Rni == player.Rni ? match.Player1 : match.Player2) ??
                                  EnsureGetPlayerByRni(match.Player1Rni == player.Rni
                                      ? match.Player1Rni
                                      : match.Player2Rni);
                var enemyInMatch = (match.Player1Rni == player.Rni ? match.Player2 : match.Player1) ??
                                   EnsureGetPlayerByRni(match.Player1Rni == player.Rni
                                       ? match.Player2Rni
                                       : match.Player1Rni);
                var days = (dateTime - match.Start.Value).TotalDays;
                var age = match.Start.Value.Year - selfInMatch.DateOfBirth.Year;
                var score = CalculateScore(match, player);
                var averageRating = _averageRatingPerAge.ContainsKey(age) ? _averageRatingPerAge[age] : 500;
                fResArray.Add(Math.Pow((double)enemyInMatch.Point / averageRating, 1.2) * Math.Pow(score, 0.8));

                fTimeArray.Add(Math.Exp(-0.01 * days));
            }

            if (!fTimeArray.Any()) return 1;
            var fEf = fResArray.Zip(fTimeArray, (fRes, fTime) => fRes * fTime).Sum()
                      / fTimeArray.Sum();

            return fEf;
        }

        public Player EnsureGetPlayerByRni(int? rni)
        {
            return rni.HasValue ? _context.Players.Find(rni) : null;
        }

        public double GetCountFactor(Player self, Player enemy, DateTime dateTime)
        {
            var selfMatchCount = _context.Matches
                .Where(x => x.Player1Rni == self.Rni || x.Player2Rni == self.Rni)
                .Where(x => x.Start > dateTime.AddYears(-1))
                .Count(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.WinnerRni.HasValue);

            var enemyMatchCount = _context.Matches
                .Where(x => x.Start > dateTime.AddYears(-1))
                .Where(x => x.Player1Rni == enemy.Rni || x.Player2Rni == enemy.Rni)
                .Count(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue && x.WinnerRni.HasValue);

            var difference = selfMatchCount - enemyMatchCount;
            return 1 / (1 + Math.Exp(-difference / NormalMatchCount)) + 0.5;
        }

        public double GetPlayerDiversityFactor(Player player)
        {
            var playerUniqueMatches = _context.Matches
                .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .Select(x => x.Player1Rni == player.Rni ? x.Player2Rni.Value : x.Player1Rni.Value)
                .GroupBy(x => x).Select(x => x.Count()).ToList();

            return 1 / playerUniqueMatches.Average();
        }

        // ReSharper disable once UnusedMember.Local
        public double GetTechniqueDiversityFactor(Player player)
        {
            var tcUniqueMatches = _context.Matches
                .Where(x => x.Player1Rni == player.Rni || x.Player2Rni == player.Rni)
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .Select(x => x.Round.Bracket.Tournament.Factory.TennisCenterId)
                .GroupBy(x => x).Select(x => x.Count()).ToList();

            return 1 / tcUniqueMatches.Average();
        }

        public double CalculateScore(Match match, Player player)
        {
            var self = player.Rni == match.Player1Rni ? match.Player1 : match.Player2;
            var score = match.Score;
            if (self.Rni != match.WinnerRni) score = ReverseScore(score);
            var sets = score.Split();

            var winCounter = 0;
            var loseCounter = 0;
            var setCounter = 0;
            foreach (var set in sets)
            {
                var selfGame = int.Parse(set[0].ToString());
                var enemyGame = int.Parse(set[1].ToString());
                winCounter += selfGame;
                loseCounter += enemyGame;

                if (winCounter > loseCounter)
                {
                    winCounter += 3;
                    setCounter += 1;
                }
                else
                {
                    loseCounter += 3;
                    setCounter -= 1;
                }
            }

            if (setCounter > 0) winCounter += 4;
            else loseCounter += 4;

            return (double)winCounter / (winCounter + loseCounter) + 0.5;
        }

        public string ReverseScore(string score)
        {
            return string.Join(" ", score.Split().Select(x => string.Join("", new List<char> { x[1], x[0] })));
        }

        public void SetRandomTime(Match match)
        {
            var startDate = _random.RandomizeStartDateTime(match);
            var durationInSeconds = _random.RandomizeDurationInSeconds(match);
            var endDate = startDate.AddSeconds(durationInSeconds);
            match.Start = startDate;
            match.End = endDate;
        }
    }
}