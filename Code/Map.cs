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
    public int MAP_LEFT_EDGE => MAP_WIDTH - 1;
    public int MAP_HEIGHT = 100;
    public int MAP_BOTTOM_EDGE => MAP_HEIGHT - 1;
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
                if(x == 0 || x == MAP_LEFT_EDGE || y == 0 || y == MAP_BOTTOM_EDGE) {
                    // Fill the edges with mountain
                    _land[x,y] = GroundType.Mountain;
                } else {
                    // Fill everything else with grass
                    _land[x,y] = GroundType.Grass;
                }
            }
        }

        // Fill in some random rivers
        var riverCount = _random.Next(2, 5);
        for(var i = 0; i < riverCount; i++) {
            AddRiver();
        }   
    }
    private void AddRiver() {
        var riverCursorX = 0;
        var riverCursorY = 0;
        var targetEdge = 0;
         
        // Start at a random place on a random edge
        // Left edge == edge 0
        // Right edge == edge 2
        // Top edge == edge 1
        // Bottom edge == edge 3
        var edge = _random.Next(0,3);
        if(edge == 0) {
            riverCursorX = 0;
            riverCursorY = _random.Next(0, MAP_BOTTOM_EDGE);
            targetEdge = 2;
        } else if(edge == 1) {
            riverCursorX = _random.Next(0, MAP_LEFT_EDGE);
            riverCursorY = 0;
            targetEdge = 3;
        } else if(edge == 2) {
            riverCursorX = MAP_LEFT_EDGE;
            riverCursorY = _random.Next(0, MAP_BOTTOM_EDGE);
            targetEdge = 0;
        } else if(edge == 3) {
            riverCursorX = _random.Next(0, MAP_LEFT_EDGE);
            riverCursorY = MAP_BOTTOM_EDGE;
            targetEdge = 1;
        }
        // Now meander until we hit mountain or river, biased to flow north/south
        while(true) {
            _land[riverCursorX, riverCursorY] = GroundType.Water;
            var randTransverse = _random.Next(-1, 1);
            var randFlow = _random.Next(0, 1);
            if(randFlow == 0 && randTransverse == 0) {
                // Make sure we're always progressing, at least
                randFlow = 1;
            }
            switch(targetEdge) {
                case 0:
                    // We're trying to get to the left edge, so flow left (-x)
                    riverCursorX -= randFlow;
                    riverCursorY += randTransverse;
                    break;
                case 1:
                    // We're trying to get to the top edge, so flow up (-y)
                    riverCursorX += randTransverse;
                    riverCursorY -= randFlow;
                    break;
                case 2:
                    // We're trying to get to the right edge, so flow right (+x)
                    riverCursorX += randFlow;
                    riverCursorY += randTransverse;
                    break;
                case 3:
                    // We're trying to get to the bottom edge, so flow down (+y)
                    riverCursorX += randTransverse;
                    riverCursorY += randFlow;
                    break;
                default: break;
            }
            // If we go past the edge, or hit water or mountain, stop
            if (riverCursorX < 0 ||
                riverCursorY < 0 ||
                riverCursorX > MAP_LEFT_EDGE ||
                riverCursorY > MAP_BOTTOM_EDGE ||
                _land[riverCursorX, riverCursorY] == GroundType.Water ||
                _land[riverCursorX, riverCursorY] == GroundType.Mountain
            ) {
                break;
            } 
        }
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
