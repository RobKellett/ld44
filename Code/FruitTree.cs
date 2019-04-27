using Godot;
using System;

public class FruitTree : FruitedPlant
{
    public override void _Ready() {
        FRUIT_TIMER = 30f;
    }

    protected override void Bloom() {
    }
}
