using Godot;
using System;

public class PopulationCount : Label
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Text = GetTree().GetNodesInGroup(nameof(Human)).Count.ToString();
    }
}
