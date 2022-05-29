using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Api.Rtt.Helpers
{
  public static class PasswordHashCodeGenerator
  {
    public static string PasswordToHashCode(this string password)
    {
      using var sha256 = SHA256.Create();
      var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

      var sb = new StringBuilder();
      foreach (var b in bytes)
      {
        sb.Append(b.ToString("x2"));
      }

      return sb.ToString();
    }
  }
}
