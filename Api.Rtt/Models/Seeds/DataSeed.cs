﻿using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class DataSeed
  {
    private readonly ApiContext _context;
    private readonly TournamentFactorySeed _tournamentFactorySeed;
    private readonly PlayerSeed _playerSeed;
    private readonly TennisCenterSeed _tennisCenterSeed;
    private readonly CitySeed _citySeed;
    private readonly PlayerTournamentSeed _playerTournamentSeed;
    private readonly MatchSeed _matchSeed;
    private readonly BracketSeed _bracketSeed;

    public DataSeed(ApiContext apiContext)
    {
      _context = apiContext;
      _playerTournamentSeed = new PlayerTournamentSeed(apiContext);
      _tournamentFactorySeed = new TournamentFactorySeed(apiContext);
      _playerSeed = new PlayerSeed(apiContext);
      _tennisCenterSeed = new TennisCenterSeed(apiContext);
      _citySeed = new CitySeed(apiContext);
      _matchSeed = new MatchSeed(apiContext);
      _bracketSeed = new BracketSeed(apiContext);
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

      if (!_context.Matches.Any())
        SeedMatches();

      if (!_context.Brackets.Any())
        SeedBrackets();
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

    private void SeedMatches()
    {
      var list = _matchSeed.GetList();
      foreach (var match in list)
      {
        _context.Matches.Add(match);
      }

      _context.SaveChanges();
    }

    private void SeedBrackets()
    {
      var list = _bracketSeed.GetList();

      foreach (var bracket in list)
        _context.Brackets.Add(bracket);

      _context.SaveChanges();
    }
  }
}
