using System;

namespace LD44.Resources
{
  public interface IFoodSource : IResource
  {
    bool HasFood { get; }
    void TakeFood(Human human);
  }
}