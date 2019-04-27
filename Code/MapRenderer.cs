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
        this.SetCell(x * 2, y * 2, GroundTypeToTileIndex(tile));
        this.SetCell(x * 2 + 1, y * 2, GroundTypeToTileIndex(tile));
        this.SetCell(x * 2, y * 2 + 1, GroundTypeToTileIndex(tile));
        this.SetCell(x * 2 + 1, y * 2 + 1, GroundTypeToTileIndex(tile));
      }
    }
    this.UpdateBitmaskRegion();
  }

  private int GroundTypeToTileIndex(GroundType groundType)
  {
    var types = new GroundType[]
      {
        GroundType.Water,
        GroundType.Grass,
        GroundType.Dirt,
        GroundType.Desert,
        GroundType.Mountain
      };

    return Array.IndexOf(types, groundType);
  }
}
