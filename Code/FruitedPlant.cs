using Godot;
using System;
using LD44.Resources;
using LD44.Utilities;

public class FruitedPlant : BaseWorldObject, IFoodSource
{
    public float FRUIT_TIMER = 0f;
    public float _timeSinceFruitPicked = 0f;
    public bool _bearingFruit = false;
    protected bool _canBearFruit = true;
    public bool HasFood { get { return _bearingFruit; } }
    protected virtual int FoodValue { get; }

    public override void _Ready() {
        Group.FoodSources.Add(this);
    }

    public override void _Process(float delta) {
        if(_canBearFruit && !_bearingFruit) {
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

    public Vector2 GetClosestPosition(Vector2 yourPosition)
    {
      return Position;
    }

    public void TakeFood(Human human)
    {
      Pick();
      human.Feed(FoodValue);
      Group.Humans.Call(GetTree(), h => h.ResourceDestroyed(this, human));
    }
}
