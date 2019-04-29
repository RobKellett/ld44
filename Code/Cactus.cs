using Godot;
using System;
using LD44.Resources;
using LD44.Utilities;

public class Cactus : BaseWorldObject, IWaterSource, IFoodSource
{
  public bool HasWater { get; private set; } = true;
  public bool HasFood { get; private set; } = true;
  private AnimationPlayer _animationPlayer;

  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    _animationPlayer = GetChild<AnimationPlayer>(1);
    sprite.LoadRandomTexture("cactus", 5);
    sprite.RandomizePosition();
    Group.WaterSources.Add(this);
    Group.FoodSources.Add(this);
  }

  public Vector2 GetClosestPosition(Vector2 yourPosition)
  {
    return Position;
  }

  private void Harvest(Human human)
  {
    if (!HasFood || !HasWater)
    {
      return;
    }

    human.Drink();
    human.Feed(3);
    Group.Humans.Call(GetTree(), h => h.ResourceDestroyed(this, human));

    HasFood = false;
    HasWater = false;
    _animationPlayer.Play("resource_harvest");
  }

  public void TakeWater(Human human)
  {
    Harvest(human);
  }

  public void TakeFood(Human human)
  {
    Harvest(human);
  }
}
