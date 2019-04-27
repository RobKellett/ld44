using Godot;
using System;
using LD44.Utilities;

public class Tree : ForestedPlant
{ 
  private bool _grown = false;
  private ResourceSprite _sprite;
  private Texture _smallTreeTexture;
  private Texture _bigTreeTexture;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
    var textureIdx = RNG.Instance.Next(2);
    _smallTreeTexture = (Texture)GD.Load($"res://Assets/tree-small{textureIdx}.png");
    _bigTreeTexture = (Texture)GD.Load($"res://Assets/tree{textureIdx}.png");

    _sprite = GetChild<ResourceSprite>(0);
    _sprite.SetTexture(_smallTreeTexture);
    _sprite.RandomizePosition();
    // Stagger all tree growth uniformly across the range defined by GROWTH_TIMER so that things are more organic
    // Also make sure that everything "grows" the first time, so we have correct adult/child trees
    GROWTH_TIMER = 15f;
    _timeSinceGrowth = (float)(RNG.Instance.NextDouble() + 1) * GROWTH_TIMER;
    GROWTH_PROBABILITY = 0.3f;
    Type = PlantType.Tree;
  }

  public override void _Process(float delta) {
    base._Process(delta);
    if(_asleep && !_grown) {
      _grown = true;
      _sprite.SetTexture(_bigTreeTexture);
    }
  }
}
