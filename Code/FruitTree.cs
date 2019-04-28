using Godot;
using System;

public class FruitTree : FruitedPlant
{
  private Texture _noFruitTexture;
  private Texture _fruitTexture;
  private ResourceSprite _sprite;

  public override void _Ready()
  {
    base._Ready();

    FRUIT_TIMER = 30f;

    _sprite = GetChild<ResourceSprite>(0);
    _noFruitTexture = (Texture)GD.Load("res://Assets/fruittree.png");
    _fruitTexture = (Texture)GD.Load("res://Assets/fruittree-fruit.png");

    _sprite.SetTexture(_noFruitTexture);
    _sprite.RandomizePosition();
  }

  protected override void Bloom()
  {
    base.Bloom();
    _sprite.SetTexture(_fruitTexture);
  }

  public override void Pick()
  {
    base.Pick();
    _sprite.SetTexture(_noFruitTexture);
  }
}
