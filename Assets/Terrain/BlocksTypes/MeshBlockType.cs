using System.Collections.Generic;
using Terrain.SpriteAtlas;
using UnityEngine;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Mesh Block", menuName = "Scriptable Objects/Blocks/Mesh Block")]
    public class MeshBlockType : BlockType
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Texture2D texture;
        
        public override MeshType GetMeshType() => MeshType.Mesh;
        
        public override AtlasSource GetSource(Direction face) => new(texture);

        public override IEnumerable<AtlasSource> GetSources()
        {
            yield return new AtlasSource(texture);
        }

        public override BlockMesh GetMesh()
        {
            return new BlockMesh()
            {
                mesh = mesh,
                texture = texture,
            };
        }
    }
}
