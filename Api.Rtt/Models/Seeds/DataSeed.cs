﻿using System;
using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Helpers;
using Api.Rtt.Models.Entities;
using Api.Rtt.Models.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

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
    private readonly ScheduleSeed _scheduleSeed;

    private readonly BracketBuilder _bracketBuilder = new BracketBuilder();

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
      _scheduleSeed = new ScheduleSeed(apiContext);
    }

    public void SeedData()
    {
      //_context.Database.EnsureDeleted();
      //_context.Database.EnsureCreated();

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
        SeedBrackets();
      }

      if (!_context.Matches.Any())
        SeedMatches();

      if (!_context.Schedules.Any())
        SeedSchedules();

      if (!_context.Accounts.Any())
        SeedAccounts();

      if (!_context.TestResults.Any())
        SeedTestResults();
    }

    private void SeedCities()
    {
      _context.Cities.Add(new City() { Name = "Самара" });
      _context.Cities.Add(new City() { Name = "Тольятти" });

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
        factory.Tournaments = result;

        _context.TournamentFactories.Add(factory);
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

      var addedBrackets = new HashSet<int>();
      foreach (var bracket in list)
      {
        _context.Brackets.Add(bracket);
        addedBrackets.Add(bracket.TournamentId);
      }

      foreach (var factory in _context.TournamentFactories)
      {
        var brackets = _bracketBuilder.AttachBracketsForFactory(factory, addedBrackets);
        foreach (var bracket in brackets)
        {
          _context.Brackets.Add(bracket);
        }
      }

      _context.SaveChanges();
    }

    private void SeedSchedules()
    {
      var list = _scheduleSeed.GetList();

      foreach (var schedule in list)
        _context.Schedules.Add(schedule);

      _context.SaveChanges();
    }

    private void SeedAccounts()
    {
      var list = new List<Account>()
      {
        new Account()
        {
          Email = "fedro_777@mail.ru", Password = "conterhome2002".PasswordToHashCode(),
          Roles = string.Join(" ", new List<string>() { "user", "org", "admin" })
        },
        new Account() { Email = "a@a.ru", Password = "a".PasswordToHashCode(), Roles = "user" },
        new Account() { Email = "b@b.ru", Password = "b".PasswordToHashCode(), Roles = "user" },
        new Account() { Email = "c@c.ru", Password = "c".PasswordToHashCode(), Roles = "user" },
      };

      foreach (var account in list)
      {
        _context.Accounts.Add(account);
      }

      _context.SaveChanges();
    }

    private void SeedTestResults()
    {
      var list = new List<PsychTestResult>()
      {
        new PsychTestResult
        {
          Rni = 33188,
          Defensive = 70,
          Active = 21,
          Reactive = 30,
          Moral = 80,
          LastTimeCompleted = new DateTime(2021, 1, 14),
        },
      };

      foreach (var testResult in list)
      {
        _context.TestResults.Add(testResult);
      }

      _context.SaveChanges();
    }
  }
}
