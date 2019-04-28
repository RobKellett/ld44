using Godot;
using System;
using LD44.Utilities;

public class RiverCarver : BaseWorldObject
{
    public int _targetX;
    public int _targetY;
    public int _heading = -1;
    public float _timeUntilCarve = 0f;
    public float _timeToCarve = 0.2f;
    public GodPowers _godPowers;

    public override void _Process(float delta) {
        _timeUntilCarve -= delta;
        if(_timeUntilCarve > 0) {
            return;
        }
        var map = GetParent<Map>();
        _timeUntilCarve = _timeToCarve;
        // If we've reached our destination or run out of power, go ahead and clean up
        if(CellX == _targetX && CellY == _targetY || _godPowers._divinity < 3) {
            QueueFree();
        }

        // Otherwise, set our current tile to water, and make a biased walk towards the target
        if(map.IsWithinBounds(CellX, CellY)) {
            map.UpdateCell(CellX, CellY, GroundType.Water);
            _godPowers._divinity -= 3;
        }

        // If we dont' have a heading, point whichever direction will get us closest to the target
        var optimalHeading = GetOptimalHeading();
        if(_heading < 0) {
            _heading = optimalHeading;
        }

        // Go straight one, and choose a new heading
        MoveForward();

        // If we're headed in the right direction
        if(_heading == optimalHeading) {
            // 12% chance we turn
            var turn = RNG.Instance.Next(0, 100);
            if(turn < 20) {
                // 10% towards target
                // 2% away from
                if(_heading == 0 || _heading == 2) { // If travelling left or right
                    if(CellY > _targetY) { // Target is above?
                        if(turn < 15) { // 10% probability to turn towards
                            _heading = 1; // Turn up
                        } else { // 2% probability to turn away
                            _heading = 3; // Turn down
                        }
                    } else { // Otherwise target is below
                        if(turn < 15) { // 10% towards
                            _heading = 3; // Turn down
                        } else { // 2% turn away
                            _heading = 1; // Turn Up
                        }
                    }
                } else { // If travelling up or down
                    if(CellX > _targetY) { // Target is to left?
                        if(turn < 15) { // 10% towards
                            _heading = 0;
                        } else { // 2% away
                            _heading = 2;
                        }
                    } else {
                        if(turn < 15) {
                            _heading = 2;
                        } else {
                            _heading = 1;
                        }
                    }
                }
            }
        } else {
            // If we're not travelling the optimal direction, we have a 70% chance to turn correct, otherwise stay the same
            var turn = RNG.Instance.Next(0, 100);
            if(turn < 60) {
                _heading = optimalHeading;
            }
        }

        /*
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
        }*/
         
    }

    int GetOptimalHeading() {
        if(Math.Abs(CellX - _targetX) > Math.Abs(CellY - _targetY)) {
            // Travel left/right
            if(CellX > _targetX) {
                return 0;
            } else {
                return 2;
            }
        } else {
            // Travel up/down
            if(CellY > _targetY) {
                return 1;
            } else {
                return 3;
            }
        }
    }

    void MoveForward() {
        switch(_heading) {
            case 0:
                CellX -= 1;
                break;
            case 1:
                CellY -= 1;
                break;
            case 2:
                CellX += 1;
                break;
            case 3:
                CellY += 1;
                break;
            default:
                break;
        }
    }
}
