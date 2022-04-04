using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Rtt.Controllers
{
    [Route("api/[controller]")]
    public class TournamentFactoryController : Controller
    {
        private readonly ApiContext _context;

        public TournamentFactoryController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = _context.Tournaments.ToList();
            var groups = new Dictionary<string, Dictionary<DateTime, List<Tournament>>>();
            foreach (var tournament in list)
            {
                if (!groups.ContainsKey(tournament.Name))
                    groups[tournament.Name] = new Dictionary<DateTime, List<Tournament>>();

                var firstDate = tournament.DateStart;
                if (tournament.Qualification is not null)
                    firstDate = tournament.Qualification.DateStart;
                
                if (!groups[tournament.Name].ContainsKey(firstDate))
                    groups[tournament.Name][firstDate] = new List<Tournament>();
                
                groups[tournament.Name][firstDate].Add(tournament);
            }

            var result = groups
                .Select(d=> d.Value
                    .Select(g => new TournamentFactory()
            {
                FirstTournamentId = g.Value.First().Id,
                Name = d.Key,
                Category = g.Value.First().Category,
                Ages = g.Value.Select(x => x.Age).Distinct().ToList(),
                Genders = g.Value.Select(x => x.Gender).Distinct().ToList(),
                DateStart = g.Value.First(x => x.QualificationId.HasValue).DateStart,
                DateEnd = g.Value.First(x => x.QualificationId.HasValue).DateEnd,
                DateRequest = g.Value.First(x => x.QualificationId.HasValue).DateRequest,
                HasQualification = g.Value.Any(x => x.QualificationId.HasValue),
                NetRange = g.Value.First(x => x.QualificationId.HasValue).NetRange,
                TennisCenter = g.Value.First().TennisCenter,
                NumberOfQualificationWinners =
                    g.Value.First(x => x.QualificationId.HasValue).NumberOfQualificationWinners,
                Tournaments = g.Value.ToList(),
            })).SelectMany(m => m).ToList();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var tournament = await _context.Tournaments.FirstOrDefaultAsync(x => x.Id == id);
            if (tournament is null)
            {
                return NotFound();
            }


            var list = _context.Tournaments
                .Where(x => x.Name == tournament.Name)
                .ToList();
            //todo also search by date

            var factory = new TournamentFactory()
            {
                FirstTournamentId = tournament.Id,
                Name = tournament.Name,
                Category = tournament.Category,
                Ages = list.Select(x => x.Age).Distinct().ToList(),
                Genders = list.Select(x => x.Gender).Distinct().ToList(),
                DateStart = list.First(x => x.QualificationId.HasValue).DateStart,
                DateEnd = list.First(x => x.QualificationId.HasValue).DateEnd,
                DateRequest = list.First(x => x.QualificationId.HasValue).DateRequest,
                HasQualification = list.Any(x => x.QualificationId.HasValue),
                NetRange = list.First(x => x.QualificationId.HasValue).NetRange,
                TennisCenter = tournament.TennisCenter,
                NumberOfQualificationWinners = list.First(x => x.QualificationId.HasValue).NumberOfQualificationWinners,
                Tournaments = list,
            };

            return Ok(factory);
        }


        [HttpPost]
        public IActionResult Post([FromBody] TournamentFactory tf)
        {
            if (tf is null)
            {
                return BadRequest();
            }

            var list = tf.Generate();
            foreach (var t in list)
            {
                _context.Tournaments.Add(t);
            }

            _context.SaveChanges();
            return Ok(_context.Tournaments);
        }
    }
}