using Godot;
using System;

public class GodPowers : Node2D
{
    public override void _Input(InputEvent evt) {
        var map = GetParent<Map>();
        if(evt is InputEventMouseButton) {
            var evtMB = (InputEventMouseButton)evt;
            if(evtMB.Pressed) {
                var cell = map.ToCellCoordinates(GetGlobalMousePosition());
                map.AddPlant((int)cell.x, (int)cell.y, PlantType.Tree, 2f);
            }
        }
    }
}
