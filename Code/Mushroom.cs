using Godot;
using System;

public class Mushroom : Node2D
{
  public override void _Ready()
  {
    GetNode<ResourceSprite>("Sprite1").Load("mushroom", 6, 4);
    GetNode<ResourceSprite>("Sprite2").Load("mushroom", 6, 4);
    GetNode<ResourceSprite>("Sprite3").Load("mushroom", 6, 4);
  }
}
