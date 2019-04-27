using Godot;
using System;

public class Cactus : Node2D
{
  public override void _Ready()
  {
    GetChild<ResourceSprite>(0).Load("cactus", 5);
  }
}
