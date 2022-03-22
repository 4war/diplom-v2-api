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
    public class TennisCenterController : Controller
    {
        private readonly ApiContext _context;
        
        public TennisCenterController(ApiContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<TennisCenter> Get()
        {
            return _context.TennisCenters.OrderBy(x => x.Id).ToList();
        }
    }
}