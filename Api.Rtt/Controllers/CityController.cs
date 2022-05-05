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
  public class CityController : Controller
  {
    private readonly ApiContext _context;

    public CityController(ApiContext context)
    {
      _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] string startWith)
    {
      if (string.IsNullOrEmpty(startWith))
      {
        return Ok(new List<City>());
      }

      var list = await _context.Cities.ToListAsync();
      var result = list.Where(x => x.Name.StartsWith(startWith, StringComparison.InvariantCultureIgnoreCase));
      return Ok(result);
    }
  }
}
