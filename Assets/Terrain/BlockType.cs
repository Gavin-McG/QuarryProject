using System.Collections.Generic;
using Terrain.SpriteAtlas;
using UnityEngine;

namespace Terrain
{
    public abstract class BlockType : ScriptableObject
    {
        [SerializeField] public string blockName;
        
        public abstract AtlasSource GetSource(Direction face);
        public abstract IEnumerable<AtlasSource> GetSources();
        
        public int Index { get; set; }
    }
}
