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
        private static readonly Dictionary<AtlasSource, Rect> sourceToUV = new();
        private static readonly Dictionary<Mesh, int> meshToIndex = new();
        
        public static NativeArray<BlockTypeInfo> blockInfos;
        public static NativeArray<BlockMeshInfo> meshInfos;
        public static NativeArray<TerrainVertex> meshVertices;
        public static NativeArray<int> meshIndices;

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
            
            // Collect Mesh Info
            List<BlockMeshInfo> meshList = new();
            int totalVertices = 0;
            int totalIndices = 0;
            foreach (var block in blocks)
            {
                if (block.GetMeshType() == MeshType.Mesh)
                {
                    // Add MeshInfo if not previously added
                    BlockMesh meshData = block.GetMesh();
                    if (!meshToIndex.ContainsKey(meshData.mesh))
                    {
                        meshToIndex[meshData.mesh] = meshList.Count;
                        int vertexCount = meshData.mesh.vertexCount;
                        int indexCount = (int)meshData.mesh.GetIndexCount(0);
                        Rect texRect = sourceToUV[new AtlasSource(meshData.texture)];
                        meshList.Add(new BlockMeshInfo()
                        {
                            vertexOffset = totalVertices,
                            vertexCount = vertexCount,
                            indexOffset = totalIndices,
                            indexCount = indexCount,
                            uvPosition = new Vector2(texRect.xMin, texRect.yMin),
                            uvScale = new Vector2(texRect.width, texRect.height)
                        });
                        totalVertices += vertexCount;
                        totalIndices += indexCount;
                    }
                }
            }
            
            // Create Mesh Data Arrays
            meshInfos = new NativeArray<BlockMeshInfo>(meshList.ToArray(), Allocator.Persistent);
            meshVertices = new NativeArray<TerrainVertex>(totalVertices, Allocator.Persistent);
            meshIndices = new NativeArray<int>(totalIndices, Allocator.Persistent);
            
            // Populate Mesh Arrays
            foreach (var pair in meshToIndex)
            {
                Mesh mesh = pair.Key;
                int index = pair.Value;
                BlockMeshInfo info = meshInfos[index];
                
                // copy vertices
                var positions = mesh.vertices;
                var normals = mesh.normals;
                var uvs = mesh.uv;
                
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    meshVertices[info.vertexOffset + i] = new TerrainVertex()
                    {
                        position = positions[i],
                        normal = normals[i],
                        uv = uvs[i],
                    };
                }
                
                // copy Indices
                var triangles = mesh.triangles;
                
                for (int i = 0; i < mesh.triangles.Length; i++)
                {
                    meshIndices[info.indexOffset + i] = triangles[i];
                }
            }
            
            // Construct Block Info Array
            blockInfos = new NativeArray<BlockTypeInfo>(blocks.Length, Allocator.Persistent);
            for (int i = 0; i < blocks.Length; i++)
            {
                MeshType meshType = blocks[i].GetMeshType();
                int meshIndex = meshType == MeshType.Mesh ? meshToIndex[blocks[i].GetMesh().mesh] : -1;
                blockInfos[i] = new BlockTypeInfo()
                {
                    type = meshType,
                    upFace = GetFaceData(blocks[i], Direction.Up),
                    downFace = GetFaceData(blocks[i], Direction.Down),
                    leftFace = GetFaceData(blocks[i], Direction.Left),
                    rightFace = GetFaceData(blocks[i], Direction.Right),
                    forwardFace = GetFaceData(blocks[i], Direction.Forward),
                    backFace = GetFaceData(blocks[i], Direction.Back),
                    meshIndex = meshIndex,
                };

                BlockFaceInfo GetFaceData(BlockType block, Direction direction)
                {
                    var source = block.GetSource(direction);
                    Rect UVs = sourceToUV.GetValueOrDefault(source);
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
            sourceToUV[source] = uv;
        }

        public static void ClearBlockData()
        {
            blockInfos.Dispose();
            meshInfos.Dispose();
            meshVertices.Dispose();
            meshIndices.Dispose();
        }

        public static BlockType GetBlock(int index)
        {
            if (index < 0) return null;
            return blocks[index];
        }

        public static Texture GetBlockAtlas() => blockAtlas;
    }
}
