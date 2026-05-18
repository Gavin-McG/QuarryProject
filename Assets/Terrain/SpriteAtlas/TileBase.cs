using UnityEngine;

namespace Terrain.SpriteAtlas
{
    public abstract class Tile
    {
        public abstract void WriteToTexture(Texture2D texture, RectInt area);
    }
}
