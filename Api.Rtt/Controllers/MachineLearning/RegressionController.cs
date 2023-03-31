using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.MachineLearning;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers.MachineLearning
{
    [Route("api/[controller]")]
    public class RegressionController : Controller
    {
        private readonly ApiContext _context;
        private readonly MathModel _mathModel;

        public RegressionController(ApiContext context)
        {
            _context = context;
            _mathModel = new MathModel(context);
        }
        
        [HttpGet("recalibrate")]
        public IActionResult Recalibrate()
        {
            var _matches = _context.Matches
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .Where(x => x.WinnerRni.HasValue)
                .Where(x => x.Start.HasValue)
                .AsEnumerable()
                .Skip(0).
                ToList();

            foreach (var match in _matches)
            {
                var existingRegression = _context.RegressionData.Find(match.Id);
                var factor =
                    _mathModel.GetPersonalFactorGroup(match, match.Start!.Value);
                existingRegression.FactorScoreTimeEffectPersonal = factor;
                _context.RegressionData.Update(existingRegression);
            }

            return Ok();
        }

        [HttpPost("setup")]
        public IActionResult SetupData()
        {
            var _matches = _context.Matches
                .Where(x => x.Player1Rni.HasValue && x.Player2Rni.HasValue)
                .Where(x => x.WinnerRni.HasValue)
                .Where(x => x.Start.HasValue)
                .AsEnumerable()
                .Skip(1544).ToList();
            
            foreach (var match in _matches)
            {
                var currentPrediction = _mathModel.GetPrediction(match.Player1, match.Player2, match);

                var actual = _mathModel.CalculateScore(match, match.Player1) - 0.5;
                var regression = _mathModel.GetRegressionData(match);
                regression.CurrentPrediction = currentPrediction.Win;
                var existingRegression = _context.RegressionData.Find(match.Id);
                if (existingRegression == null)
                {
                    _context.RegressionData.Add(regression);
                    _context.SaveChanges();
                }
                else
                {
                    existingRegression.Actual = Math.Round(actual * 100);
                    existingRegression.CurrentPrediction = currentPrediction.Win;
                    existingRegression.FactorScoreTimeEffect = Math.Round(regression.FactorScoreTimeEffect, 2);
                    existingRegression.FactorCount = Math.Round(regression.FactorCount, 2);
                    existingRegression.FactorDiversity = Math.Round(regression.FactorDiversity, 2);
                    existingRegression.FactorScoreTimeEffectPersonal = Math.Round(regression.FactorScoreTimeEffectPersonal, 2);
                    existingRegression.FactorFatigue = Math.Round(regression.FactorFatigue, 2);
                    existingRegression.FactorDayTime = Math.Round(regression.FactorDayTime, 2);
                    existingRegression.FactorMoral = Math.Round(regression.FactorMoral, 2);
                    existingRegression.FactorStyle = Math.Round(regression.FactorStyle, 2);
                    existingRegression.FactorRating = Math.Round(regression.FactorRating, 2);
                    _context.RegressionData.Update(existingRegression);
                    _context.SaveChanges();
                }
            }

            return Ok();
        }
    }
}