using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public enum Direction { Up, Down, Left, Right, Forward, Back };
    public enum Rotation { Degrees0, Degrees90, Degrees180, Degrees270 };

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
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
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

    public static class RotationUtility
    {
        private static readonly Direction[] HorizontalDirections =
        {
            Direction.Right,
            Direction.Forward,
            Direction.Left,
            Direction.Back
        };

        private static readonly Rotation[] Rotations =
        {
            Rotation.Degrees0,
            Rotation.Degrees90,
            Rotation.Degrees180,
            Rotation.Degrees270
        };

        public static int GetRotationIndex(Rotation rotation) => rotation switch
        {
            Rotation.Degrees0 => 0,
            Rotation.Degrees90 => 1,
            Rotation.Degrees180 => 2,
            Rotation.Degrees270 => 3,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static int GetDirectionIndex(Direction direction) => direction switch
        {
            Direction.Right => 0,
            Direction.Forward => 1,
            Direction.Left => 2,
            Direction.Back => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

        public static Direction Rotate(Direction direction, Rotation rotation)
        {
            // Y-axis rotation does not affect vertical directions
            if (direction == Direction.Up || direction == Direction.Down)
                return direction;

            int index = GetDirectionIndex(direction);

            int rotationSteps = (int)rotation;

            return HorizontalDirections[(index + rotationSteps) % 4];
        }
        
        public static Vector3Int Rotate(Vector3Int position, Rotation rotation) => rotation switch
        {
            Rotation.Degrees0 => position,
            Rotation.Degrees90 => new Vector3Int(-position.z, position.y, position.x),
            Rotation.Degrees180 => new Vector3Int(-position.x, position.y, -position.z),
            Rotation.Degrees270 => new Vector3Int(position.z, position.y, -position.x),
            _ => throw new ArgumentOutOfRangeException(nameof(rotation))
        };

        public static Rotation Rotate(Rotation rotation1, Rotation rotation2)
        {
            int rotationSum = (GetRotationIndex(rotation1) + GetRotationIndex(rotation2)) % Rotations.Length;
            return Rotations[rotationSum];
        }

        public static Rotation GetRotation(Direction direction) => direction switch
        {
            Direction.Left => Rotation.Degrees180,
            Direction.Right => Rotation.Degrees0,
            Direction.Forward => Rotation.Degrees90,
            Direction.Back => Rotation.Degrees270,
            _ => Rotation.Degrees0
        };

        public static Rotation GetAngle(Direction direction1, Direction direction2)
        {
            // Default to 0 when non XZ axis
            if (direction1 == Direction.Up || direction1 == Direction.Down || 
                direction2 == Direction.Up || direction2 == Direction.Down) 
                return Rotation.Degrees0;
            
            int direction1Index = GetDirectionIndex(direction1);
            int direction2Index = GetDirectionIndex(direction2);
            
            int offset = (direction2Index - direction1Index + 4) % 4;

            return offset switch
            {
                0 => Rotation.Degrees0,
                1 => Rotation.Degrees90,
                2 => Rotation.Degrees180,
                3 => Rotation.Degrees270,
                _ => Rotation.Degrees0
            };
        }

        public static Quaternion GetQuaternion(Rotation rotation) => rotation switch
        {
            Rotation.Degrees0 => Quaternion.identity,
            Rotation.Degrees90 => Quaternion.Euler(0, 90, 0),
            Rotation.Degrees180 => Quaternion.Euler(0, 180, 0),
            Rotation.Degrees270 => Quaternion.Euler(0, 270, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(rotation))
        };
    }
    
}