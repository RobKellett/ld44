using Godot;
using System;
using LD44.Utilities;

public class ResourceSprite : Sprite
{
  public void Load(string assetName, int variants)
  {
    var textureIdx = RNG.Instance.Next(variants);
    var texture = (Texture)GD.Load($"res://Assets/{assetName}{textureIdx}.png");
    SetTexture(texture);

    const int POSITION_JITTER = 6;
    Position = new Vector2(
      RNG.Instance.Next(POSITION_JITTER) - (POSITION_JITTER / 2),
      RNG.Instance.Next(POSITION_JITTER) - (POSITION_JITTER / 2)
    );
    ZIndex = (int)Position.y;
  }
}
