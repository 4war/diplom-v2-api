#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Api.Rtt.Helpers
{
  public static class NameSimilarAnalyser
  {
    public static bool IsSimilarTo(this string x, string y, double precision)
    {
      if (precision is < 0 or > 1)
      {
        throw new ArgumentException("precision should be in [0;1]");
      }

      var tokensX = Regex.Split(x, @"\W").Where(s => s.Length > 1).ToList();
      var tokensY = Regex.Split(y, @"\W").Where(s => s.Length > 1).ToList();

      if (tokensX.First() != tokensY.First())
      {
        return false;
      }

      var hashSetX = new List<string>(tokensX);
      var hashSetY = new List<string>(tokensY);

      var intersection = hashSetX.Intersect(hashSetY).ToList();
      var maxCountHashSet = Math.Min(tokensX.Count, tokensY.Count);
      var similarity = (double)intersection.Count / maxCountHashSet;
      return similarity > precision;
    }

    public static double GetSimilarityTo(this string x, string y)
    {
      var tokensX = Regex.Split(x, "\\W");
      var tokensY = Regex.Split(y, "\\W");

      var hashSetX = new HashSet<string>(tokensX);
      var hashSetY = new HashSet<string>(tokensY);

      var intersection = hashSetX.Intersect(hashSetY).ToList();
      var maxCountHashSet = Math.Max(hashSetX.Count, hashSetY.Count);
      return (double)intersection.Count / maxCountHashSet;
    }
  }
}
