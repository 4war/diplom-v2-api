using System;
using System.Collections.Generic;
using System.IO;
using _excel = Microsoft.Office.Interop.Excel;

namespace Excel
{
  public class Player
  {
    public Dictionary<int, Player> Copy()
    {
      var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

      return Copy(path);
    }

    public Dictionary<int, Player> Copy(string path)
    {
      //todo: open
      var app = new _excel.ApplicationClass();
      var workBook = app.Workbooks.Open(path);
      var workSheet = (_excel.Worksheet)(workBook.Worksheets[1]);

      //todo: read
      var result = new Dictionary<int, Player>();


      //todo: close
      workBook.Close();
      app.Quit();
      System.Runtime.InteropServices.Marshal.FinalReleaseComObject(app);

      return result;
    }
  }
}
