using System;
using UnityEngine;

namespace Terrain
{
    public static class BlockData
    {

        private static BlockType[] blocks;
        private static Texture2D blockAtlas;

        private static BlockType[] GetBlockTypes()
        {
            return Resources.LoadAll<BlockType>("");
        }

        public static void InitializeBlockData()
        {
            blocks = GetBlockTypes();

            // Assign block indices
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Index = i;
            }

            // Create spriteAtlas
            int maxWidth = 0;
            int maxHeight = 0;
            int atlasDim = (int)Mathf.Ceil(Mathf.Sqrt(blocks.Length));
            foreach (var block in blocks)
            {
                if (!block.sprite) continue;
                //Get the maximum height of the block sprites 
                maxWidth = Math.Max(maxWidth, (int)block.sprite.rect.width);
                maxHeight = Math.Max(maxHeight, (int)block.sprite.rect.height);
            }

            int textureWidth = maxWidth * atlasDim;
            int textureHeight = maxHeight * atlasDim;
            blockAtlas = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            blockAtlas.wrapMode = TextureWrapMode.Clamp;
            blockAtlas.filterMode = FilterMode.Point;

            // Copy sprites into texture atlas
            for (int i = 0; i < blocks.Length; i++)
            {
                Texture2D blockTex = blocks[i].sprite.texture;

                // Get source/dest positions
                int sourceX = (int)blocks[i].sprite.rect.x;
                int sourceY = (int)blocks[i].sprite.rect.y;
                int sourceW = (int)blocks[i].sprite.rect.width;
                int sourceH = (int)blocks[i].sprite.rect.height;

                int destX = (i % atlasDim) * maxWidth;
                int destY = (i / atlasDim) * maxHeight;

                // Copy sprite contents to atlas
                for (int y = 0; y < sourceW; y++)
                {
                    for (int x = 0; x < sourceH; x++)
                    {
                        blockAtlas.SetPixel(destX + x, destY + y, blockTex.GetPixel(sourceX + x, sourceY + y));
                    }
                }

                blockAtlas.Apply();

                // Assign the UV of the blockType
                float uvMinX = (float)destX / textureWidth;
                float uvMinY = (float)destY / textureHeight;
                float uvWidth = (float)sourceW / textureWidth;
                float uvHeight = (float)sourceH / textureHeight;

                Rect uvRect = new Rect(uvMinX, uvMinY, uvWidth, uvHeight);
                blocks[i].UVs = uvRect;
            }
        }

        public static BlockType GetBlock(int index)
        {
            return blocks[index];
        }

        public static Texture GetBlockAtlas() => blockAtlas;
    }
}
