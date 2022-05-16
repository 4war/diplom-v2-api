using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities.Authorization;
using JWT;
using JWT.Algorithms;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Rtt.Controllers.Authorization
{
  [Route("api/[controller]")]
  public class AuthorizationController : Controller
  {
    private readonly ApiContext _context;
    private readonly IOptions<AuthOptions> _authOptions;

    public AuthorizationController(ApiContext context, IOptions<AuthOptions> authOptions)
    {
      _context = context;
      _authOptions = authOptions;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] Credentials credentials)
    {
      var passwordHashcode = credentials.Password.GetHashCode();
      var contextAccount =
        _context.Accounts.FirstOrDefault(x => x.Email == credentials.Email);

      if (contextAccount is not null) return BadRequest(contextAccount);

      _context.Accounts.Add(new Account
      {
        Id = 0,
        Email = credentials.Email,
        Password = passwordHashcode,
        Roles = "user",
      });
      _context.SaveChanges();
      return Ok(credentials);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Credentials credentials)
    {
      var account = AuthenticateUser(credentials.Email, credentials.Password);
      if (account is null) return Unauthorized();

      var token = GenerateJwt(account);

      return Ok(new { access_token = token });
    }

    private Account AuthenticateUser(string email, string password)
    {
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return null;
      var passwordHashCode = password.GetHashCode();
      return _context.Accounts.FirstOrDefault(x => x.Email == email && x.Password == passwordHashCode);
    }

    private string GenerateJwt(Account account)
    {
      var authParams = _authOptions.Value;

      var securityKey = authParams.GetSymmetricSecurityKey();
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Email, account.Email),
        new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString())
      };

      claims.AddRange(account.Roles.Split().Select(role => new Claim("role", role)));

      var token = new JwtSecurityToken(authParams.Issuer,
        authParams.Audience,
        claims: claims,
        expires: DateTime.Now.AddSeconds(authParams.TokenLifeTime),
        signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
