using Godot;
using System;

public class RaiseMountains : TextureButton
{
    public override void _Process(float delta)
    {
        var powers = GetNode<GodPowers>("/root/Map/GodPowers");
        Pressed = powers._activePower == GodPowerTypes.RaiseMountains;
    }

    public void _on_RaiseMountains_pressed() {
        var gp = GetNode<GodPowers>("/root/Map/GodPowers");
        gp._activePower = GodPowerTypes.RaiseMountains;
    }
}
