using Godot;
using System;
using LD44.Resources;

public class FruitTree : FruitedPlant, IBuildingMaterialSource
{
  private Texture _noFruitTexture;
  private Texture _fruitTexture;
  private ResourceSprite _sprite;
  private AnimationPlayer _animationPlayer;
  protected override int FoodValue { get; } = 5;
  public bool HasBuildingMaterial { get; private set; } = true;

  public override void _Ready()
  {
    base._Ready();

    FRUIT_TIMER = 30f;

    _sprite = GetChild<ResourceSprite>(0);
    _noFruitTexture = (Texture)GD.Load("res://Assets/fruittree.png");
    _fruitTexture = (Texture)GD.Load("res://Assets/fruittree-fruit.png");
    _animationPlayer = GetChild<AnimationPlayer>(1);

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

  public void TakeBuildingMaterial(Human human)
  {
    base.TakeFood(human);
    human.BuildingMaterials++;

    HasBuildingMaterial = false;
    _canBearFruit = false;
    _animationPlayer.Play("resource_harvest");
  }
}
