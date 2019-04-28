using Godot;
using System;
using LD44.Utilities;

public class MapRenderer : TileMap, ICareAboutMapUpdates
{
  private GroundType[,] _oldMap;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
    Group.MapUpdates.Add(this);
    var map = GetParent<Map>();
    _oldMap = new GroundType[map.MAP_WIDTH,map.MAP_HEIGHT];
    for(var x = 0; x < map.MAP_WIDTH; x++) {
      for(var y = 0; y < map.MAP_HEIGHT; y++) {
        _oldMap[x,y] = (GroundType)(-1);
      }
    }
  }

  public void MapUpdated() {
    DoFullRender(GetParent<Map>());
  }

  public void MapCellUpdated(int x, int y, GroundType tile) {
    UpdateCell(x, y, tile);
  }

  public void UpdateCell(int x, int y, GroundType tile) {
    var oldTile = _oldMap[x,y];
    if(tile != oldTile) {
      var tileIdx = GroundTypeToTileIndex(tile);
      this.SetCell(x * 2, y * 2, tileIdx);
      this.SetCell(x * 2 + 1, y * 2, tileIdx);
      this.SetCell(x * 2, y * 2 + 1, tileIdx);
      this.SetCell(x * 2 + 1, y * 2 + 1, tileIdx);
      _oldMap[x,y] = tile;
      this.UpdateBitmaskRegion(new Vector2(x*2, y*2), new Vector2(x*2 + 1, y*2 + 1));
    }
  }

  public void DoFullRender(Map map)
  {
    GD.Print("Full redraw");
    int dirtyMinX = int.MaxValue, dirtyMaxX = -1;
    int dirtyMinY = int.MaxValue, dirtyMaxY = -1;

    for (var y = 0; y < map.MAP_HEIGHT; ++y)
    {
      for (var x = 0; x < map.MAP_WIDTH; ++x)
      {
        var tile = map._land[x, y];
        var oldTile = _oldMap[x,y];
        if(tile != oldTile) {
          dirtyMinX = Math.Min(dirtyMinX, x);
          dirtyMaxX = Math.Max(dirtyMaxX, x);
          dirtyMinY = Math.Min(dirtyMinY, y);
          dirtyMaxY = Math.Max(dirtyMaxY, y);
          var tileIdx = GroundTypeToTileIndex(tile);
          this.SetCell(x * 2, y * 2, tileIdx);
          this.SetCell(x * 2 + 1, y * 2, tileIdx);
          this.SetCell(x * 2, y * 2 + 1, tileIdx);
          this.SetCell(x * 2 + 1, y * 2 + 1, tileIdx);
          _oldMap[x,y] = tile;
        }
      }
    }
    if(dirtyMaxX >= 0) {
      this.UpdateBitmaskRegion(new Vector2(dirtyMinX, dirtyMinY), new Vector2(dirtyMaxX, dirtyMaxY));
    }
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
