using Godot;
using System;
using LD44.Utilities;

public class RiverCarver : BaseWorldObject
{
    public int _targetX;
    public int _targetY;
    public float _timeUntilCarve = 0f;
    public float _timeToCarve = 0.5f;

    public override void _Process(float delta) {
        _timeUntilCarve -= delta;
        if(_timeUntilCarve > 0) {
            return;
        }
        _timeUntilCarve = _timeToCarve;
        // If we've reached our destination, go ahead and clean up
        if(CellX == _targetX && CellY == _targetY) {
            QueueFree();
        }

        // Otherwise, set our current tile to water, and make a biased walk towards the target
        var map = GetParent<Map>();
        if(map.IsWithinBounds(CellX, CellY)) {
            map.UpdateCell(CellX, CellY, GroundType.Water);
        }

        var leftBias = 0;
        var rightBias = 0;
        var upBias = 0;
        var downBias = 0;
        // For each direction, compute the delta in that direction.
        // If it would be moving away, use a "distance" of 5, so we can meander past a bit
        // Left/Right
        if(CellX > _targetX) {
            leftBias = (CellX - _targetX) * 10; // Travelling left
            rightBias = 5; // Travelling right
        } else if(_targetX > CellX) {
            rightBias = (_targetX - CellX) * 10; // Travelling right
            leftBias = 5; // Travelling left
        } else {
            leftBias = 5;
            rightBias = 5;
        }
        // Up/Down
        if(CellY > _targetY) {
            upBias = (CellY - _targetY) * 10;
            downBias = 5;
        } else if(_targetY > CellY) {
            downBias = (_targetY - CellY) * 10;
            upBias = 5;
        } else {
            upBias = 5;
            downBias = 5;
        }
        var totalBias = leftBias + upBias + rightBias + downBias;
        // Now choose a random number between 0 and totalBias
        var direction = RNG.Instance.Next(0, totalBias + 1);
        if(direction <= leftBias) {
            CellX -= 1;
        } else if(direction <= leftBias + upBias) {
            CellY -= 1;
        } else if(direction <= leftBias + upBias + rightBias) {
            CellX += 1;
        } else {
            CellY += 1;
        }
         
    }
}
