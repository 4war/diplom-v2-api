using System;
using System.IO;
using System.Linq;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace Api.Rtt.Controllers
{
  [Route("api/[controller]")]
  public class PlayerTournamentController : Controller
  {
    private readonly ApiContext _context;

    public PlayerTournamentController(ApiContext context)
    {
      _context = context;
    }

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
      var playerList = _context.Tournaments
        .FirstOrDefault(x => x.Id == id);

      if (playerList is null)
        return NotFound();

      var result = playerList.Players
        .OrderByDescending(x => x.Point)
        .AsQueryable();

      return Ok(result);
    }

    [HttpPost("{id:int}")]
    [Authorize(Roles = "org")]
    public IActionResult Post([FromRoute] int id, [FromBody] Player player)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == id);
      if (tournament is null) return NotFound();

      if (tournament.Players.FirstOrDefault(x => x.Rni == player.Rni) is not null)
        return BadRequest();

      tournament.Players.Add(player);
      _context.SaveChanges();
      return Ok(player);
    }

    [HttpDelete("{idTournament:int}/{rni:int}")]
    [Authorize(Roles = "org")]
    public IActionResult Delete([FromRoute] int idTournament, [FromRoute] int rni)
    {
      var tournament = _context.Tournaments.FirstOrDefault(x => x.Id == idTournament);
      if (tournament is null)
      {
        return NotFound();
      }

      var player = tournament.Players.FirstOrDefault(x => x.Rni == rni);
      if (player is null)
      {
        return NotFound();
      }

      tournament.Players.Remove(player);
      _context.SaveChanges();
      return Ok(player);
    }

    [HttpPost("document"), DisableRequestSizeLimit]
    public IActionResult Test()
    {
      var file = Request.Form.Files[0];
      using (var ms = new MemoryStream())
      {
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        var s = Convert.ToBase64String(fileBytes);
        var mug = new Document() { File = fileBytes };
        _context.Documents.Add(mug);
        _context.SaveChanges();
      }

      return Ok();
    }

    [HttpGet("document"), DisableRequestSizeLimit]
    public IActionResult GetDocuments()
    {
      var document = _context.Documents.First(x => x.Id == 6);
      return Ok(document.File);
    }
  }
}
