using Godot;
using System;

public class Mushroom : BasePlant
{
  public override void _Ready()
  {
    foreach (var name in new[] { "Sprite1", "Sprite2", "Sprite3" })
    {
      var sprite = GetNode<ResourceSprite>(name);
      sprite.LoadRandomTexture("mushroom", 6);
      sprite.RandomizePosition(4);
    }
  }
}
