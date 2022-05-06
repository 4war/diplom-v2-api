﻿using System;
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
  public class TournamentController : Controller
  {
    private readonly ApiContext _context;

    public TournamentController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IEnumerable<Tournament> Get()
    {
      return _context.Tournaments
        .OrderBy(x => x.DateStart)
        .AsQueryable();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
      var tournament = await _context.Tournaments.SingleOrDefaultAsync(x => x.Id == id);
      if (tournament is null)
      {
        return NotFound();
      }

      return Ok(tournament);
    }
  }
}
