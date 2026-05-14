using UnityEngine;

namespace Terrain
{
    [CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
    public class BlockType : ScriptableObject
    {
        [SerializeField] public string blockName;
        [SerializeField] public Sprite sprite;

        public int Index { get; set; }
        public Rect UVs { get; set; }

        public struct BlockTypeInfo
        {
            public float uMin, uMax, vMin, vMax;
        }
        
        public BlockTypeInfo Info => new BlockTypeInfo()
        {
            uMin = UVs.xMin, uMax = UVs.xMax,
            vMin = UVs.yMin, vMax = UVs.yMax,
        };
    }
}
