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
            //todo: create anonymous factories
            return Ok(_context.Tournaments.ToList().GroupBy(x => x.Name).ToList());
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