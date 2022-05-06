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
        new()
        {
          Name = "ДЮСШ №1", Address = "ул. Ново-Садовая, 32А, Самара, Самарская обл., 443110", City = "Самара",
          Courts = new List<Court>()
          {
            new Court(){Name = "1", Opened = true, Surface = "hard"},
            new Court(){Name = "2", Opened = true, Surface = "hard"},
            new Court(){Name = "3", Opened = true, Surface = "hard"},
            new Court(){Name = "4", Opened = false, Surface = "hard"},
            new Court(){Name = "5", Opened = false, Surface = "hard"},
            new Court(){Name = "6", Opened = false, Surface = "hard"},
            new Court(){Name = "7", Opened = true, Surface = "hard"},
          }
        },
        new()
        {
          Name = "Тригон", Address = "ул. Дачная, 4А, Самара, Самарская обл., 443096", City = "Самара",
          Courts = new List<Court>()
          {
            new Court(){Name = "1", Opened = true, Surface = "hard"},
            new Court(){Name = "2", Opened = true, Surface = "hard"},
            new Court(){Name = "3", Opened = true, Surface = "hard"},
            new Court(){Name = "4", Opened = false, Surface = "hard"},
            new Court(){Name = "5", Opened = false, Surface = "hard"},
          }
        },
        new()
        {
          Name = "СДЮСШОР по теннису", Address = "Шушенская ул., Самара, Самарская обл., 443011", City = "Самара",
          Courts = new List<Court>()
          {
            new Court(){Name = "1", Opened = true, Surface = "clay"},
            new Court(){Name = "2", Opened = true, Surface = "clay"},
            new Court(){Name = "3", Opened = true, Surface = "clay"},
            new Court(){Name = "4", Opened = true, Surface = "clay"},
            new Court(){Name = "5", Opened = false, Surface = "hard"},
            new Court(){Name = "6", Opened = false, Surface = "hard"},
          }
        },
        new()
        {
          Name = "Тольятти Теннис Центр", Address = "ул. Баныкина, 19А, Тольятти, Самарская обл., 445021",
          City = "Тольятти", Courts = new List<Court>()
          {
            new Court(){Name = "1", Opened = false, Surface = "hard"},
            new Court(){Name = "2", Opened = false, Surface = "hard"},
            new Court(){Name = "3", Opened = false, Surface = "hard"},
            new Court(){Name = "4", Opened = false, Surface = "hard"},
            new Court(){Name = "5", Opened = false, Surface = "hard"},
            new Court(){Name = "6", Opened = false, Surface = "hard"},
            new Court(){Name = "7", Opened = false, Surface = "hard"},
            new Court(){Name = "8", Opened = false, Surface = "hard"},
            new Court(){Name = "9", Opened = true, Surface = "hard"},
            new Court(){Name = "10", Opened = true, Surface = "hard"},
            new Court(){Name = "11", Opened = true, Surface = "hard"},
            new Court(){Name = "12", Opened = true, Surface = "hard"},
            new Court(){Name = "13", Opened = true, Surface = "hard"},
            new Court(){Name = "14", Opened = true, Surface = "clay"},
            new Court(){Name = "15", Opened = true, Surface = "clay"},
          }
        },
        new()
        {
          Name = "Davis", Address = "ул. Спортивная, 19, Тольятти, Самарская обл., 445057", City = "Тольятти",
          Courts = new List<Court>()
          {
            new Court(){Name = "1", Opened = false, Surface = "hard"},
            new Court(){Name = "2", Opened = false, Surface = "hard"},
            new Court(){Name = "3", Opened = false, Surface = "hard"},
            new Court(){Name = "4", Opened = false, Surface = "hard"},
            new Court(){Name = "5", Opened = true, Surface = "hard"},
            new Court(){Name = "6", Opened = true, Surface = "hard"},
            new Court(){Name = "7", Opened = true, Surface = "hard"},
            new Court(){Name = "8", Opened = true, Surface = "hard"},
            new Court(){Name = "9", Opened = true, Surface = "hard"},
            new Court(){Name = "10", Opened = true, Surface = "hard"},
            new Court(){Name = "11", Opened = true, Surface = "clay"},
            new Court(){Name = "12", Opened = true, Surface = "clay"},
          }
        },
      };

      return list;
    }
  }
}
