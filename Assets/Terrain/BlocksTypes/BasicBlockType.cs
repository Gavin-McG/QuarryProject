using System.Collections.Generic;
using Terrain.SpriteAtlas;
using UnityEngine;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Basic Block", menuName = "Scriptable Objects/Blocks/Basic Block")]
    public class BasicBlockType : BlockType
    {
        [SerializeField] private Sprite sprite;
        
        public override AtlasSource GetSource(Direction face) => new(sprite);

        public override IEnumerable<AtlasSource> GetSources()
        {
            yield return new AtlasSource(sprite);
        }
    }
}
