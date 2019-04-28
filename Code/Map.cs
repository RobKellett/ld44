using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LD44.Utilities;
using LD44.Resources;

public enum GroundType {
    Grass,
    Water,
    Dirt,
    Desert,
    Mountain,
}

public enum PlantType {
    Tree,
    FruitTree,
    Mushroom,
    Bush,
    Cactus,
}

public class Map : Node, IWaterSource
{
    public bool HasWater { get; } = true;
    public int MAP_WIDTH = 100;
    public int MAP_RIGHT_EDGE => MAP_WIDTH - 1;
    public int MAP_HEIGHT = 100;
    public int MAP_BOTTOM_EDGE => MAP_HEIGHT - 1;


    public int MIN_RIVERS = 6, MAX_RIVERS = 12;
    public int MIN_BIOMES = 30, MAX_BIOMES = 60;
    public int MIN_BIOME_SIZE = 40, MAX_BIOME_SIZE = 300;

    public int MIN_FORESTS = 8, MAX_FORESTS = 15;
    public int MIN_FOREST_SIZE = 100, MAX_FOREST_SIZE = 200;

    public int MIN_SPARSE_PLANTS = 90, MAX_SPARSE_PLANTS = 150;

    public int MIN_HUMANS_PER_TEAM = 50, MAX_HUMANS_PER_TEAM = 100;
    public int HUMAN_TEAM_RADIUS = 10;

    public GroundType[,] _land;
    public Node2D[,] _plants;

    private PackedScene[] _plantScenes;
    private PackedScene _humanScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _plantScenes = new PackedScene[] {
            GD.Load<PackedScene>("res://Objects/Tree.tscn"),
            GD.Load<PackedScene>("res://Objects/FruitTree.tscn"),
            GD.Load<PackedScene>("res://Objects/Mushroom.tscn"),
            GD.Load<PackedScene>("res://Objects/Bush.tscn"),
            GD.Load<PackedScene>("res://Objects/Cactus.tscn"),
        };
        _humanScene = GD.Load<PackedScene>("res://Objects/Human.tscn");
        Group.WaterSources.Add(this);
        GenerateMap();
    }

    private void GenerateMap() {
        if(_land != null) {
            return;
        }

        _land = new GroundType[MAP_WIDTH,MAP_HEIGHT];
        _plants = new Node2D[MAP_WIDTH,MAP_HEIGHT];

        for(int x = 0; x < MAP_WIDTH; x++) {
            for(int y = 0; y < MAP_WIDTH; y++) {
                if(x == 0 || x == MAP_RIGHT_EDGE || y == 0 || y == MAP_BOTTOM_EDGE) {
                    // Fill the edges with mountain
                    _land[x,y] = GroundType.Mountain;
                } else {
                    // Fill everything else with dirt
                    _land[x,y] = GroundType.Dirt;
                }
            }
        }

        // Fill in some random rivers
        var riverCount = RNG.Instance.Next(MIN_RIVERS, MAX_RIVERS + 1);
        for(var i = 0; i < riverCount; i++) {
            AddRiver();
        }

        var biomeCount = RNG.Instance.Next(MIN_BIOMES, MAX_BIOMES + 1);
        for(var i = 0; i < biomeCount; i++) {
            var centerX = RNG.Instance.Next(0, MAP_WIDTH);
            var centerY = RNG.Instance.Next(0, MAP_HEIGHT);

            var biomeSize = RNG.Instance.Next(MIN_BIOME_SIZE, MAX_BIOME_SIZE + 1);
            AddBiome(centerX, centerY, 0, biomeSize, RandomGroundType(new GroundType[] { GroundType.Mountain, GroundType.Water }));
        }

        // Set some wood and mushroom forests
        var forestedPlants = new[] { PlantType.Tree, PlantType.Mushroom };
        var forestCount = RNG.Instance.Next(MIN_FORESTS, MAX_FORESTS + 1);
        for(var i = 0; i < forestCount; i++) {
            var centerX = RNG.Instance.Next(0, MAP_WIDTH);
            var centerY = RNG.Instance.Next(0, MAP_HEIGHT);
            var allowedTypes = GetAllowedPlants(_land[centerX, centerY]).Intersect(forestedPlants).ToArray();
            if(allowedTypes.Length == 0) continue;
            var plantType = RNG.Instance.Next(0, allowedTypes.Length);
            var forestSize = RNG.Instance.Next(MIN_FOREST_SIZE, MAX_FOREST_SIZE + 1);
            AddForest(centerX, centerY, 0, forestSize, allowedTypes[plantType]);
        }

        // Pepper some cacti, bushes, and fruit trees
        var sparsePlants = new[] { PlantType.Bush, PlantType.Cactus, PlantType.FruitTree };
        var sparsePlantCount = RNG.Instance.Next(MIN_SPARSE_PLANTS, MAX_SPARSE_PLANTS + 1);
        for(var i = 0; i < sparsePlantCount; i++) {
            var posX = RNG.Instance.Next(1, MAP_RIGHT_EDGE);
            var posY = RNG.Instance.Next(1, MAP_BOTTOM_EDGE);
            if(_plants[posX, posY] != null) continue;
            var allowedTypes = GetAllowedPlants(_land[posX, posY]).Intersect(sparsePlants).ToArray();
            if(allowedTypes.Length == 0) continue;
            var plantType = RNG.Instance.Next(0, allowedTypes.Length);
            AddPlant(posX, posY, allowedTypes[plantType]);
        }

        // Humans, I think
        foreach (var team in new[] { Human.Team.Blue, Human.Team.Green, Human.Team.Red }) {
            var humanCount = RNG.Instance.Next(MIN_HUMANS_PER_TEAM, MAX_HUMANS_PER_TEAM + 1);
            var originX = RNG.Instance.Next(1 + HUMAN_TEAM_RADIUS, MAP_RIGHT_EDGE - HUMAN_TEAM_RADIUS);
            var originY = RNG.Instance.Next(1 + HUMAN_TEAM_RADIUS, MAP_BOTTOM_EDGE - HUMAN_TEAM_RADIUS);
            var validSpawns = new List<Vector2>();
            for (var y = originY - HUMAN_TEAM_RADIUS; y < originY + HUMAN_TEAM_RADIUS; y++) {
                for (var x = originX - HUMAN_TEAM_RADIUS; x < originX + HUMAN_TEAM_RADIUS; x++) {
                    var distanceFromOriginSquared = (y - originY) * (y - originY) + (x - originX) * (x - originX);
                    if (distanceFromOriginSquared > HUMAN_TEAM_RADIUS * HUMAN_TEAM_RADIUS) continue;
                    if (_plants[x, y] != null) continue;
                    validSpawns.Add(new Vector2(x, y));
                }
            }

            RNG.Shuffle(validSpawns);
            foreach (var spawn in validSpawns.Take(humanCount))
            {
                AddHuman((int)spawn.x, (int)spawn.y, team);
            }
        }

        GD.Print("Done generating map.");
        Group.MapUpdates.Call(GetTree(), c => c.MapUpdated());
    }

    public void FinishUpdates() {
        Group.MapUpdates.Call(GetTree(), c => c.MapUpdated());
    }

    public void UpdateCell(int x, int y, GroundType type) {
        _land[x,y] = type;
        Group.MapUpdates.Call(GetTree(), c => c.MapCellUpdated(x, y, type));
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
        var edge = RNG.Instance.Next(0, 4);
        if(edge == 0) {
            riverCursorX = 0;
            riverCursorY = RNG.Instance.Next(1, MAP_BOTTOM_EDGE);
            targetEdge = 2;
        } else if(edge == 1) {
            riverCursorX = RNG.Instance.Next(1, MAP_RIGHT_EDGE);
            riverCursorY = 0;
            targetEdge = 3;
        } else if(edge == 2) {
            riverCursorX = MAP_RIGHT_EDGE;
            riverCursorY = RNG.Instance.Next(1, MAP_BOTTOM_EDGE);
            targetEdge = 0;
        } else if(edge == 3) {
            riverCursorX = RNG.Instance.Next(1, MAP_RIGHT_EDGE);
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
                    if(RNG.Instance.NextDouble() < 0.25) {
                        dir = 0;
                    } else if(RNG.Instance.NextDouble() < 0.35) {
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
            if (!IsWithinBounds(riverCursorX, riverCursorY)) {
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

    public void AddForest(int centerX, int centerY, float lumpiness, int targetSize, PlantType plantType) {
        // Starting at centerX and centerY, start filling in things with type until you reach size
        // Use lumpiness for random bias, such that lumpiness of 0 is a circle
        var items = new List<PrioritizedMapCoord>();
        items.Add(new PrioritizedMapCoord { X = centerX, Y = centerY, Priority = 1 });
        int biomeSize = 0;
        while(items.Count > 0 && biomeSize < targetSize) {
            var next = BiasedPick(items, 0);

            if(AddPlant(next.X, next.Y, plantType)) {
                biomeSize++;
            } else {
                continue;
            }
            // Now add the neighbors
            items.Add(new PrioritizedMapCoord { X = next.X - 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X + 1, Y = next.Y, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y - 1, Priority = 1 });
            items.Add(new PrioritizedMapCoord { X = next.X, Y = next.Y + 1, Priority = 1 });
        }
    }

    public bool AddPlant(int x, int y, PlantType plant, float supergrowthTime = 0f) {
        if(!IsAllowedAtPoint(plant, x, y)) return false;
        var node = _plantScenes[(int)plant].Instance() as BaseWorldObject;
        if(supergrowthTime > 0) {
            var fn = node as ForestedPlant;
            if(fn == null) { throw new Exception(); }
            fn._superGrowthTimeRemaining = supergrowthTime;
        }
        node.CellX = x;
        node.CellY = y;
        node.Position = new Vector2(x * 32 + 16, y * 32 + 16);
        AddChild(node);
        _plants[x,y] = node;
        return true;
    }
    private PrioritizedMapCoord BiasedPick(List<PrioritizedMapCoord> options, float bias) {
        PrioritizedMapCoord c = options[0];
        // For now, just use equal probability to pick all of them
        int idx = 0;
        foreach(var i in options) {
            var r = RNG.Instance.Next(0, idx + 1);
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
        
        int choice = RNG.Instance.Next(0, allowedTypes.Count);
        return allowedTypes[choice];
    }

    private PlantType[] GetAllowedPlants(GroundType biome) {
        switch(biome) {
            case GroundType.Desert:
                return new [] { PlantType.Cactus };
            case GroundType.Dirt:
                return new [] { PlantType.Tree, PlantType.Mushroom, PlantType.Bush };
            case GroundType.Grass:
                return new [] { PlantType.Tree, PlantType.FruitTree, PlantType.Bush };
            default:
                return new PlantType[] {};
        }
    }

    public bool IsWithinBounds(int x, int y) {
        if(x < 0 || x > MAP_RIGHT_EDGE || y < 0 || y > MAP_BOTTOM_EDGE) return false;
        return true;
    }

    public bool IsAllowedAtPoint(PlantType plant, int x, int y) {
        if(!IsWithinBounds(x, y)) return false;
        if(_plants[x, y] != null) return false;
        if(!IsAllowedInBiome(plant, _land[x, y])) return false;
        return true;
    }

    private bool IsAllowedInBiome(PlantType plant, GroundType biome) {
        switch(plant) {
            case PlantType.Tree:
                return biome == GroundType.Grass || biome == GroundType.Dirt;
            case PlantType.FruitTree:
                return biome == GroundType.Grass;
            case PlantType.Mushroom:
                return biome == GroundType.Dirt;
            case PlantType.Bush:
                return biome == GroundType.Grass || biome == GroundType.Dirt;
            case PlantType.Cactus:
                return biome == GroundType.Desert;
            default: return true;
        }
    }

    public Vector2 ToCellCoordinates(Vector2 world) {
        return new Vector2 { x = (int)(world.x / 32), y = (int)(world.y / 32) };
    }

    public bool AddHuman(int x, int y, Human.Team team)
    {
        var node = _humanScene.Instance() as Human;
        node.SetTeam(team);
        node.Position = new Vector2(x * 32 + 16, y * 32 + 16);
        AddChild(node);
        return true;
    }

    public Vector2 FindNearestTileOfType(int x, int y, GroundType type) {
        var queue = new Queue<Tuple<int,int>>();
        var visited = new HashSet<Tuple<int,int>>();
        queue.Enqueue(new Tuple<int,int>(x, y));
        while(queue.Count > 0) {
            var next = queue.Dequeue();
            if(_land[next.Item1, next.Item2] == type) {
                return new Vector2(next.Item1, next.Item2);
            }
            var left = new Tuple<int,int>(next.Item1 - 1, next.Item2);
            if(!visited.Contains(left) && IsWithinBounds(left.Item1, left.Item2)) {
                queue.Enqueue(left);
                visited.Add(left);
            }
            var up = new Tuple<int,int>(next.Item1, next.Item2 - 1);
            if(!visited.Contains(up) && IsWithinBounds(up.Item1, up.Item2)) {
                queue.Enqueue(up);
                visited.Add(up);
            }
            var right = new Tuple<int,int>(next.Item1 + 1, next.Item2);
            if(!visited.Contains(right) && IsWithinBounds(right.Item1, right.Item2)) {
                queue.Enqueue(right);
                visited.Add(right);
            }
            var down = new Tuple<int,int>(next.Item1, next.Item2 + 1);
            if(!visited.Contains(down) && IsWithinBounds(down.Item1, down.Item2)) {
                queue.Enqueue(down);
                visited.Add(down);
            }
        }
        return new Vector2(-1, -1);
    }

    public Vector2 GetClosestPosition(Vector2 yourPosition)
    {
        var cellCoords = ToCellCoordinates(yourPosition);
        var target = FindNearestTileOfType((int)cellCoords.x, (int)cellCoords.y, GroundType.Water);
        return new Vector2(target.x * 32 + 16, target.y * 32 + 16);
    }

    public void TakeWater(Human human)
    {
        human.Drink();
    }
}
