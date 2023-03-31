using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Helpers;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.JsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
    [Route("api/[controller]")]
    public class WinPredictController : Controller
    {
        private readonly MathModel _mathModel;
        private readonly Random _random = new Random();
        private readonly ApiContext _context;

        public WinPredictController(ApiContext context)
        {
            _context = context;
            _mathModel = new MathModel(context);
        }

        [HttpPost("fromMatch")]
        public IActionResult Predict([FromBody] Match match)
        {
            var contextMatch = _context.Matches.FirstOrDefault(x => x.Id == match.Id);
            if (contextMatch?.Player1Rni is null || contextMatch.Player2Rni is null) return NotFound();
            if (!match.Start.HasValue)
            {
                match.Start = _random.RandomizeStartDateTime(match);
                _context.SaveChanges();
            }
            
            return Ok(_mathModel.GetMultiLinearRegressionPredictionNoRating(contextMatch));
        }

        [HttpPost]
        public IActionResult Predict([FromBody] Prediction prediction)
        {
            var contextSelf = _context.Players.FirstOrDefault(x => x.Rni == prediction.Self.Rni);
            var contextEnemy = _context.Players.FirstOrDefault(x => x.Rni == prediction.Enemy.Rni);

            var newMatch = new Match()
            {
                Player1 = contextSelf,
                Player2 = contextEnemy,
                Court = new Court() { Surface = "hard" },
                Start = DateTime.Now,
            };

            var result = _mathModel.GetMultiLinearRegressionPredictionNoRating(newMatch);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("randomize/time")]
        public IActionResult RandomizeTime()
        {
            //todo: get all finished matches with score
            var matches = _context.Matches
                .Where(x => x.Score != null && x.Player1Rni != null && x.Player2Rni != null)
                .Where(x => x.End == null)
                .ToList();

            foreach (var match in matches)
            {
                _mathModel.SetRandomTime(match);
            }

            _context.Matches.UpdateRange(matches);
            _context.SaveChanges();
            return Ok();
        }
        

        [Authorize(Roles = "admin")]
        [HttpGet("randomize/style")]
        public IActionResult RandomizeStyles()
        {
            var list = Enum.GetNames(typeof(Style));
            var players = _context.Players.ToList();
            foreach (var player in players)
            {
                player.Style = list[_random.Next(list.Length)];
            }

            _context.Players.UpdateRange(players);
            _context.SaveChanges();

            return Ok();
        }
    }
}