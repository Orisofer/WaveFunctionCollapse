using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public static class TileUtility
    {
        public static readonly string UP = "up";
        public static readonly string DOWN = "down";
        public static readonly string LEFT = "left";
        public static readonly string RIGHT = "right";

        public static CellDirection GetOppositeDirection(CellDirection direction)
        {
            CellDirection oppositeDirection = default;
            
            switch (direction)
            {
                case CellDirection.Up:
                    oppositeDirection = CellDirection.Down;
                    break;
                case CellDirection.Down:
                    oppositeDirection = CellDirection.Up;
                    break;
                case CellDirection.Left:
                    oppositeDirection = CellDirection.Right;
                    break;
                case CellDirection.Right:
                    oppositeDirection = CellDirection.Left;
                    break;
            }

            return oppositeDirection;
        }
    }
}


