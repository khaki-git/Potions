// UNUSED, SHITTY CHATGPT CODE

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Potions
{
    public class VoidMaze
    {
        private const string RootNamePrefix = "VoidMaze";

        private readonly Cell[,] _cells;
        private readonly Vector2Int _gridSize;
        private readonly float _tileSize;
        private readonly System.Random _random;
        private readonly List<Neighbor> _neighborBuffer = new(4);

        public GameObject Root { get; }
        public Vector3 StartPosition { get; }
        public Vector3 GoalPosition { get; }
        private bool _destroyed;

        private enum Direction
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        private static readonly Vector2Int[] DirectionVectors =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0)
        };

        private static readonly Direction[] OppositeDirections =
        {
            Direction.South,
            Direction.West,
            Direction.North,
            Direction.East
        };

        private class Cell
        {
            public readonly Transform?[] Walls = new Transform?[4];
            public bool Visited;
        }

        private struct Neighbor
        {
            public Vector2Int Position;
            public Direction Direction;
        }

        public VoidMaze(Vector3 origin, int gridX, int gridY, float tileSize, int? seed = null, Transform? parent = null)
        {
            if (Plugin.voidTile == null) throw new InvalidOperationException("Cannot build a VoidMaze before Plugin.voidTile is initialized.");
            if (gridX <= 0) throw new ArgumentOutOfRangeException(nameof(gridX), gridX, "Grid X must be greater than zero.");
            if (gridY <= 0) throw new ArgumentOutOfRangeException(nameof(gridY), gridY, "Grid Y must be greater than zero.");
            if (tileSize <= 0f) throw new ArgumentOutOfRangeException(nameof(tileSize), tileSize, "Tile size must be greater than zero.");

            _gridSize = new Vector2Int(gridX, gridY);
            _tileSize = tileSize;
            _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
            _cells = new Cell[_gridSize.x, _gridSize.y];

            Root = new GameObject($"{RootNamePrefix}_{gridX}x{gridY}");
            if (parent != null) Root.transform.SetParent(parent, false);
            Root.transform.position = origin;
            Root.transform.rotation = Quaternion.identity;
            Root.transform.localScale = Vector3.one;

            PopulateTiles();
            GenerateMaze();

            StartPosition = Root.transform.TransformPoint(Vector3.zero);
            GoalPosition = Root.transform.TransformPoint(new Vector3((_gridSize.x - 1) * _tileSize, 0f, (_gridSize.y - 1) * _tileSize));
        }

        public void DestroyMaze(bool immediate = false)
        {
            if (_destroyed) return;
            _destroyed = true;
            if (Root == null) return;
            if (Application.isPlaying && !immediate) UnityEngine.Object.Destroy(Root);
            else UnityEngine.Object.DestroyImmediate(Root);
        }

        private void PopulateTiles()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    var cell = new Cell();
                    var tileInstance = UnityEngine.Object.Instantiate(Plugin.voidTile, Root.transform);
                    tileInstance.name = $"VoidTile_{x}_{y}";
                    var t = tileInstance.transform;

                    t.localRotation = Quaternion.identity;
                    t.localScale = new Vector3(_tileSize, 1f, _tileSize);
                    t.localPosition = new Vector3(x * _tileSize, 0f, y * _tileSize);

                    CacheWall(t, cell, Direction.North, "North");
                    CacheWall(t, cell, Direction.East, "East");
                    CacheWall(t, cell, Direction.South, "South");
                    CacheWall(t, cell, Direction.West, "West");

                    _cells[x, y] = cell;
                }
            }
        }

        private static void CacheWall(Transform root, Cell cell, Direction dir, string baseName)
        {
            Transform? w = FindChild(root, baseName);
            if (w == null) w = FindChild(root, $"{baseName}Wall");
            if (w == null) w = FindChild(root, $"Wall_{baseName}");
            cell.Walls[(int)dir] = w;
        }

        private static Transform? FindChild(Transform parent, string name)
        {
            if (parent.name.Equals(name, StringComparison.Ordinal)) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                var r = FindChild(c, name);
                if (r != null) return r;
            }
            return null;
        }

        private void GenerateMaze()
        {
            var start = Vector2Int.zero;
            var stack = new Stack<Vector2Int>(_gridSize.x * _gridSize.y);
            _cells[start.x, start.y].Visited = true;
            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var neighbors = GetUnvisitedNeighbors(current);

                if (neighbors.Count == 0)
                {
                    stack.Pop();
                    continue;
                }

                Shuffle(neighbors);
                var choice = neighbors[0];

                RemoveWallBetween(current, choice.Position, choice.Direction);

                var next = choice.Position;
                _cells[next.x, next.y].Visited = true;
                stack.Push(next);
            }
        }

        private List<Neighbor> GetUnvisitedNeighbors(Vector2Int cell)
        {
            _neighborBuffer.Clear();
            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;
                var n = cell + DirectionVectors[i];
                if (n.x >= 0 && n.x < _gridSize.x && n.y >= 0 && n.y < _gridSize.y && !_cells[n.x, n.y].Visited)
                {
                    _neighborBuffer.Add(new Neighbor { Position = n, Direction = dir });
                }
            }
            return _neighborBuffer;
        }

        private void RemoveWallBetween(Vector2Int current, Vector2Int next, Direction dirFromCurrentToNext)
        {
            var a = _cells[current.x, current.y];
            var b = _cells[next.x, next.y];
            DisableWall(a, dirFromCurrentToNext);
            DisableWall(b, OppositeDirections[(int)dirFromCurrentToNext]);
        }

        private static void DisableWall(Cell cell, Direction direction)
        {
            var w = cell.Walls[(int)direction];
            if (w != null && w.gameObject.activeSelf) w.gameObject.SetActive(false);
        }

        private void Shuffle(List<Neighbor> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
