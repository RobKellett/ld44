using System;
using Godot;

namespace LD44.Resources
{
  public interface IResource
  {
    Vector2 GetClosestPosition(Vector2 yourPosition);
  }
}