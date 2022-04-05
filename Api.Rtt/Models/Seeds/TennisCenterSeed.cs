using System.Collections.Generic;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class TennisCenterSeed
  {
    private ApiContext _context;

    public TennisCenterSeed(ApiContext context)
    {
      _context = context;
    }

    public List<TennisCenter> GetList()
    {
      var list = new List<TennisCenter>()
      {
        new() { Name = "ДЮСШ №1", Address = "ул. Ново-Садовая, 32А, Самара, Самарская обл., 443110", City = "Самара" },
        new() { Name = "Тригон", Address = "ул. Дачная, 4А, Самара, Самарская обл., 443096", City = "Самара" },
        new()
        {
          Name = "СДЮСШОР по теннису", Address = "Шушенская ул., Самара, Самарская обл., 443011", City = "Самара"
        },
        new()
        {
          Name = "Тольятти Теннис Центр", Address = "ул. Баныкина, 19А, Тольятти, Самарская обл., 445021",
          City = "Тольятти"
        },
        new() { Name = "Davis", Address = "ул. Спортивная, 19, Тольятти, Самарская обл., 445057", City = "Тольятти" },
      };

      return list;
    }
  }
}
