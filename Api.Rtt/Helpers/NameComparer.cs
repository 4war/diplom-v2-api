﻿#nullable enable
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

      var tokensX = Regex.Split(x, "\\W");
      var tokensY = Regex.Split(y, "\\W");

      var hashSetX = new HashSet<string>(tokensX);
      var hashSetY = new HashSet<string>(tokensY);

      var intersection = hashSetX.Intersect(hashSetY).ToList();
      var maxCountHashSet = Math.Max(hashSetX.Count, hashSetY.Count);
      var similarity = (double)intersection.Count / maxCountHashSet;
      return similarity > precision;
    }
  }
}