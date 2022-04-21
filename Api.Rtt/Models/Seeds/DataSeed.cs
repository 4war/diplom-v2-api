using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class DataSeed
  {
    private ApiContext _context;
    private TournamentFactorySeed _tournamentFactorySeed;
    private PlayerSeed _playerSeed;
    private TennisCenterSeed _tennisCenterSeed;
    private CitySeed _citySeed;
    private PlayerTournamentSeed _playerTournamentSeed;

    public DataSeed(ApiContext apiContext)
    {
      _context = apiContext;
      _playerTournamentSeed = new PlayerTournamentSeed(apiContext);
      _tournamentFactorySeed = new TournamentFactorySeed(apiContext);
      _playerSeed = new PlayerSeed(apiContext);
      _tennisCenterSeed = new TennisCenterSeed(apiContext);
      _citySeed = new CitySeed(apiContext);
    }

    public void SeedData()
    {
      _context.Database.EnsureDeleted();
      _context.Database.EnsureCreated();

      if (!_context.Cities.Any())
        SeedCities();

      if (!_context.TennisCenters.Any())
        SeedTennisCenters();

      if (!_context.Players.Any())
        SeedPlayers();

      if (!_context.Tournaments.Any())
      {
        SeedTournaments();
        SeedPlayerTournament();
      }
    }

    private void SeedCities()
    {
      foreach (var city in _context.Cities)
      {
        _context.Cities.Add(city);
      }

      _context.SaveChanges();
    }

    private void SeedTennisCenters()
    {
      var list = _tennisCenterSeed.GetList();

      foreach (var center in list)
        _context.TennisCenters.Add(center);

      _context.SaveChanges();
    }

    private void SeedTournaments()
    {
      var factoryList = _tournamentFactorySeed.GetList();
      foreach (var factory in factoryList)
      {
        factory.SetDate();
        var result = factory.Generate();
        foreach (var tournament in result)
        {
          _context.Tournaments.Add(tournament);
        }
      }

      _context.SaveChanges();
    }

    private void SeedPlayers()
    {
      var playerList = _playerSeed.GetExcelList();
      foreach (var player in playerList)
      {
        _context.Players.Add(player);
      }

      _context.SaveChanges();
    }


    private void SeedPlayerTournament()
    {
      var list = _playerTournamentSeed.GetList();
      foreach (var playerTournament in list)
      {
        _context.Tournaments.Update(playerTournament);
      }

      _context.SaveChanges();
    }
  }
}
