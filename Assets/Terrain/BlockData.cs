using System;
using System.Collections.Generic;
using Terrain.Blocks;
using Unity.Collections;
using UnityEngine;

namespace Terrain
{
    public static class BlockData
    {

        private static BlockType[] blocks;
        private static Texture2D blockAtlas;
        private readonly static HashSet<Sprite> sprites = new();
        private readonly static Dictionary<Sprite, Rect> spriteUVs = new();
        
        public static NativeArray<BlockType.BlockTypeInfo> blockInfos;

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
            foreach (var block in blocks)
            {
                foreach (var direction in DirectionUtility.GetDirections())
                {
                    Sprite sprite = block.GetSprite(direction); 
                    if (!sprite) continue;
                    //Get the maximum height of the block sprites 
                    maxWidth = Math.Max(maxWidth, (int)sprite.rect.width);
                    maxHeight = Math.Max(maxHeight, (int)sprite.rect.height);
                    sprites.Add(sprite);
                }
            }
            int atlasDim = (int)Mathf.Ceil(Mathf.Sqrt(sprites.Count));

            int textureWidth = maxWidth * atlasDim;
            int textureHeight = maxHeight * atlasDim;
            blockAtlas = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            blockAtlas.wrapMode = TextureWrapMode.Clamp;
            blockAtlas.filterMode = FilterMode.Point;

            // Copy sprites into texture atlas
            int spriteIndex = 0;
            foreach (var sprite in sprites) {
                Texture2D blockTex = sprite.texture;

                // Get source/dest positions
                int sourceX = (int)sprite.rect.x;
                int sourceY = (int)sprite.rect.y;
                int sourceW = (int)sprite.rect.width;
                int sourceH = (int)sprite.rect.height;

                int destX = (spriteIndex % atlasDim) * maxWidth;
                int destY = (spriteIndex / atlasDim) * maxHeight;

                // Copy sprite contents to atlas
                for (int y = 0; y < sourceW; y++)
                    for (int x = 0; x < sourceH; x++)
                        blockAtlas.SetPixel(destX + x, destY + y, blockTex.GetPixel(sourceX + x, sourceY + y));
                
                // Assign the UV of the sprite
                float uvMinX = (float)destX / textureWidth;
                float uvMinY = (float)destY / textureHeight;
                float uvWidth = (float)sourceW / textureWidth;
                float uvHeight = (float)sourceH / textureHeight;

                Rect uvRect = new Rect(uvMinX, uvMinY, uvWidth, uvHeight);
                spriteUVs.Add(sprite, uvRect);
                spriteIndex++;
            }
            
            blockAtlas.Apply();
            
            // Construct Block Info Array
            blockInfos = new NativeArray<BlockType.BlockTypeInfo>(blocks.Length, Allocator.Persistent);
            for (int i = 0; i < blocks.Length; i++)
            {
                blockInfos[i] = new BlockType.BlockTypeInfo()
                {
                    upFace = GetFaceData(blocks[i], Direction.Up),
                    downFace = GetFaceData(blocks[i], Direction.Down),
                    leftFace = GetFaceData(blocks[i], Direction.Left),
                    rightFace = GetFaceData(blocks[i], Direction.Right),
                    forwardFace = GetFaceData(blocks[i], Direction.Forward),
                    backFace = GetFaceData(blocks[i], Direction.Back)
                };

                BlockType.BlockFaceData GetFaceData(BlockType block, Direction direction)
                {
                    Sprite sprite = block.GetSprite(direction);
                    Rect UVs = spriteUVs.GetValueOrDefault(sprite);
                    return new BlockType.BlockFaceData()
                    {
                        uMin = UVs.xMin,
                        uMax = UVs.xMax,
                        vMin = UVs.yMin,
                        vMax = UVs.yMax,
                    };
                }
            }
        }

        public static void ClearBlockData()
        {
            blockInfos.Dispose();
        }

        public static BlockType GetBlock(int index)
        {
            if (index < 0) return null;
            return blocks[index];
        }

        public static Texture GetBlockAtlas() => blockAtlas;
    }
}
