using Godot;
using System;
using LD44.Utilities;

public class ResourceSprite : Sprite
{
  public void LoadRandomTexture(string assetName, int variants)
  {
    var textureIdx = RNG.Instance.Next(variants);
    var texture = (Texture)GD.Load($"res://Assets/{assetName}{textureIdx}.png");
    SetTexture(texture);
  }

  public void RandomizePosition(int jitter = 6)
  {
    Position += new Vector2(
      RNG.Instance.Next(jitter) - (jitter / 2),
      RNG.Instance.Next(jitter) - (jitter / 2)
    );
    var parentY = GetParentOrNull<Node2D>()?.Position.y ?? 0;
    ZIndex = 100 + (int)parentY + (int)Position.y;
  }
}
