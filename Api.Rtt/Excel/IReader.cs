using System.Collections.Generic;

namespace Api.Rtt.Excel
{
  public interface IReader<T>
  {
    public List<T> Copy();
    public List<T> Copy(string path);
  }
}
