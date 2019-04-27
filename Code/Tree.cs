using Godot;
using System;
using LD44.Utilities;

public class Tree : Node2D
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    GetChild<ResourceSprite>(0).Load("tree", 2);
  }
}
