using Godot;
using System;

public class MapRenderer : TileMap
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
  }

  public void DoRender(Map map)
  {
    for (var y = 0; y < map.MAP_HEIGHT; ++y)
    {
      for (var x = 0; x < map.MAP_WIDTH; ++x)
      {
        var tile = map._land[x, y];
        this.SetCell(x, y, GroundTypeToTileIndex(tile));
      }
    }
    this.UpdateBitmaskRegion();
  }

  private int GroundTypeToTileIndex(GroundType groundType)
  {
    if (groundType == GroundType.Water)
    {
      return 3;
    }

    if (groundType == GroundType.Grass)
    {
      return 4;
    }

    if (groundType == GroundType.Dirt)
    {
      return 5;
    }

    return -1;
  }
}
