using Godot;
using System;
using LD44.Utilities;

public interface ICareAboutMapUpdates {
  void MapUpdated();
}

public class Tree : ForestedPlant, ICareAboutMapUpdates
{ 
  private bool _grown = false;
  private AnimationPlayer _animationPlayer;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
    Group.MapUpdates.Add(this);

    var textureIdx = RNG.Instance.Next(2);
    var smallTreeTexture = (Texture)GD.Load($"res://Assets/tree-small{textureIdx}.png");
    var bigTreeTexture = (Texture)GD.Load($"res://Assets/tree{textureIdx}.png");

    var smallSprite = GetChild<ResourceSprite>(0);
    var bigSprite = GetChild<Sprite>(1);
    smallSprite.SetTexture(smallTreeTexture);
    bigSprite.SetTexture(bigTreeTexture);
    smallSprite.RandomizePosition();
    bigSprite.Position = smallSprite.Position;
    bigSprite.ZIndex = smallSprite.ZIndex;

    _animationPlayer = GetChild<AnimationPlayer>(2);
    _animationPlayer.Play("Spawn");

    // Stagger all tree growth uniformly across the range defined by GROWTH_TIMER so that things are more organic
    // Also make sure that everything "grows" the first time, so we have correct adult/child trees
    GROWTH_TIMER = 15f;
    _timeSinceGrowth = (float)(RNG.Instance.NextDouble() + 1) * GROWTH_TIMER;
    GROWTH_PROBABILITY = 0.3f;
    type = PlantType.Tree;
    MapUpdated();
  }

  public void MapUpdated() {
    // Our growth rate depends on the land type
    GROWTH_TIMER = 0f;
    GROWTH_PROBABILITY = 0f;
    var map = GetParent() as Map;
    if(map._land[CellX, CellY] == GroundType.Dirt) {
      GROWTH_TIMER = 20f;
      GROWTH_PROBABILITY = 0.2f;
    }
    else if(map._land[CellX, CellY] == GroundType.Grass) {
      GROWTH_TIMER = 12f;
      GROWTH_PROBABILITY = 0.4f;
    }
        
    for(int dx = -3; dx < 3; dx++) {
      for(int dy = -3; dy < 3; dy++) {
        if(!map.IsWithinBounds(CellX + dx, CellY + dy)) continue;
        var neighbor = map._land[CellX + dx, CellY + dy];
        if(neighbor == GroundType.Water) {
          GROWTH_TIMER = Math.Max(GROWTH_TIMER, 7f);
          GROWTH_PROBABILITY = Math.Max(GROWTH_PROBABILITY, 0.6f);
          break;
        }
      }
    }
  }

  public override void _Process(float delta) {
    base._Process(delta);
    if(_asleep && !_grown) {
      _grown = true;
      _animationPlayer.Play("Grow");
    }
  }
}
