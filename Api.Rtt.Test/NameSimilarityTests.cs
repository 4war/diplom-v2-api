using System.Collections.Generic;
using System.Linq;
using Api.Rtt.Helpers;
using MoreLinq;
using NUnit.Framework;

namespace Api.Rtt.Test
{
  [TestFixture]
  public class NameSimilarityTests
  {
    private readonly HashSet<string> _dBTournamentNames = new HashSet<string>()
    {
      "Зимний Кубок ФТСО",
      "Первенство Самарской области",
      "Весенний кубок МОО ФТ г.о.Самара",
      "Турнир на призы Tecnifibre",
      "Зимний Кубок МОО ФТ г. о. Самара",
      "САМАРСКИЙ КУБОК",
      "Мемориал А.Япрынцева",
      "Звёздочки Тольятти - 1"
    };

    private readonly HashSet<string> _realTournamentNames = new HashSet<string>()
    {
      "\"Зимний Кубок ФТСО\"(ограничение участия по принадлежности к региону), Тольятти, Самарская",
      "\"Зимний Кубок ФТСО\"(ограничение участия по принадлежности к региону)",
      "\"Зимний Кубок ФТСО\", Тольятти, Самарская",
      "\"Зимний Кубок ФТСО\"",

      "\"Зимний кубок МОО ФТ г.о.Самара\"(ограничение участия по принадлежности к региону), Самара, Самарская",
      "\"Зимний кубок МОО ФТ\"(ограничение участия по принадлежности к региону), Самара, Самарская",
      "\"Зимний кубок МОО ФТ г.о.Самара\"(ограничение участия по принадлежности к региону)",
      "\"Зимний кубок МОО ФТ г.о.Самара\", Самара, Самарская",
      "\"Зимний кубок МОО ФТ г.о.Самара\"",
      "\"Зимний кубок МОО ФТ\"",

      "\"Звёздочки Тольятти - 1\"(ограничение участия по принадлежности к региону), Тольятти, Самарская",
      "\"Звёздочки Тольятти - 1\"",
      "\"Звёздочки Тольятти - 1\"(ограничение участия по принадлежности к региону)",
      "\"Звёздочки Тольятти - 1\", Тольятти, Самарская",

      "Турнир на призы Tecnifibre (ограничение участия по принадлежности к региону), Самара, Самарская",
      "Турнир на призы Tecnifibre (ограничение участия по принадлежности к региону)",
      "Турнир на призы Tecnifibre, Самара, Самарская",
      "Турнир на призы Tecnifibre",

      "Первенство Самарской области (ограничение участия по принадлежности к региону), Тольятти, Самарская",
      "Первенство Самарской области (ограничение участия по принадлежности к региону)",
      "Первенство Самарской области, Тольятти, Самарская",
      "Первенство Самарской области",
    };

    [SetUp]
    public void Setup()
    {
    }


    [Test]
    public void WeAreNotTheSame()
    {
      var x = "Весенний кубок МОО ФТ г.о.Самара";
      var y = "Зимний Кубок МОО ФТ г. о. Самара";
      Assert.IsFalse(x.IsSimilarTo(y, 0.7));
    }

    [Test]
    public void SeedBoolTest()
    {
      foreach (var realTournamentName in _realTournamentNames)
      {
        var flag = false;
        foreach (var dBTournamentName in _dBTournamentNames)
        {
          if (dBTournamentName.IsSimilarTo(realTournamentName, 0.7))
          {
            flag = true;
            break;
          }
        }

        if (!flag)
        {
          Assert.Fail();
        }
      }
    }

    [Test]
    public void SeedPrecisionCompareTest()
    {
      var result = new List<string>();
      var expectedBestList = new List<string>()
      {
        "Зимний Кубок ФТСО", "Зимний Кубок ФТСО", "Зимний Кубок ФТСО", "Зимний Кубок ФТСО",
        "Зимний Кубок МОО ФТ г. о. Самара", "Зимний Кубок МОО ФТ г. о. Самара", "Зимний Кубок МОО ФТ г. о. Самара",
        "Зимний Кубок МОО ФТ г. о. Самара", "Зимний Кубок МОО ФТ г. о. Самара", "Зимний Кубок МОО ФТ г. о. Самара",
        "Звёздочки Тольятти - 1", "Звёздочки Тольятти - 1", "Звёздочки Тольятти - 1", "Звёздочки Тольятти - 1",
        "Турнир на призы Tecnifibre", "Турнир на призы Tecnifibre", "Турнир на призы Tecnifibre", "Турнир на призы Tecnifibre",
        "Первенство Самарской области", "Первенство Самарской области", "Первенство Самарской области", "Первенство Самарской области",
      };

      foreach (var realTournamentName in _realTournamentNames)
      {
        var best = _dBTournamentNames
          .Select(dbName =>
            new { X = dbName, Y = realTournamentName, Precision = dbName.GetSimilarityTo(realTournamentName) })
          .MaxBy(pair => pair.Precision)
          .FirstOrDefault()
          .X;

        result.Add(best);
      }

      Assert.AreEqual(expectedBestList, result);
    }
  }
}
