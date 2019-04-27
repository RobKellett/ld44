using Godot;
using System;

public class Cactus : BasePlant
{
  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    sprite.LoadRandomTexture("cactus", 5);
    sprite.RandomizePosition();
  }
}
