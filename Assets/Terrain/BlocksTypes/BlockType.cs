using UnityEngine;

namespace Terrain.Blocks
{
    public abstract class BlockType : ScriptableObject
    {
        [SerializeField] public string blockName;
        
        public abstract Sprite GetSprite(Direction direction);
        
        public int Index { get; set; }
        
        public struct BlockFaceData
        {
            public float uMin, uMax, vMin, vMax;
        }

        public struct BlockTypeInfo
        {
            public BlockFaceData upFace;
            public BlockFaceData downFace;
            public BlockFaceData leftFace;
            public BlockFaceData rightFace;
            public BlockFaceData forwardFace;
            public BlockFaceData backFace;

            public BlockFaceData GetFace(Direction direction) => direction switch
            {
                Direction.Up => upFace,
                Direction.Down => downFace,
                Direction.Left => leftFace,
                Direction.Right => rightFace,
                Direction.Forward => forwardFace,
                Direction.Back => backFace,
                _ => upFace,
            };
            
            public BlockFaceData GetFace(Direction direction, Rotation rotation)
            {
                Direction rotatedDirection = RotationUtility.Rotate(direction, rotation);
                return rotatedDirection switch
                {
                    Direction.Up => upFace,
                    Direction.Down => downFace,
                    Direction.Left => leftFace,
                    Direction.Right => rightFace,
                    Direction.Forward => forwardFace,
                    Direction.Back => backFace,
                    _ => upFace,
                };
            }
        }
    }
}
