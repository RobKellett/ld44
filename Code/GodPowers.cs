using Godot;
using System;

public class GodPowers : Node2D
{
    private PackedScene _riverCarverScene;
    public int _divinity = 100;

    public override void _Ready() {
        _riverCarverScene = GD.Load<PackedScene>("res://Objects/RiverCarver.tscn");
    }

    public override void _Input(InputEvent evt) {
        var map = GetParent<Map>();
        if(evt is InputEventMouseButton) {
            var evtMB = (InputEventMouseButton)evt;
            var cell = map.ToCellCoordinates(GetGlobalMousePosition());
            if(evtMB.Pressed && evtMB.ButtonIndex == 1 && _divinity >= 10) {
                SpawnDivineForest((int)cell.x, (int)cell.y, PlantType.Tree);
                _divinity -= 10;
            }
            if(evtMB.Pressed && evtMB.ButtonIndex == 2) {
                GD.Print("Spawning river towards ", cell.x, cell.y);
                SpawnDivineRiverTargetingPoint((int)cell.x, (int)cell.y);
            }
        }
    }

    public void SpawnDivineForest(int x, int y, PlantType type) {
        var map = GetParent<Map>();
        map.AddPlant(x, y, PlantType.Tree, 2f);
    }

    public void SpawnDivineRiverTargetingPoint(int x, int y) {
        // Find the nearest water tile
        var map = GetParent<Map>();
        var nearestRiver = map.FindNearestTileOfType(x, y, GroundType.Water);
        if(nearestRiver.x < 0) {
            return;
        }

        GD.Print("Found river: ", nearestRiver.x, ", ", nearestRiver.y);
        
        // Spawn a river carver
        var node = _riverCarverScene.Instance() as RiverCarver;
        node.CellX = (int)nearestRiver.x;
        node.CellY = (int)nearestRiver.y;
        node._targetX = x;
        node._targetY = y;
        node._godPowers = this;
        map.AddChild(node);
    }
}
