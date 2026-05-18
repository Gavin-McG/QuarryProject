using System.Collections.Generic;
using UnityEngine;

namespace Terrain.SpriteAtlas
{
    public class TileGroup : Tile
    {
        private readonly List<Tile> tiles;

        public TileGroup(List<Tile> tiles)
        {
            this.tiles = tiles;
        }
        
        public override void WriteToTexture(Texture2D texture, RectInt area)
        {
            int halfWidth = area.width / 2;
            int halfHeight = area.height / 2;

            if (tiles.Count >= 1)
            {
                RectInt topLeft = new RectInt(area.x, area.y, halfWidth, halfHeight);
                tiles[0].WriteToTexture(texture, topLeft);
            }

            if (tiles.Count >= 2)
            {
                RectInt topRight = new RectInt(area.x + halfWidth, area.y, halfWidth, halfHeight);
                tiles[1].WriteToTexture(texture, topRight);
            }

            if (tiles.Count >= 3)
            {
                RectInt bottomLeft = new RectInt(area.x, area.y + halfHeight, halfWidth, halfHeight);
                tiles[2].WriteToTexture(texture, bottomLeft);
            }

            if (tiles.Count >= 4)
            {
                RectInt bottomRight = new RectInt(area.x + halfWidth, area.y + halfHeight, halfWidth, halfHeight);
                tiles[3].WriteToTexture(texture, bottomRight);
            }
        }
    }
}