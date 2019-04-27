using Godot;
using LD44.Utilities;
using System;

public class Mushroom : ForestedPlant
{
  public override void _Ready()
  {
    base._Ready();
    foreach (var name in new[] { "Sprite1", "Sprite2", "Sprite3" })
    {
      var sprite = GetNode<ResourceSprite>(name);
      sprite.LoadRandomTexture("mushroom", 6);
      sprite.RandomizePosition(4);
    }
    // Stagger all tree growth uniformly across the range defined by GROWTH_TIMER so that things are more organic
    // Also make sure that everything "grows" the first time, so we have correct adult/child trees
    GROWTH_TIMER = 3f;
    _timeSinceGrowth = (float)(RNG.Instance.NextDouble() + 1) * GROWTH_TIMER;
    GROWTH_PROBABILITY = 0.1f;
    Type = PlantType.Mushroom;
  }
}
