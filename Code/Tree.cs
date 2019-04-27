using Godot;
using System;
using LD44.Utilities;

public class Tree : Node2D
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var textureIdx = RNG.Instance.Next(2);
    var texture = (Texture)GD.Load($"res://Assets/tree{textureIdx}.png");
    var sprite = GetChild<Sprite>(0);
    sprite.SetTexture(texture);

    const int POSITION_JITTER = 6;
    sprite.Position = new Vector2(
      RNG.Instance.Next(POSITION_JITTER) - (POSITION_JITTER / 2),
      RNG.Instance.Next(POSITION_JITTER) - (POSITION_JITTER / 2)
    );
    sprite.ZIndex = (int)Position.y;
  }
}
