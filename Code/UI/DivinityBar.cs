using Godot;
using System;

public class DivinityBar : TextureProgress
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var gp = GetNode<GodPowers>("/root/Map/GodPowers");
        Value = gp._divinity;
    }
}
