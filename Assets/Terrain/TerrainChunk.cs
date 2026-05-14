using System.Collections.Generic;
using ClickManager;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain
{

    public struct CountTerrainSizeJob : IJobFor
    {
        public int xSize, ySize, zSize;
        [ReadOnly] public NativeArray<BlockType.BlockTypeInfo> blocks;
        [ReadOnly] public NativeArray<int> blockIndexes;

        public NativeArray<int> vertexCounts;
        public NativeArray<int> indexCounts;
        
        public void Execute(int index)
        {
            int x = index % xSize;
            int y = (index / xSize) % ySize;
            int z = index / (xSize * ySize);
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
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            int width = size.x;
            int height = size.y;
            int depth = size.z;

            int vertexIndex = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int index = x + (y * size.x) + (z * size.x * size.y);
                        
                        BlockType block = BlockData.GetBlock(terrain[index]);
                        if (!block) continue;

                        Vector3 blockPos = new Vector3(x, y, z);

                        // Check all 6 directions
                        TryAddFace(x, y, z, Vector3.forward, blockPos, block); // +Z
                        TryAddFace(x, y, z, Vector3.back, blockPos, block); // -Z
                        TryAddFace(x, y, z, Vector3.left, blockPos, block); // -X
                        TryAddFace(x, y, z, Vector3.right, blockPos, block); // +X
                        TryAddFace(x, y, z, Vector3.up, blockPos, block); // +Y
                        TryAddFace(x, y, z, Vector3.down, blockPos, block); // -Y
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();

            return mesh;

            // Local helpers
            void TryAddFace(int x, int y, int z, Vector3 dir, Vector3 pos, BlockType block)
            {
                if (block.Index == 0)
                {
                    Debug.Log("Block 1");
                }
                
                int nx = x + (int)dir.x;
                int ny = y + (int)dir.y;
                int nz = z + (int)dir.z;

                // Add face if out of bounds
                if (nx < 0 || ny < 0 || nz < 0 || nx >= width || ny >= height || nz >= depth)
                {
                    AddFace(dir, pos, block);
                    return;
                }
                
                // Skip face if covered by block
                int index = nx + (ny * size.x) + (nz * size.x * size.y);
                if (terrain[index] != -1) return;

                // Add face if in-bounds and not covered
                AddFace(dir, pos, block);
            }

            void AddFace(Vector3 dir, Vector3 pos, BlockType block)
            {
                // Define quad corners based on direction
                Vector3[] quad = GetFaceVertices(dir, pos);

                vertices.AddRange(quad);

                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 0);

                Rect faceUVs = block.UVs;
                uvs.Add(new Vector2(faceUVs.xMin, faceUVs.yMin));
                uvs.Add(new Vector2(faceUVs.xMax, faceUVs.yMin));
                uvs.Add(new Vector2(faceUVs.xMax, faceUVs.yMax));
                uvs.Add(new Vector2(faceUVs.xMin, faceUVs.yMax));
                
                vertexIndex += 4;
            }

            Vector3[] GetFaceVertices(Vector3 dir, Vector3 pos)
            {
                // Each face is defined in local cube space [0,1]
                if (dir == Vector3.forward) // +Z
                {
                    return new Vector3[]
                    {
                        pos + new Vector3(0, 0, 1),
                        pos + new Vector3(1, 0, 1),
                        pos + new Vector3(1, 1, 1),
                        pos + new Vector3(0, 1, 1)
                    };
                }

                if (dir == Vector3.back) // -Z
                {
                    return new Vector3[]
                    {
                        pos + new Vector3(1, 0, 0),
                        pos + new Vector3(0, 0, 0),
                        pos + new Vector3(0, 1, 0),
                        pos + new Vector3(1, 1, 0)
                    };
                }

                if (dir == Vector3.left) // -X
                {
                    return new Vector3[]
                    {
                        pos + new Vector3(0, 0, 0),
                        pos + new Vector3(0, 0, 1),
                        pos + new Vector3(0, 1, 1),
                        pos + new Vector3(0, 1, 0)
                    };
                }

                if (dir == Vector3.right) // +X
                {
                    return new Vector3[]
                    {
                        pos + new Vector3(1, 0, 1),
                        pos + new Vector3(1, 0, 0),
                        pos + new Vector3(1, 1, 0),
                        pos + new Vector3(1, 1, 1)
                    };
                }

                if (dir == Vector3.up) // +Y
                {
                    return new Vector3[]
                    {
                        pos + new Vector3(0, 1, 1),
                        pos + new Vector3(1, 1, 1),
                        pos + new Vector3(1, 1, 0),
                        pos + new Vector3(0, 1, 0)
                    };
                }

                // down
                return new Vector3[]
                {
                    pos + new Vector3(0, 0, 0),
                    pos + new Vector3(1, 0, 0),
                    pos + new Vector3(1, 0, 1),
                    pos + new Vector3(0, 0, 1)
                };
            }
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