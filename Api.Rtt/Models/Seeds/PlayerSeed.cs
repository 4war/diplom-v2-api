using System;
using System.Collections.Generic;
using Api.Rtt.Models.Entities;

namespace Api.Rtt.Models.Seeds
{
  public class PlayerSeed
  {

    private ApiContext _context;

    public PlayerSeed(ApiContext context)
    {
      _context = context;
    }

    public List<Player> GetList()
    {
      var list = new List<Player>();

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 5, 9),
        Gender = 0,
        Surname = "Лукьянов",
        Name = "Матвей",
        Patronymic = "Денисович",
        Rni = 40054,
        Point = 376,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 6, 10),
        Gender = 0,
        Surname = "Багателия",
        Name = "Павел",
        Patronymic = "Юрьевич",
        Rni = 40177,
        Point = 190,
      });

      list.Add(new Player()
      {
        City = "Самара",
        DateOfBirth = new DateTime(2009, 10, 19),
        Gender = 0,
        Surname = "Иваков",
        Name = "Роман",
        Patronymic = "Алексеевич",
        Rni = 41868,
        Point = 151,
      });

      list.Add(new Player()
      {
        City = "Самара",
        DateOfBirth = new DateTime(2010, 6, 17),
        Gender = 0,
        Surname = "Шаридода",
        Name = "Роман",
        Patronymic = "Алексеевич",
        Rni = 43053,
        Point = 491,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 8, 7),
        Gender = 0,
        Surname = "Колесников",
        Name = "Владислав",
        Patronymic = "Аркадьевич",
        Rni = 42253,
        Point = 403,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 7, 26),
        Gender = 0,
        Surname = "Соколов",
        Name = "Макар",
        Patronymic = "Александрович",
        Rni = 40132,
        Point = 202,
      });

      list.Add(new Player()
      {
        City = "Самара",
        DateOfBirth = new DateTime(2010, 5, 22),
        Gender = 0,
        Surname = "Огарков",
        Name = "Егор",
        Patronymic = "Сергеевич",
        Rni = 42264,
        Point = 276,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 7, 17),
        Gender = 0,
        Surname = "Китев",
        Name = "Богдан",
        Patronymic = "Олегович",
        Rni = 40176,
        Point = 174,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 7, 17),
        Gender = 0,
        Surname = "Китев",
        Name = "Богдан",
        Patronymic = "Олегович",
        Rni = 40176,
        Point = 174,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 6, 7),
        Gender = 0,
        Surname = "Ермолаев",
        Name = "Артур",
        Patronymic = "Владимирович",
        Rni = 42332,
        Point = 110,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 4, 25),
        Gender = 0,
        Surname = "Данилов",
        Name = "Глеб",
        Patronymic = "Александрович",
        Rni = 42331,
        Point = 252,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 11, 18),
        Gender = 0,
        Surname = "Нуриев",
        Name = "Семур",
        Patronymic = "Эхтибарович",
        Rni = 43548	,
        Point = 80,
      });

      list.Add(new Player()
      {
        City = "Самара",
        DateOfBirth = new DateTime(2010, 6, 2),
        Gender = 0,
        Surname = "Аксенов",
        Name = "Денис",
        Patronymic = "Дмитриевич",
        Rni = 42566,
        Point = 135,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 8, 26),
        Gender = 0,
        Surname = "Гайлюс",
        Name = "Максим",
        Patronymic = "Викторович",
        Rni = 43240,
        Point = 156,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2009, 4, 13),
        Gender = 0,
        Surname = "Галин",
        Name = "Руслан",
        Patronymic = "Марселевич",
        Rni = 40030,
        Point = 99,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 3, 26),
        Gender = 0,
        Surname = "Деркин",
        Name = "Ярослав",
        Patronymic = "Дмитриевич",
        Rni = 42341,
        Point = 12,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 12, 23),
        Gender = 0,
        Surname = "Косарев",
        Name = "Алексей",
        Patronymic = "Петрович",
        Rni = 42219,
        Point = 56,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 11, 3),
        Gender = 0,
        Surname = "Чиненков",
        Name = "Герман",
        Patronymic = "Денисович",
        Rni = 42445,
        Point = 7,
      });

      list.Add(new Player()
      {
        City = "Тольятти",
        DateOfBirth = new DateTime(2010, 2, 9),
        Gender = 0,
        Surname = "Тё",
        Name = "Дмитрий",
        Patronymic = "Михайлович",
        Rni = 42433,
        Point = 19,
      });

      list.Add(new Player()
      {
        City = "Самара",
        DateOfBirth = new DateTime(2010, 3, 30),
        Gender = 0,
        Surname = "Авдонин",
        Name = "Матвей",
        Patronymic = "Семенович",
        Rni = 45363,
        Point = 125,
      });

      return list;
    }
  }
}
