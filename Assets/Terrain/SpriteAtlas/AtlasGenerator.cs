using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Terrain.SpriteAtlas
{
    public static class AtlasGenerator
    {
        public static Texture2D GenerateAtlas(HashSet<AtlasSource> sources)
        {
            Queue<Tile> tile16Queue = new();
            Queue<Tile> tile32Queue = new();
            Queue<Tile> tile64Queue = new();

            // Sort sources into lists
            foreach (AtlasSource source in sources)
            {
                Vector2Int resolution = source.GetResolution();
                if (resolution.x == 16 && resolution.y == 16)
                {
                    tile16Queue.Enqueue(new TileSource(source));
                }
                else if (resolution.x == 32 && resolution.y == 32)
                {
                    tile32Queue.Enqueue(new TileSource(source));
                }
                else if (resolution.x == 64 && resolution.y == 64)
                {
                    tile64Queue.Enqueue(new TileSource(source));
                }
                else
                {
                    Debug.LogError("Invalid resolution: " + resolution);
                }
            }
            
            // Combine 16s into groups
            while (tile16Queue.Count > 0)
            {
                List<Tile> group = new List<Tile>();
                while (tile16Queue.Count > 0 && group.Count < 4)
                {
                    Tile tile16 = tile16Queue.Dequeue();
                    group.Add(tile16);
                }
                tile32Queue.Enqueue(new TileGroup(group));
            }
            
            // Combine 32s into groups
            while (tile32Queue.Count > 0)
            {
                List<Tile> group = new List<Tile>();
                while (tile32Queue.Count > 0 && group.Count < 4)
                {
                    Tile tile32 = tile32Queue.Dequeue();
                    group.Add(tile32);
                }
                tile64Queue.Enqueue(new TileGroup(group));
            }
            
            // Create texture
            List<Tile> finalTiles = tile64Queue.ToList();
            float log2 = (int)Mathf.Ceil(Mathf.Log(finalTiles.Count, 2));
            int atlasWidth = (int)Mathf.Pow(2, Mathf.Ceil(log2 / 2)) * 64;
            int atlasHeight = (int)Mathf.Pow(2, Mathf.Floor(log2 / 2)) * 64;
            
            Texture2D atlas = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, false);
            atlas.filterMode = FilterMode.Point;
            for (int i = 0; i < finalTiles.Count; i++)
            {
                int x = (i * 64) % atlasWidth;
                int y = (i * 64) / atlasWidth * 64;
                RectInt area = new RectInt(x, y, 64, 64);
                finalTiles[i].WriteToTexture(atlas, area);
            }

            atlas.Apply();
            return atlas;
        }
    }
}
