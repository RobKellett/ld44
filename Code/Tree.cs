using Godot;
using System;
using LD44.Utilities;

public class Tree : BasePlant
{
  public float GROWTH_TIMER = 1f;
  public float GROWTH_PROBABILITY = 1f;

  private float _timeSinceGrowth = 0f;
  private bool _grown = false;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    sprite.LoadRandomTexture("tree", 2);
    // Stagger all tree growth uniformly across the range defined by GROWTH_TIMER so that things are more organic
    _timeSinceGrowth = (float)RNG.Instance.NextDouble() * GROWTH_TIMER;
  }

  public override void _Process(float delta) {
    _timeSinceGrowth += delta;

    while(!_grown && _timeSinceGrowth >= GROWTH_TIMER) {
      var parent = GetParent() as Map;
      _timeSinceGrowth -= GROWTH_TIMER;
      var shouldGrow = RNG.Instance.NextDouble() < GROWTH_PROBABILITY;
      if(!shouldGrow) continue;
      GD.Print("Growing!");
      int spawnX = 0, spawnY = 0;
      int prob = 1;
      // Reservoir sample a random free neighbor
      if(parent.IsAllowedAtPoint(PlantType.Tree, CellX - 1, CellY)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = -1; spawnY = 0;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(PlantType.Tree, CellX, CellY - 1)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 0; spawnY = -1;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(PlantType.Tree, CellX + 1, CellY)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 1; spawnY = 0;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(PlantType.Tree, CellX, CellY + 1)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 0; spawnY = 1;
          prob++;
        }
      }

      if(spawnX != 0 || spawnY != 0) {
        parent.AddPlant(CellX + spawnX, CellY + spawnY, PlantType.Tree);
      } else {
        _grown = true;
      }
    }
  }
}
