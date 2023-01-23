using System;
using System.Collections.Generic;

namespace narkdagas.tbcs.grid {
    public readonly struct GridPosition : IEquatable<GridPosition> {
        public readonly int X;
        public readonly int Z;

        private static readonly List<GridPosition> SquareNeighbours = new() {
            new GridPosition(-1, 1), //left-top
            new GridPosition(0, 1), //top
            new GridPosition(1, 1), //right-top
            new GridPosition(1, 0), //right
            new GridPosition(1, -1), //right-bottom
            new GridPosition(0, -1), //bottom
            new GridPosition(-1, -1), //left-bottom
            new GridPosition(-1, 0) //left
        };

        public static List<GridPosition> GetSquareGridPositionsNeighbours() {
            return SquareNeighbours;
        }

        public static List<GridPosition> GetHexGridPositionsNeighbours(bool oddRow) {
            List<GridPosition> neighbours = new() {
                new GridPosition(-1, 0), //left
                new GridPosition(+1, 0), //right
                new GridPosition(0, +1), //top
                new GridPosition(0, -1), //bottom
                new GridPosition(oddRow ? +1 : -1, +1), //top
                new GridPosition(oddRow ? +1 : -1, -1), //bottom
            };
            return neighbours;
        }

        public GridPosition(int x, int z) {
            this.X = x;
            this.Z = z;
        }

        public override string ToString() {
            return $"[{X},{Z}]";
        }

        public static GridPosition operator +(GridPosition a, GridPosition b) {
            return new GridPosition(a.X + b.X, a.Z + b.Z);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b) {
            return new GridPosition(a.X - b.X, a.Z - b.Z);
        }

        public static bool operator ==(GridPosition a, GridPosition b) {
            return a.X == b.X && a.Z == b.Z;
        }

        public static bool operator !=(GridPosition a, GridPosition b) {
            return !(a == b);
        }

        public bool Equals(GridPosition other) {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj) {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Z);
        }
    }

    public struct GridDimension {
        public readonly int Width;
        public readonly int Length;
        public readonly float CellSize;

        public GridDimension(int width, int length, float cellSize) {
            Width = width;
            Length = length;
            CellSize = cellSize;
        }
    }
}