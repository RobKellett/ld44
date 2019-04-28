using Godot;
using System;

public class FruitedPlant : BaseWorldObject
{
    public float FRUIT_TIMER = 0f;
    public float _timeSinceFruitPicked = 0f;
    public bool _bearingFruit = false;

    public override void _Process(float delta) {
        if(!_bearingFruit) {
            _timeSinceFruitPicked += delta;
            if(_timeSinceFruitPicked > FRUIT_TIMER) {
                Bloom();
            }
        }
    }

    protected virtual void Bloom() {
        _bearingFruit = true;
    }
    public virtual void Pick() {
        if(_bearingFruit) {
            _bearingFruit = false;
            _timeSinceFruitPicked = 0;
        }
    }
}
