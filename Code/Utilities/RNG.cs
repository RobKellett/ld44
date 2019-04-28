using System;
using System.Collections.Generic;

namespace LD44.Utilities
{
  public static class RNG
  {
    public static Random Instance { get; } = new Random();

    public static void Shuffle<T>(IList<T> list)
    {
      for (var i = 0; i < list.Count - 1; ++i)
      {
        var j = Instance.Next(i, list.Count);
        var tmp = list[i];
        list[i] = list[j];
        list[j] = tmp;
      }
    }
  }
}