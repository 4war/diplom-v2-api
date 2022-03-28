using System.Linq;
using Api.Rtt.Models;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(_context.Tournaments.ToList().GroupBy(x => x.Name).
                Select(g => new TournamentFactory()
                {
                    Name = g.Key,
                    Category = g.First().Category,
                    Ages = g.Select(x => x.Age).Distinct().ToList(),
                    Genders = g.Select(x => x.Gender).Distinct().ToList(),
                    DateStart = g.First(x => x.IdQualification.HasValue).DateStart,
                    DateEnd = g.First(x => x.IdQualification.HasValue).DateEnd,
                    DateRequest = g.First(x => x.IdQualification.HasValue).DateRequest,
                    HasQualification = g.Any(x => x.IdQualification.HasValue),
                    NetRange = g.First(x => x.IdQualification.HasValue).NetRange,
                    TennisCenter = g.First().TennisCenter,
                    NumberOfQualificationWinners = g.First(x => x.IdQualification.HasValue).NumberOfQualificationWinners,
                }).ToList());
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