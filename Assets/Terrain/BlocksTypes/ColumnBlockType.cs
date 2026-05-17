using UnityEngine;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Column Block", menuName = "Scriptable Objects/Blocks/Column Block")]
    public class ColumnBlockType : BlockType
    {
        private enum AxisDirection { XAxis, YAxis, ZAxis }
        
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
