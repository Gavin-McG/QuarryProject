using System.Collections.Generic;
using Terrain.SpriteAtlas;
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
        
        public override AtlasSource GetSource(Direction face)
        {
            AxisDirection spriteAxis = face switch
            {
                Direction.Up => AxisDirection.YAxis,
                Direction.Down => AxisDirection.YAxis,
                Direction.Left => AxisDirection.XAxis,
                Direction.Right => AxisDirection.XAxis,
                _ => AxisDirection.ZAxis,
            };
            
            return spriteAxis == axis ? new AtlasSource(topSprite) : new AtlasSource(sideSprite);
        }
        
        public override IEnumerable<AtlasSource> GetSources()
        {
            yield return new AtlasSource(topSprite);
            yield return new AtlasSource(sideSprite);
        }
    }
}
