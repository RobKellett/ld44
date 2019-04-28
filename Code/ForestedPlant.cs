using Godot;
using System;
using LD44.Utilities;

public class ForestedPlant : BaseWorldObject {
    
  public float GROWTH_TIMER = 0f;
  public float GROWTH_PROBABILITY = 0f;
  public PlantType type;

  public float _superGrowthTimeRemaining = 0f;
  public float _timeSinceGrowth = 0f;
  protected bool _asleep = false;
  protected bool _supergrowth = false;

  protected virtual void UpdateGrowthRates(float delta) {}

  public override void _Process(float delta) {
    _timeSinceGrowth += delta;
    if(_superGrowthTimeRemaining > 0) {
      _superGrowthTimeRemaining -= delta;
      _supergrowth = _superGrowthTimeRemaining > 0;
    }
    UpdateGrowthRates(delta);

    var parent = GetParent() as Map;
    while(!_asleep && _timeSinceGrowth >= GROWTH_TIMER) {
      _timeSinceGrowth -= GROWTH_TIMER;
      int spawnX = 0, spawnY = 0;
      int prob = 1;
      // Reservoir sample a random free neighbor
      if(parent.IsAllowedAtPoint(type, CellX - 1, CellY)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = -1; spawnY = 0;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(type, CellX, CellY - 1)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 0; spawnY = -1;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(type, CellX + 1, CellY)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 1; spawnY = 0;
          prob++;
        }
      }
      if(parent.IsAllowedAtPoint(type, CellX, CellY + 1)) {
        if(RNG.Instance.Next(0, prob) < 1) {
          spawnX = 0; spawnY = 1;
          prob++;
        }
      }

      if(spawnX != 0 || spawnY != 0) {
        var shouldGrow = RNG.Instance.NextDouble() < GROWTH_PROBABILITY;
        if(!shouldGrow) continue;
        parent.AddPlant(CellX + spawnX, CellY + spawnY, type, _superGrowthTimeRemaining);
      } else {
        _asleep = true;
      }
    }
  }
}