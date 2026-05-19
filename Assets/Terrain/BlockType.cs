using System.Collections.Generic;
using Terrain.SpriteAtlas;
using UnityEngine;

namespace Terrain
{
    public abstract class BlockType : ScriptableObject
    {
        [SerializeField] public string blockName;

        public virtual MeshType MeshType => MeshType.Cube;
        public virtual bool FullBlock => true;
        public abstract AtlasSource GetSource(Direction face);
        public abstract IEnumerable<AtlasSource> GetSources();
        public abstract BlockMesh GetMesh();
        
        public int Index { get; set; }
    }
}
