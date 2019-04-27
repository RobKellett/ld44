using Godot;
using System;

public enum GroundType {
    Grass,
    Water,
    Dirt,
    Desert,
    Mountain,
}

public class Map : Node
{
    public int MAP_WIDTH = 100;
    public int MAP_HEIGHT = 100;
    public GroundType[,] _land;
    private Random _random;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _random = new Random();
        GenerateMap();
    }

    private void GenerateMap() {
        if(_land != null) {
            return;
        }

        _land = new GroundType[MAP_WIDTH,MAP_HEIGHT];

        for(int x = 0; x < MAP_WIDTH; x++) {
            for(int y = 0; y < MAP_WIDTH; y++) {
                _land[x,y] = RandomGroundType();
            }
        }
        GD.Print(_land[10,10]);
    }

    private GroundType RandomGroundType() {
        int choice = _random.Next(0, 4);
        switch(choice) {
            case 0: return GroundType.Grass;
            case 1: return GroundType.Water;
            case 2: return GroundType.Dirt;
            case 3: return GroundType.Desert;
            case 4: return GroundType.Mountain;
            default: return GroundType.Grass;
        }
    }
}
