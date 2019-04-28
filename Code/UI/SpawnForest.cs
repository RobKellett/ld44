using Godot;
using System;

public class SpawnForest : TextureButton
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var powers = GetNode<GodPowers>("/root/Map/GodPowers");
        Pressed = powers._activePower == GodPowerTypes.SpawnForest;
    }

    public void _on_SpawnForest_pressed() {
        var powers = GetNode<GodPowers>("/root/Map/GodPowers");
        powers._activePower = GodPowerTypes.SpawnForest;
    }
}
