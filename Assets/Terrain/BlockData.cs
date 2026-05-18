using System;
using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.SpriteAtlas;
using Unity.Collections;
using UnityEngine;

namespace Terrain
{
    public static class BlockData
    {

        private static BlockType[] blocks;
        private static Texture2D blockAtlas;
        private static readonly Dictionary<AtlasSource, Rect> sourceUVs = new();
        
        public static NativeArray<BlockTypeInfo> blockInfos;

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
            HashSet<AtlasSource> sources = new();
            foreach (var block in blocks)
            {
                var blockSources = block.GetSources();
                foreach (var source in blockSources)
                {
                    sources.Add(source);
                }
            }
            blockAtlas = AtlasGenerator.GenerateAtlas(sources);
            
            // Construct Block Info Array
            blockInfos = new NativeArray<BlockTypeInfo>(blocks.Length, Allocator.Persistent);
            for (int i = 0; i < blocks.Length; i++)
            {
                blockInfos[i] = new BlockTypeInfo()
                {
                    upFace = GetFaceData(blocks[i], Direction.Up),
                    downFace = GetFaceData(blocks[i], Direction.Down),
                    leftFace = GetFaceData(blocks[i], Direction.Left),
                    rightFace = GetFaceData(blocks[i], Direction.Right),
                    forwardFace = GetFaceData(blocks[i], Direction.Forward),
                    backFace = GetFaceData(blocks[i], Direction.Back)
                };

                BlockFaceInfo GetFaceData(BlockType block, Direction direction)
                {
                    var source = block.GetSource(direction);
                    Rect UVs = sourceUVs.GetValueOrDefault(source);
                    return new BlockFaceInfo()
                    {
                        uMin = UVs.xMin,
                        uMax = UVs.xMax,
                        vMin = UVs.yMin,
                        vMax = UVs.yMax,
                    };
                }
            }
        }

        public static void SetSourceUV(AtlasSource source, Rect uv)
        {
            sourceUVs[source] = uv;
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
