using System.Collections.Generic;
using ClickManager;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain
{
    public struct TerrainVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;

        public static NativeArray<VertexAttributeDescriptor> GetVertexAttributes()
        {
            NativeArray<VertexAttributeDescriptor> vertexAttributes = new NativeArray<VertexAttributeDescriptor>(3, Allocator.TempJob);
            vertexAttributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, stream: 0);
            vertexAttributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, stream: 0);
            vertexAttributes[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, stream: 0);
            return vertexAttributes;
        }
    }
    
    /// <summary>
    /// Count the number of Vertices and Indices each block will require
    /// </summary>
    [BurstCompile]
    public struct TerrainCountsJob : IJobFor
    {
        public int xSize, ySize, zSize;
        [ReadOnly] public NativeArray<int> blockIndexes;

        public NativeArray<int> vertexCounts;
        public NativeArray<int> indexCounts;
        
        private int PositionToIndex(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= xSize || y >= ySize || z >= zSize)
                return -1;
            
            return x + (y * xSize) + (z * xSize * ySize);
        }

        private void CheckFace(int index, int x, int y, int z)
        {
            int checkIndex = PositionToIndex(x,y,z);
            
            if (checkIndex == -1 || blockIndexes[checkIndex] == -1) 
                CountFace(index);
        }

        private void CountFace(int index)
        {
            vertexCounts[index] += 4;
            indexCounts[index] += 6;
        }
        
        public void Execute(int index)
        {
            // Set initial value
            vertexCounts[index] = 0;
            indexCounts[index] = 0;
            
            // Skip empty block
            if (blockIndexes[index] == -1) return;
            
            // Get position from index
            int x = index % xSize;
            int y = (index / xSize) % ySize;
            int z = index / (xSize * ySize);
            
            // Check 6 sides
            CheckFace(index,x+1,y,z);
            CheckFace(index,x-1,y,z);
            CheckFace(index,x,y+1,z);
            CheckFace(index,x,y-1,z);
            CheckFace(index,x,y,z+1);
            CheckFace(index,x,y,z-1);
        }
    }

    /// <summary>
    /// Use a prefix sum to compute the offset for each block within the vertex and index array
    /// </summary>
    [BurstCompile]
    public struct TerrainPrefixSumJob : IJob
    {
        public int elements;
        [ReadOnly] public NativeArray<int> vertexCounts;
        [ReadOnly] public NativeArray<int> indexCounts;
        
        public NativeArray<int> vertexSums;
        public NativeArray<int> indexSums;
        public NativeReference<int> vertexTotal;
        public NativeReference<int> indexTotal;
        
        public void Execute()
        {
            int vertexSum = 0;
            int indexSum = 0;
            for (int i = 0; i < elements; i++)
            {
                vertexSums[i] = vertexSum;
                indexSums[i] = indexSum;
                
                vertexSum += vertexCounts[i];
                indexSum += indexCounts[i];
            }
            vertexTotal.Value = vertexSum;
            indexTotal.Value = indexSum;
        }
    }

    /// <summary>
    /// Setup the parameters of the MeshData struct
    /// </summary>
    [BurstCompile]
    public struct TerrainSetParamsJob : IJob
    {
        [ReadOnly] public NativeArray<VertexAttributeDescriptor> vertexAttributes;
        [ReadOnly] public NativeReference<int> vertexCount;
        [ReadOnly] public NativeReference<int> indexCount;
        
        public Mesh.MeshData meshData;
        
        public void Execute()
        {
            meshData.SetVertexBufferParams(vertexCount.Value, vertexAttributes);
            meshData.SetIndexBufferParams(indexCount.Value, IndexFormat.UInt32);
        }
    }

    /// <summary>
    /// Populate the Vertex and index arrays of the MeshData
    /// </summary>
    [BurstCompile]
    public struct TerrainGenerateMeshJob : IJobFor
    {
        public int xSize, ySize, zSize;

        [ReadOnly] public NativeArray<int> blockIndexes;
        [ReadOnly] public NativeArray<BlockType.BlockTypeInfo> blockInfo;
        [ReadOnly] public NativeArray<int> vertexOffsets;
        [ReadOnly] public NativeArray<int> indexOffsets;

        public Mesh.MeshData meshData;

        private static int PositionToIndex(
            int x,
            int y,
            int z,
            int xSize,
            int ySize,
            int zSize)
        {
            if (x < 0 || y < 0 || z < 0 || x >= xSize || y >= ySize || z >= zSize)
                return -1;

            return x + (y * xSize) + (z * xSize * ySize);
        }

        public void Execute(int index)
        {
            int blockIndex = blockIndexes[index];

            // Skip air
            if (blockIndex == -1)
                return;

            NativeArray<TerrainVertex> vertices = meshData.GetVertexData<TerrainVertex>(0);
            NativeArray<int> indices = meshData.GetIndexData<int>();

            int vertexOffset = vertexOffsets[index];
            int indexOffset = indexOffsets[index];

            int currentVertex = vertexOffset;
            int currentIndex = indexOffset;

            // Convert index -> position
            int x = index % xSize;
            int y = (index / xSize) % ySize;
            int z = index / (xSize * ySize);

            Vector3 pos = new Vector3(x, y, z);

            BlockType.BlockTypeInfo info = blockInfo[blockIndex];

            // Cache locals to avoid capturing 'this'
            int localXSize = xSize;
            int localYSize = ySize;
            int localZSize = zSize;

            NativeArray<int> localBlockIndexes = blockIndexes;

            // +Z
            TryAddFace(
                0, 0, 1,
                Vector3.forward,
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1)
            );

            // -Z
            TryAddFace(
                0, 0, -1,
                Vector3.back,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            );

            // -X
            TryAddFace(
                -1, 0, 0,
                Vector3.left,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0)
            );

            // +X
            TryAddFace(
                1, 0, 0,
                Vector3.right,
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1)
            );

            // +Y
            TryAddFace(
                0, 1, 0,
                Vector3.up,
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
            );

            // -Y
            TryAddFace(
                0, -1, 0,
                Vector3.down,
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
            );

            void TryAddFace(
                int dx,
                int dy,
                int dz,
                Vector3 normal,
                Vector3 v0,
                Vector3 v1,
                Vector3 v2,
                Vector3 v3)
            {
                int neighborIndex = PositionToIndex(
                    x + dx,
                    y + dy,
                    z + dz,
                    localXSize,
                    localYSize,
                    localZSize
                );

                // Skip hidden faces
                if (neighborIndex != -1 && localBlockIndexes[neighborIndex] != -1)
                    return;

                AddFace(normal, v0, v1, v2, v3);
            }

            void AddFace(
                Vector3 normal,
                Vector3 v0,
                Vector3 v1,
                Vector3 v2,
                Vector3 v3)
            {
                vertices[currentVertex + 0] = new TerrainVertex
                {
                    position = pos + v0,
                    normal = normal,
                    uv = new Vector2(info.uMin, info.vMin)
                };

                vertices[currentVertex + 1] = new TerrainVertex
                {
                    position = pos + v1,
                    normal = normal,
                    uv = new Vector2(info.uMax, info.vMin)
                };

                vertices[currentVertex + 2] = new TerrainVertex
                {
                    position = pos + v2,
                    normal = normal,
                    uv = new Vector2(info.uMax, info.vMax)
                };

                vertices[currentVertex + 3] = new TerrainVertex
                {
                    position = pos + v3,
                    normal = normal,
                    uv = new Vector2(info.uMin, info.vMax)
                };

                indices[currentIndex + 0] = currentVertex + 0;
                indices[currentIndex + 1] = currentVertex + 1;
                indices[currentIndex + 2] = currentVertex + 2;

                indices[currentIndex + 3] = currentVertex + 2;
                indices[currentIndex + 4] = currentVertex + 3;
                indices[currentIndex + 5] = currentVertex + 0;

                currentVertex += 4;
                currentIndex += 6;
            }
        }
    }

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainChunk : MonoBehaviour, IReceiveClickCast
    {
        [SerializeField] private Material material;

        private TerrainManager manager;

        public void UpdateTerrain(TerrainManager terrainManager, NativeArray<int> terrain, Vector3Int size)
        {
            manager = terrainManager;

            // Create terrain mesh
            Mesh terrainMesh = GenerateTerrainMesh(terrain, size);

            // Assign to components
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = terrainMesh;

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            material.SetTexture("_BaseMap", BlockData.GetBlockAtlas());
            meshRenderer.material = material;

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = terrainMesh;
        }

        //TODO Don't generate faces based on neighboring chunks
        private Mesh GenerateTerrainMesh(NativeArray<int> terrain, Vector3Int size)
        {
            float startTime = Time.realtimeSinceStartup;
            
            // Create Containers for the process
            NativeArray<int> vertexCounts = new NativeArray<int>(terrain.Length, Allocator.TempJob);
            NativeArray<int> indexCounts = new NativeArray<int>(terrain.Length, Allocator.TempJob);
            NativeArray<int> vertexOffsets = new NativeArray<int>(terrain.Length, Allocator.TempJob);
            NativeArray<int> indexOffsets = new NativeArray<int>(terrain.Length, Allocator.TempJob);
            NativeReference<int> vertexCount = new NativeReference<int>(0, Allocator.TempJob);
            NativeReference<int> indexCount = new NativeReference<int>(0, Allocator.TempJob);
            NativeArray<VertexAttributeDescriptor> vertexAttributes = TerrainVertex.GetVertexAttributes();
            
            // Allocate MeshData
            var meshArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshArray[0];
            
            // Create Jobs
            TerrainCountsJob countsJob = new()
            {
                xSize = size.x,
                ySize = size.y,
                zSize = size.z,
                blockIndexes =  terrain,
                vertexCounts = vertexCounts,
                indexCounts = indexCounts,
            };
            TerrainPrefixSumJob prefixSumJob = new()
            {
                elements = terrain.Length,
                vertexCounts = vertexCounts,
                indexCounts = indexCounts,
                vertexSums = vertexOffsets,
                indexSums = indexOffsets,
                vertexTotal = vertexCount,
                indexTotal = indexCount,
            };
            TerrainSetParamsJob setParamsJob = new()
            {
                vertexCount = vertexCount,
                indexCount = indexCount,
                vertexAttributes = vertexAttributes,
                meshData = meshData,
            };
            TerrainGenerateMeshJob generateMeshJob = new()
            {
                xSize = size.x,
                ySize = size.y,
                zSize = size.z,
                blockIndexes = terrain,
                blockInfo = BlockData.blockInfos,
                vertexOffsets = vertexOffsets,
                indexOffsets = indexOffsets,
                meshData = meshData,
            };
            
            // Schedule/Complete Jobs
            JobHandle countsHandle = countsJob.ScheduleParallel(terrain.Length, 64, new JobHandle());
            JobHandle prefixSumHandle = prefixSumJob.Schedule(countsHandle);
            JobHandle setParamsHandle = setParamsJob.Schedule(prefixSumHandle);
            JobHandle generateMeshHandle = generateMeshJob.ScheduleParallel(terrain.Length, 64, setParamsHandle);
            generateMeshHandle.Complete();
            
            // Set Submesh
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount.Value));
            
            // Dispose of Native Containers
            vertexCounts.Dispose();
            indexCounts.Dispose();
            vertexOffsets.Dispose();
            indexOffsets.Dispose();
            vertexCount.Dispose();
            indexCount.Dispose();
            vertexAttributes.Dispose();
            
            // Create Mesh from MeshData
            Mesh mesh = new Mesh();
            Mesh.ApplyAndDisposeWritableMeshData(meshArray, mesh);
            mesh.RecalculateBounds();
            
            return mesh;
        }


        public bool ReceiveLeftPress(RaycastHit hit)
        {
            manager.ReceiveLeftPress(hit);
            return true;
        }
        
        public void ReleaseLeftPress()
        {
            manager.ReleaseLeftPress();
        }

        public bool ReceiveRightPress(RaycastHit hit)
        {
            manager.ReceiveRightPress(hit);
            return true;
        }

        public void ReleaseRightPress()
        {
            manager.ReleaseRightPress();
        }
    }
}