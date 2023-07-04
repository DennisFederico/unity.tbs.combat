using System;
using System.Collections.Generic;

namespace narkdagas.tbcs.grid {
    public readonly struct GridPosition : IEquatable<GridPosition> {
        public readonly int X;
        public readonly int Z;
        public readonly int FloorNumber;

        public static List<GridPosition> GetHexGridPositionsNeighbours(int floorNumber) {
            return new() {
                new GridPosition(-1, 1, floorNumber), //left-top
                new GridPosition(0, 1, floorNumber), //top
                new GridPosition(1, 1, floorNumber), //right-top
                new GridPosition(1, 0, floorNumber), //right
                new GridPosition(1, -1, floorNumber), //right-bottom
                new GridPosition(0, -1, floorNumber), //bottom
                new GridPosition(-1, -1, floorNumber), //left-bottom
                new GridPosition(-1, 0, floorNumber) //left
            };
        }

        public static List<GridPosition> GetHexGridPositionsNeighbours(int floorNumber, bool oddRow) {
            List<GridPosition> neighbours = new() {
                new GridPosition(-1, 0, floorNumber), //left
                new GridPosition(+1, 0, floorNumber), //right
                new GridPosition(0, +1, floorNumber), //top
                new GridPosition(0, -1, floorNumber), //bottom
                new GridPosition(oddRow ? +1 : -1, +1, floorNumber), //top
                new GridPosition(oddRow ? +1 : -1, -1, floorNumber), //bottom
            };
            return neighbours;
        }

        public GridPosition(int x, int z, int floorNumber) {
            this.X = x;
            this.Z = z;
            this.FloorNumber = floorNumber;
        }

        public override string ToString() {
            return $"Floor:{FloorNumber} [{X},{Z}]";
        }

        public static GridPosition operator +(GridPosition a, GridPosition b) {
            return new GridPosition(a.X + b.X, a.Z + b.Z, a.FloorNumber + b.FloorNumber);
        }
        
        public static GridPosition operator %(GridPosition a, GridPosition b) {
            return new GridPosition(a.X + b.X, a.Z + b.Z, a.FloorNumber);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b) {
            return new GridPosition(a.X - b.X, a.Z - b.Z, a.FloorNumber - b.FloorNumber);
        }

        public static bool operator ==(GridPosition a, GridPosition b) {
            return a.X == b.X && a.Z == b.Z;
        }

        public static bool operator !=(GridPosition a, GridPosition b) {
            return !(a == b);
        }

        public bool Equals(GridPosition other) {
            return FloorNumber == other.FloorNumber &&
                   X == other.X && 
                   Z == other.Z;
        }

        public override bool Equals(object obj) {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Z, FloorNumber);
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