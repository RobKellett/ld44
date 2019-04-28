using Godot;
using System;

public enum GodPowerTypes {
    None,
    SpawnForest,
    CarveRiver,
}

public class GodPowers : Node2D
{
    private PackedScene _riverCarverScene;
    public int _divinity = 100;
    public GodPowerTypes _activePower = GodPowerTypes.None;

    public override void _Ready() {
        _riverCarverScene = GD.Load<PackedScene>("res://Objects/RiverCarver.tscn");
    }

    public override void _Input(InputEvent evt) {
        var map = GetParent<Map>();
        if(evt is InputEventMouseButton) {
            var evtMB = (InputEventMouseButton)evt;
            var cell = map.ToCellCoordinates(GetGlobalMousePosition());
            if(evtMB.ButtonIndex != 1) return;
            if(evtMB.Pressed && _activePower == GodPowerTypes.SpawnForest) {
                if(_divinity >= 10) {
                    SpawnDivineForest((int)cell.x, (int)cell.y, PlantType.Tree);
                    _divinity -= 10;
                }
                _activePower = GodPowerTypes.None;
            }
            if(evtMB.Pressed && _activePower == GodPowerTypes.CarveRiver) {
                SpawnDivineRiverTargetingPoint((int)cell.x, (int)cell.y);
                _activePower = GodPowerTypes.None;
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
