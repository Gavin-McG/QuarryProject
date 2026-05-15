using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Basic Block", menuName = "Scriptable Objects/Blocks/Column Block")]
    public class ColumnBlockType : BlockType
    {
        private enum AxisDirection { XAxis, YAxis, ZAxis }
        
        [SerializeField] public string blockName;
        [SerializeField] private AxisDirection axis;
        [SerializeField] private Sprite topSprite;
        [SerializeField] private Sprite sideSprite;
        
        public override Sprite GetSprite(Direction direction)
        {
            AxisDirection spriteAxis = direction switch
            {
                Direction.Up => AxisDirection.YAxis,
                Direction.Down => AxisDirection.YAxis,
                Direction.Left => AxisDirection.XAxis,
                Direction.Right => AxisDirection.XAxis,
                _ => AxisDirection.ZAxis,
            };
            
            return spriteAxis == axis ? topSprite : sideSprite;
        }
    }
}
