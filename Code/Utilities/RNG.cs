using System;

namespace LD44.Utilities
{
  public static class RNG
  {
    public static Random Instance { get; } = new Random();
  }
}