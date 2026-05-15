using UnityEngine;

namespace Terrain.Blocks
{
    public abstract class BlockType : ScriptableObject
    {
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
        }
    }
}
