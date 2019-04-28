using Godot;
using System;
using LD44.Utilities;

public class Bush : FruitedPlant
{
  private Texture _noFruitTexture;
  private Texture _fruitTexture;
  private ResourceSprite _sprite;

  public override void _Ready()
  {
    base._Ready();
    _noFruitTexture = (Texture)GD.Load("res://Assets/bush.png");
    var textureIdx = RNG.Instance.Next(3);
    _fruitTexture = (Texture)GD.Load($"res://Assets/bush-fruit{textureIdx}.png");

    _sprite = GetChild<ResourceSprite>(0);
    _sprite.SetTexture(_noFruitTexture);
    _sprite.RandomizePosition();

    FRUIT_TIMER = 15f;
    _timeSinceFruitPicked = (float)RNG.Instance.NextDouble() * FRUIT_TIMER;
  }
  protected override void Bloom() {
      base.Bloom();
      _sprite.SetTexture(_fruitTexture);
  }
  public override void Pick() {
      base.Pick();
      _sprite.SetTexture(_noFruitTexture);
  }
}
