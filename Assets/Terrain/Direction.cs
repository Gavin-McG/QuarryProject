using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public enum Direction { Up, Down, Left, Right, Forward, Back };

    public static class DirectionUtility
    {

        public static IEnumerable<Direction> GetDirections()
        {
            yield return Direction.Up;
            yield return Direction.Down;
            yield return Direction.Left;
            yield return Direction.Right;
            yield return Direction.Forward;
            yield return Direction.Back;
        }
        
        public static Direction Opposite(Direction direction) => direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Forward,
            Direction.Forward => Direction.Back,
            _ => Direction.Forward,
        };

        public static Vector3Int GetVector(Direction direction) => direction switch
        {
            Direction.Up => Vector3Int.up,
            Direction.Down => Vector3Int.down,
            Direction.Left => Vector3Int.left,
            Direction.Right => Vector3Int.right,
            Direction.Forward => Vector3Int.forward,
            _ => Vector3Int.back,
        };

        public static Vector3Int MovePosition(Vector3Int position, Direction direction)
        {
            return position + GetVector(direction);
        }
    }
    
}