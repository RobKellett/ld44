using System;

namespace LD44.Resources
{
  public interface IWaterSource : IResource
  {
    bool HasWater { get; }
    void TakeWater(Human human);
  }
}