using System.Collections.Generic;
using Api.Rtt.Models;
using Api.Rtt.Models.Entities;
using NUnit.Framework;

namespace Excel
{
  [TestFixture]
  public class Tests
  {
    private PlayerReader _playerReader = new PlayerReader();

    [Test]
    public void FirstOpenTest()
    {
      var actual = _playerReader.Copy();
      var expected = new Dictionary<int, Player>();
      Assert.AreNotEqual(expected, actual);
    }

    [Test]
    public void RunSamara()
    {
      var dictionary = _playerReader.Copy();
    }
  }
}
