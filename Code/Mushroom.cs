using Godot;
using System;

public class Mushroom : Node2D
{
  public override void _Ready()
  {
    GetChild<ResourceSprite>(0).Load("mushroom", 6);
  }
}
