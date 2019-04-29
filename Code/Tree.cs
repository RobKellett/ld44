using Godot;
using System;
using LD44.Resources;
using LD44.Utilities;

public interface ICareAboutMapUpdates {
  void MapUpdated();
  void MapCellUpdated(int x, int y, GroundType ground);
}

public class Tree : ForestedPlant, IBuildingMaterialSource
{ 
  private bool _grown = false;
  private AnimationPlayer _animationPlayer;
  public bool HasBuildingMaterial { get; private set; } = true;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
    
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
    GROWTH_TIMER = 0f;
    _timeSinceGrowth = 0f;
    GROWTH_PROBABILITY = 0f;
    type = PlantType.Tree;
    UpdateGrowthRates(0f);
  }

  protected override void UpdateGrowthRates(float delta) {
    // Our growth rate depends on the land type
    var target_growth_timer = 0f;
    var target_growth_prob = 0f;
    var map = GetParent() as Map;
    if(map._land[CellX, CellY] == GroundType.Dirt) {
      target_growth_timer = 20f;
      target_growth_prob = 0.2f;
    }
    else if(map._land[CellX, CellY] == GroundType.Grass) {
      target_growth_timer = 12f;
      target_growth_prob = 0.4f;
    }
        
    for(int dx = -3; dx < 3; dx++) {
      for(int dy = -3; dy < 3; dy++) {
        if(!map.IsWithinBounds(CellX + dx, CellY + dy)) continue;
        var neighbor = map._land[CellX + dx, CellY + dy];
        if(neighbor == GroundType.Water) {
          target_growth_timer = Math.Max(GROWTH_TIMER, 7f);
          target_growth_prob = Math.Max(GROWTH_PROBABILITY, 0.6f);
          break;
        }
      }
    }
    if(_supergrowth) {
      target_growth_timer *= 0.3f;
      target_growth_prob = 1f;
    } 
    if(GROWTH_TIMER == 0) {
      _timeSinceGrowth = (float)RNG.Instance.NextDouble() * target_growth_timer;
    }
    GROWTH_TIMER = target_growth_timer;
    GROWTH_PROBABILITY = target_growth_prob;
  }

  public override void _Process(float delta) {
    base._Process(delta);
    if(!_grown && !HasFreeNeighbor()) {
      _grown = true;
      _animationPlayer.Play("Grow");
    }
  }

  public bool HasFreeNeighbor() {
      // Reservoir sample a random free neighbor
    var parent = GetParent<Map>();
    return
      parent.IsAllowedAtPoint(type, CellX - 1, CellY) ||
      parent.IsAllowedAtPoint(type, CellX, CellY - 1) ||
      parent.IsAllowedAtPoint(type, CellX + 1, CellY) ||
      parent.IsAllowedAtPoint(type, CellX, CellY + 1);
  }

  public Vector2 GetClosestPosition(Vector2 yourPosition)
  {
    return Position;
  }

  public void TakeBuildingMaterial(Human human)
  {
    human.BuildingMaterials++;
    
    Visible = false;
    HasBuildingMaterial = false;
    _animationPlayer.Play("resource_harvest");
  }
}
