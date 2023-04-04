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

        public Prediction GetNotRetartedResult(Match match)
        {
            var regressionData = GetRegressionData(match);
            var factorArray = new double[]
            {
                regressionData.FactorScoreTimeEffect,
                regressionData.FactorCount,
                regressionData.FactorDiversity,
                regressionData.FactorScoreTimeEffectPersonal,
                regressionData.FactorDayTime,
                regressionData.FactorFatigue,
                regressionData.FactorMoral,
                regressionData.FactorStyle,
            };

            var totalNoRating = factorArray.Aggregate((x, y) => x * y);
            var totalRating = totalNoRating * regressionData.FactorRating;

            var resultNoRating = GetRelativeResult(totalNoRating);
            var resultRating = GetRelativeResult(totalRating);
            return new Prediction()
            {
                Date = match.Start!.Value,
                Self = match.Player1,
                Enemy = match.Player2,
                ClearPrediction = (int)(Math.Round(resultNoRating * 100, 0)),
                RatingPrediction = (int)(Math.Round(resultRating * 100, 0)),
            };
        }

        private double GetRelativeResult(double value)
        {
            return value / (value + 1);
        }

        public Prediction GetMultiLinearRegressionPredictionNoRating(Match match)
        {
            var regressionData = GetRegressionData(match);
            var result = new Prediction()
            {
                Date = match.Start!.Value,
                Self = match.Player1,
                Enemy = match.Player2,
                ClearPrediction = GetClearPrediction(regressionData),
                RatingPrediction = GetRatingPrediction(regressionData)
            };

            return result;
        }

        private int GetClearPrediction(Regression regressionData)
        {
            var coefficientArray = new double[]
            {
                16.55964844, 8.04340933, -8.56757983, -8.39585571, 35.19347896,
                2.88224585, -0.15443087
            };

            var factorArray = new double[]
            {
                regressionData.FactorScoreTimeEffect,
                regressionData.FactorCount,
                regressionData.FactorDiversity,
                regressionData.FactorDayTime,
                regressionData.FactorFatigue,
                regressionData.FactorMoral,
                regressionData.FactorStyle
            };

            var result = coefficientArray.Zip(factorArray, (a, x) => a * x).Sum()
                         / coefficientArray.Sum();

            return (int)Math.Round(result / (result + 1), 2);
        }


        private int GetRatingPrediction(Regression regressionData)
        {
            var coefficientArray = new double[]
            {
                -15.4063362, 3.97350453, -3.25148573, 66.49602047,
                -7.04789544, 14.80812846, -4.76229066, -2.65363531,
                64.44242284
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
                regressionData.FactorStyle,
                regressionData.FactorRating
            };


            var result = coefficientArray.Zip(factorArray, (a, x) => a * x).Sum()
                         / coefficientArray.Sum();

            return (int)Math.Round(result / (result + 1), 2);
        }

        public Regression GetRegressionData(Match match)
        {
            var scoreTimeEfficiencyFactor =
                GetScoreTimeEfficiencyFactor(match.Player1, match.Player2, match.Start!.Value);
            var diversityFactor = GetDiversityFactor(match.Player1, match.Player2);
            var countFactor = GetCountFactor(match.Player1, match.Player2, match.Start.Value);
            var personalFactor = GetPersonalFactor(match, match.Start.Value);
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
            return Math.Pow(selfEfficientFactor / enemyEfficientFactor, 0.5);
        }

        public double GetDiversityFactor(Player self, Player enemy)
        {
            var selfPlayerDiversity = GetPlayerDiversityFactor(self);
            var enemyPlayerDiversity = GetPlayerDiversityFactor(enemy);
            return selfPlayerDiversity / enemyPlayerDiversity;
        }

        public double GetPersonalFactor(Match match, DateTime dateTime)
        {
            var matches = _context.Matches
                .Where(x => x.Id != match.Id &&
                            (x.Player1Rni == match.Player1Rni || x.Player2Rni == match.Player1Rni) &&
                            (x.Player1Rni == match.Player2Rni || x.Player2Rni == match.Player2Rni) &&
                            x.Score != null && x.WinnerRni.HasValue)
                .Where(x => x.Start.HasValue && x.Start < dateTime)
                .ToList();

            var fResArray = new List<double>();
            var fTimeArray = new List<double>();
            foreach (var playedMatch in matches)
            {
                var days = (dateTime - playedMatch.Start!.Value).TotalDays;
                var score = CalculateScore(playedMatch, playedMatch.Player1);
                fResArray.Add(score);
                fTimeArray.Add(Math.Exp(-0.01 * days));
            }

            if (!fTimeArray.Any()) return 1;
            var fEf = fResArray.Zip(fTimeArray, (fRes, fTime) => fRes * fTime).Sum()
                      / fTimeArray.Sum();

            return fEf;
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
                .Where(x => (match.Start.Value - x.End.Value).TotalHours > 0)
                .Select(x => new
                {
                    Duration = (x.End.Value - x.Start!.Value).TotalMinutes,
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