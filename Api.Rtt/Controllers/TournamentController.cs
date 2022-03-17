using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rtt.Controllers
{
    [Route("api/[controller]")]
    public class TournamentController : Controller
    {
        private readonly List<Tournament> _tournaments = new()
        {
            new Tournament(){Age = "Тест", Name = "Тест", CategoryDigit = "Тест", CategoryLetter = "Тест"},
        };
        
        [HttpGet(Name = "GetTournaments")]
        public IActionResult Get()
        {
            return Ok(_tournaments.OrderBy(x => x.Age).ToList());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Tournament tournament)
        {
            if (tournament is null)
            {
                return BadRequest();
            }

            _tournaments.Add(tournament);
            return Ok(_tournaments);
        }
    }
}