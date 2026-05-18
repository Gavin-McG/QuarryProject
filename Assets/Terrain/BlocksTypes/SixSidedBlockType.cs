using System.Collections.Generic;
using Terrain.SpriteAtlas;
using UnityEngine;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Six Sided Block", menuName = "Scriptable Objects/Blocks/Six Sided Block")]
    public class SixSidedBlockType : BlockType
    {
        [SerializeField] private Sprite topSprite;
        [SerializeField] private Sprite bottomSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite frontSprite;
        [SerializeField] private Sprite backSprite;

        public override AtlasSource GetSource(Direction face) => face switch
        {
            Direction.Up => new AtlasSource(topSprite),
            Direction.Down => new AtlasSource(bottomSprite),
            Direction.Left => new AtlasSource(leftSprite),
            Direction.Right => new AtlasSource(rightSprite),
            Direction.Forward => new AtlasSource(frontSprite),
            _ => new AtlasSource(backSprite),
        };
        
        public override IEnumerable<AtlasSource> GetSources()
        {
            yield return new AtlasSource(topSprite);
            yield return new AtlasSource(bottomSprite);
            yield return new AtlasSource(leftSprite);
            yield return new AtlasSource(rightSprite);
            yield return new AtlasSource(frontSprite);
            yield return new AtlasSource(backSprite);
        }
    }
}