using Godot;
using System;
using LD44.Utilities;

public class Bush : FruitedPlant
{
  private Texture noFruitTexture;
  private Texture fruitTexture;

  public override void _Ready()
  {
    noFruitTexture = (Texture)GD.Load("res://Assets/bush.png");
    var textureIdx = RNG.Instance.Next(3);
    fruitTexture = (Texture)GD.Load($"res://Assets/bush-fruit{textureIdx}.png");

    var sprite = GetChild<ResourceSprite>(0);
    sprite.SetTexture(noFruitTexture);
    sprite.RandomizePosition();

    FRUIT_TIMER = 15f;
    _timeSinceFruitPicked = (float)RNG.Instance.NextDouble() * FRUIT_TIMER;
  }
  protected override void Bloom() {
      base.Bloom();
      var sprite = GetChild<ResourceSprite>(0);
      sprite.SetTexture(fruitTexture);
  }
  public override void Pick() {
      base.Pick();
      var sprite = GetChild<ResourceSprite>(0);
      sprite.SetTexture(noFruitTexture);
  }
}
