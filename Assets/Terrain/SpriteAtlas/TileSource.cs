
using UnityEngine;

namespace Terrain.SpriteAtlas
{
    public class TileSource : Tile
    {
        private readonly AtlasSource source;

        public TileSource(AtlasSource source)
        {
            this.source = source;
        }

        public override void WriteToTexture(Texture2D texture, RectInt area)
        {
            source.WriteToTexture(texture, area);
        }
    }
}