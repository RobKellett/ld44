using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public int MAP_RIGHT_EDGE => MAP_WIDTH - 1;
    public int MAP_HEIGHT = 100;
    public int MAP_BOTTOM_EDGE => MAP_HEIGHT - 1;
    public GroundType[,] _land;
    private Random _random;

    private PackedScene _tree;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _tree = GD.Load<PackedScene>("res://Tree.tscn");
        _random = new Random();
        GenerateMap();
        var mapRenderer = GetChild<MapRenderer>(0);
        mapRenderer.DoRender(this);
    }

    private void GenerateMap() {
        if(_land != null) {
            return;
        }

        _land = new GroundType[MAP_WIDTH,MAP_HEIGHT];

        for(int x = 0; x < MAP_WIDTH; x++) {
            for(int y = 0; y < MAP_WIDTH; y++) {
                if(x == 0 || x == MAP_RIGHT_EDGE || y == 0 || y == MAP_BOTTOM_EDGE) {
                    // Fill the edges with mountain
                    _land[x,y] = GroundType.Mountain;
                } else {
                    // Fill everything else with grass
                    _land[x,y] = GroundType.Grass;
                }
            }
        }

        // Fill in some random rivers
        var riverCount = _random.Next(6, 12);
        for(var i = 0; i < riverCount; i++) {
            AddRiver();
        }

        for(var i = 0; i < 50; i++) {
            var centerX = _random.Next(0, MAP_WIDTH);
            var centerY = _random.Next(0, MAP_HEIGHT);

            AddBiome(centerX, centerY, 0, 200, RandomGroundType(new GroundType[] { GroundType.Mountain, GroundType.Water }));
        }

        for(var i = 0; i < 10; i++) {
            var centerX = _random.Next(0, MAP_WIDTH);
            var centerY = _random.Next(0, MAP_HEIGHT);
            AddForest(centerX, centerY, 0, 200);
        }
        GD.Print("Done generating map.");
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
        var edge = _random.Next(0, 4);
        if(edge == 0) {
            riverCursorX = 0;
            riverCursorY = _random.Next(1, MAP_BOTTOM_EDGE);
            targetEdge = 2;
        } else if(edge == 1) {
            riverCursorX = _random.Next(1, MAP_RIGHT_EDGE);
            riverCursorY = 0;
            targetEdge = 3;
        } else if(edge == 2) {
            riverCursorX = MAP_RIGHT_EDGE;
            riverCursorY = _random.Next(1, MAP_BOTTOM_EDGE);
            targetEdge = 0;
        } else if(edge == 3) {
            riverCursorX = _random.Next(1, MAP_RIGHT_EDGE);
            riverCursorY = MAP_BOTTOM_EDGE;
            targetEdge = 1;
        }
        // Now meander until we hit mountain or river, biased to flow north/south
        bool first = true;
        int dir = 0; // 0 = forward, 1 = left, 2 = right
        while(true) {
            _land[riverCursorX, riverCursorY] = GroundType.Water;
            var randFlow = 0;
            var randTransverse = 0;
            switch(dir) {
                case 0: {
                    randFlow = 1;
                    if(_random.NextDouble() < 0.25) {
                        dir = 0;
                    } else if(_random.NextDouble() < 0.35) {
                        dir = 1;
                    } else {
                        dir = 2;
                    }
                    break;
                }
                case 1: randTransverse = 1; dir = 0; break;
                case 2: randTransverse = -1; dir = 0; break;
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
                riverCursorX > MAP_RIGHT_EDGE ||
                riverCursorY > MAP_BOTTOM_EDGE
            ) {
                break;
            }
            if (_land[riverCursorX, riverCursorY] == GroundType.Mountain) {
                _land[riverCursorX, riverCursorY] = GroundType.Water;
                break;
            }
            if (_land[riverCursorX, riverCursorY] == GroundType.Water) {
                break;
            }
            first = false; 
        }
    }

    private struct PrioritizedMapCoord {
        public int X, Y;
        public float Priority;
    }
    private void AddBiome(int centerX, int centerY, float lumpiness, int targetSize, GroundType type) {
        // Starting at centerX and centerY, start filling in things with type until you reach size
        // Use lumpiness for random bias, such that lumpiness of 0 is a circle
        var items = new List<PrioritizedMapCoord>();
        items.Add(new PrioritizedMapCoord { X = centerX, Y = centerY, Priority = 1 });
        int biomeSize = 0;
        while(items.Count > 0 && biomeSize < targetSize) {
            var next = BiasedPick(items, 0);
            if(next.X < 0 || next.X > MAP_RIGHT_EDGE) continue;
            if(next.Y < 0 || next.Y > MAP_BOTTOM_EDGE) continue;
            if(_land[next.X, next.Y] == type) continue;
            biomeSize++;
            if(_land[next.X, next.Y] != GroundType.Water && _land[next.X, next.Y] != GroundType.Mountain) {
                _land[next.X, next.Y] = type;
            }
            // Now add the neighbors
            items.Add(new PrioritizedMapCoord { X = next.X - 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X + 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y - 1, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y + 1, Priority = 1 });
        }
    }

    private void AddForest(int centerX, int centerY, float lumpiness, int targetSize) {
        // Starting at centerX and centerY, start filling in things with type until you reach size
        // Use lumpiness for random bias, such that lumpiness of 0 is a circle
        var items = new List<PrioritizedMapCoord>();
        items.Add(new PrioritizedMapCoord { X = centerX, Y = centerY, Priority = 1 });
        int biomeSize = 0;
        var trees = new List<PrioritizedMapCoord>();
        while(items.Count > 0 && biomeSize < targetSize) {
            var next = BiasedPick(items, 0);
            if(next.X < 0 || next.X > MAP_RIGHT_EDGE) continue;
            if(next.Y < 0 || next.Y > MAP_BOTTOM_EDGE) continue;
            
            // If a tree already exists at this position
            if(trees.Any(t => t.X == next.X && t.Y == next.Y)) {
                continue;
            }
            biomeSize++;

            if(_land[next.X, next.Y] == GroundType.Dirt ||
               _land[next.X, next.Y] == GroundType.Grass
            ) {
                AddTree(next.X, next.Y);
                trees.Add(next);
            }
            // Now add the neighbors
            items.Add(new PrioritizedMapCoord { X = next.X - 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X + 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y - 1, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y + 1, Priority = 1 });
        }
    }

    private void AddTree(int x, int y) {
        var node = _tree.Instance() as Node2D;
        node.Position = new Vector2(x * 32 + 16, y * 32 + 16);
        AddChild(node);
    }
    private PrioritizedMapCoord BiasedPick(List<PrioritizedMapCoord> options, float bias) {
        PrioritizedMapCoord c = options[0];
        // For now, just use equal probability to pick all of them
        int idx = 0;
        foreach(var i in options) {
            var r = _random.Next(0, idx + 1);
            if(r < 1) {
                c = i;
            }
            idx++;
        }
        options.Remove(c);
        return c;
    }
    private GroundType RandomGroundType(GroundType[] disallowedTypes = null) {
        var allowedTypes = new List<GroundType> {
            GroundType.Grass,
            GroundType.Water,
            GroundType.Dirt,
            GroundType.Desert,
            GroundType.Mountain
        };
        if(disallowedTypes != null) {
            foreach(var dt in disallowedTypes) {
                allowedTypes.Remove(dt);
            }
        }
        
        int choice = _random.Next(0, allowedTypes.Count);
        return allowedTypes[choice];
    }
}
