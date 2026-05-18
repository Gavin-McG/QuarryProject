using System;
using Terrain.Blocks;
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

    public struct TerrainData
    {
        public Vector3Int chunkSize;
        [ReadOnly] public NativeArray<BlockInfo> blocks;
        [ReadOnly] public NativeArray<BlockInfo> upBlocks;
        [ReadOnly] public NativeArray<BlockInfo> downBlocks;
        [ReadOnly] public NativeArray<BlockInfo> leftBlocks;
        [ReadOnly] public NativeArray<BlockInfo> rightBlocks;
        [ReadOnly] public NativeArray<BlockInfo> forwardBlocks;
        [ReadOnly] public NativeArray<BlockInfo> backBlocks;
    }
    
    /// <summary>
    /// Count the number of Vertices and Indices each block will require
    /// </summary>
    [BurstCompile]
    public struct TerrainCountsJob : IJobFor
    {
        public TerrainData terrainData;

        public NativeArray<int> vertexCounts;
        public NativeArray<int> indexCounts;
        
        private int PositionToIndex(int x, int y, int z)
        {
            return (x) + 
                   (y * terrainData.chunkSize.x) + 
                   (z * terrainData.chunkSize.x * terrainData.chunkSize.y);
        }

        private void CheckFace(int index, int x, int y, int z)
        {
            // Check Left chunk
            if (x < 0)
            {
                int leftCheckIndex = PositionToIndex(x + terrainData.chunkSize.x,y,z);
                if (terrainData.leftBlocks[leftCheckIndex].blockIndex == -1) CountFace(index);
            }
            //Check Right chunk
            else if (x >= terrainData.chunkSize.x)
            {
                int rightCheckIndex = PositionToIndex(x - terrainData.chunkSize.x,y,z);
                if (terrainData.rightBlocks[rightCheckIndex].blockIndex == -1) CountFace(index);
            }
            // Check Down chunk
            else if (y < 0)
            {
                int downCheckIndex = PositionToIndex(x,y + terrainData.chunkSize.y,z);
                if (terrainData.downBlocks[downCheckIndex].blockIndex == -1) CountFace(index);
            }
            // Check Up chunk
            else if (y >= terrainData.chunkSize.y)
            {
                int upCheckIndex = PositionToIndex(x,y - terrainData.chunkSize.y,z);
                if (terrainData.upBlocks[upCheckIndex].blockIndex == -1) CountFace(index);
            }
            // Check Back chunk
            else if (z < 0)
            {
                int backCheckIndex = PositionToIndex(x,y,z + terrainData.chunkSize.z);
                if (terrainData.backBlocks[backCheckIndex].blockIndex == -1) CountFace(index);
            }
            // Check Front chunk
            else if (z >= terrainData.chunkSize.z)
            {
                int frontCheckIndex = PositionToIndex(x,y,z - terrainData.chunkSize.z);
                if (terrainData.forwardBlocks[frontCheckIndex].blockIndex == -1) CountFace(index);
            }
            // Check Main chunk
            else
            {
                int checkIndex = PositionToIndex(x,y,z);
                if (terrainData.blocks[checkIndex].blockIndex == -1) CountFace(index);
            }
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
            if (terrainData.blocks[index].blockIndex == -1) return;
            
            // Get position from index
            int x = index % terrainData.chunkSize.x;
            int y = (index / terrainData.chunkSize.x) % terrainData.chunkSize.y;
            int z = index / (terrainData.chunkSize.x * terrainData.chunkSize.y);
            
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
        public TerrainData terrainData;

        [ReadOnly] public NativeArray<BlockTypeInfo> blockTypes;
        [ReadOnly] public NativeArray<int> vertexOffsets;
        [ReadOnly] public NativeArray<int> indexOffsets;

        public Mesh.MeshData meshData;

        private static int PositionToIndex(int x, int y, int z, int xSize, int ySize)
        {
            return (x) +
                   (y * xSize) +
                   (z * xSize * ySize);
        }

        public void Execute(int index)
        {
            int blockIndex = terrainData.blocks[index].blockIndex;
            Rotation blockRotation = terrainData.blocks[index].rotation;

            // Skip air
            if (blockIndex == -1)
                return;

            NativeArray<TerrainVertex> vertices = meshData.GetVertexData<TerrainVertex>(0);
            NativeArray<int> indices = meshData.GetIndexData<int>();

            int vertexOffset = vertexOffsets[index];
            int indexOffset = indexOffsets[index];

            int currentVertex = vertexOffset;
            int currentIndex = indexOffset;

            // Cache locals
            int localXSize = terrainData.chunkSize.x;
            int localYSize = terrainData.chunkSize.y;
            int localZSize = terrainData.chunkSize.z;
            
            NativeArray<BlockInfo> blocks = terrainData.blocks;
            NativeArray<BlockInfo> upBlocks = terrainData.upBlocks;
            NativeArray<BlockInfo> downBlocks = terrainData.downBlocks;
            NativeArray<BlockInfo> rightBlocks = terrainData.rightBlocks;
            NativeArray<BlockInfo> leftBlocks = terrainData.leftBlocks;
            NativeArray<BlockInfo> forwardBlocks = terrainData.forwardBlocks;
            NativeArray<BlockInfo> backBlocks = terrainData.backBlocks;

            // Convert index -> position
            int x = index % localXSize;
            int y = (index / localXSize) % localYSize;
            int z = index / (localXSize * localYSize);

            Vector3 pos = new Vector3(x, y, z);

            // +Z
            TryAddFace(
                0, 0, 1,
                blockTypes[blockIndex].GetFace(Direction.Forward, blockRotation),
                Rotation.Degrees0,
                Vector3.forward,
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1)
            );

            // -Z
            TryAddFace(
                0, 0, -1,
                blockTypes[blockIndex].GetFace(Direction.Back, blockRotation),
                Rotation.Degrees0,
                Vector3.back,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            );

            // -X
            TryAddFace(
                -1, 0, 0,
                blockTypes[blockIndex].GetFace(Direction.Left, blockRotation),
                Rotation.Degrees0,
                Vector3.left,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0)
            );

            // +X
            TryAddFace(
                1, 0, 0,
                blockTypes[blockIndex].GetFace(Direction.Right, blockRotation),
                Rotation.Degrees0,
                Vector3.right,
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1)
            );

            // +Y
            TryAddFace(
                0, 1, 0,
                blockTypes[blockIndex].GetFace(Direction.Up, blockRotation),
                blockRotation,
                Vector3.up,
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
            );

            // -Y
            TryAddFace(
                0, -1, 0,
                blockTypes[blockIndex].GetFace(Direction.Down, blockRotation),
                blockRotation,
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
                BlockFaceInfo faceInfo,
                Rotation rotation,
                Vector3 normal,
                Vector3 v0,
                Vector3 v1,
                Vector3 v2,
                Vector3 v3)
            {
                int nx = x + dx;
                int ny = y + dy;
                int nz = z + dz;

                bool shouldRender = false;

                // Check Left chunk
                if (nx < 0)
                {
                    int neighborIndex = PositionToIndex(
                        nx + localXSize, ny, nz,
                        localXSize, localYSize
                    );

                    shouldRender =
                        leftBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Right chunk
                else if (nx >= localXSize)
                {
                    int neighborIndex = PositionToIndex(
                        nx - localXSize, ny, nz,
                        localXSize, localYSize
                    );

                    shouldRender = rightBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Down chunk
                else if (ny < 0)
                {
                    int neighborIndex = PositionToIndex(
                        nx, ny + localYSize, nz,
                        localXSize, localYSize
                    );

                    shouldRender = downBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Up chunk
                else if (ny >= localYSize)
                {
                    int neighborIndex = PositionToIndex(
                        nx, ny - localYSize, nz,
                        localXSize, localYSize
                    );

                    shouldRender = upBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Back chunk
                else if (nz < 0)
                {
                    int neighborIndex = PositionToIndex(
                        nx, ny, nz + localZSize,
                        localXSize, localYSize
                    );

                    shouldRender = backBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Forward chunk
                else if (nz >= localZSize)
                {
                    int neighborIndex = PositionToIndex(
                        nx, ny, nz - localZSize,
                        localXSize, localYSize
                    );

                    shouldRender = forwardBlocks[neighborIndex].blockIndex == -1;
                }
                // Check Main chunk
                else
                {
                    int neighborIndex = PositionToIndex(
                        nx, ny, nz,
                        localXSize, localYSize
                    );

                    shouldRender = blocks[neighborIndex].blockIndex == -1;
                }

                if (!shouldRender)
                    return;

                AddFace(faceInfo, rotation, normal, v0, v1, v2, v3);
            }

            void AddFace(
                BlockFaceInfo faceInfo,
                Rotation rotation,
                Vector3 normal,
                Vector3 v0,
                Vector3 v1,
                Vector3 v2,
                Vector3 v3)
            {
                int rotationIndex = RotationUtility.GetRotationIndex(rotation);

                Span<Vector2> uvs = stackalloc Vector2[4]
                {
                    new(faceInfo.uMin, faceInfo.vMin),
                    new(faceInfo.uMax, faceInfo.vMin),
                    new(faceInfo.uMax, faceInfo.vMax),
                    new(faceInfo.uMin, faceInfo.vMax)
                };

                vertices[currentVertex + 0] = new TerrainVertex
                {
                    position = pos + v0,
                    normal = normal,
                    uv = uvs[(0 + rotationIndex) & 3]
                };

                vertices[currentVertex + 1] = new TerrainVertex
                {
                    position = pos + v1,
                    normal = normal,
                    uv = uvs[(1 + rotationIndex) & 3]
                };

                vertices[currentVertex + 2] = new TerrainVertex
                {
                    position = pos + v2,
                    normal = normal,
                    uv = uvs[(2 + rotationIndex) & 3]
                };

                vertices[currentVertex + 3] = new TerrainVertex
                {
                    position = pos + v3,
                    normal = normal,
                    uv = uvs[(3 + rotationIndex) & 3]
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
    public class TerrainChunk : MonoBehaviour
    {
        [SerializeField] private Material material;

        private TerrainManager manager;

        public void UpdateTerrain(TerrainManager terrainManager, TerrainData data)
        {
            manager = terrainManager;

            // Create terrain mesh
            Mesh terrainMesh = GenerateTerrainMesh(data);

            // Assign to components
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = terrainMesh;

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            material.SetTexture("_BaseMap", BlockData.GetBlockAtlas());
            meshRenderer.material = material;

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = terrainMesh;
        }

        private Mesh GenerateTerrainMesh(TerrainData terrainData)
        {
            float startTime = Time.realtimeSinceStartup;
            
            // Create Containers for the process
            int terrainLength = terrainData.chunkSize.x * terrainData.chunkSize.y * terrainData.chunkSize.z; 
            NativeArray<int> vertexCounts = new NativeArray<int>(terrainLength, Allocator.TempJob);
            NativeArray<int> indexCounts = new NativeArray<int>(terrainLength, Allocator.TempJob);
            NativeArray<int> vertexOffsets = new NativeArray<int>(terrainLength, Allocator.TempJob);
            NativeArray<int> indexOffsets = new NativeArray<int>(terrainLength, Allocator.TempJob);
            NativeReference<int> vertexCount = new NativeReference<int>(0, Allocator.TempJob);
            NativeReference<int> indexCount = new NativeReference<int>(0, Allocator.TempJob);
            NativeArray<VertexAttributeDescriptor> vertexAttributes = TerrainVertex.GetVertexAttributes();
            
            // Allocate MeshData
            var meshArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshArray[0];
            
            // Create Jobs
            TerrainCountsJob countsJob = new()
            {
                terrainData =  terrainData,
                vertexCounts = vertexCounts,
                indexCounts = indexCounts,
            };
            TerrainPrefixSumJob prefixSumJob = new()
            {
                elements = terrainLength,
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
                terrainData = terrainData,
                blockTypes = BlockData.blockInfos,
                vertexOffsets = vertexOffsets,
                indexOffsets = indexOffsets,
                meshData = meshData,
            };
            
            // Schedule/Complete Jobs
            JobHandle countsHandle = countsJob.ScheduleParallel(terrainLength, 64, new JobHandle());
            JobHandle prefixSumHandle = prefixSumJob.Schedule(countsHandle);
            JobHandle setParamsHandle = setParamsJob.Schedule(prefixSumHandle);
            JobHandle generateMeshHandle = generateMeshJob.ScheduleParallel(terrainLength, 64, setParamsHandle);
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
    }
}