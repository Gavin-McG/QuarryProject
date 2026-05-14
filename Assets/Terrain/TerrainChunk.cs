using System.Collections.Generic;
using ClickManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain
{

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainChunk : MonoBehaviour, IReceiveClickCast
    {
        [SerializeField] private Material material;

        private TerrainManager manager;

        public void UpdateTerrain(TerrainManager terrainManager, BlockType[,,] terrain)
        {
            manager = terrainManager;

            // Create terrain mesh
            Mesh terrainMesh = GenerateTerrainMesh(terrain);

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
        private Mesh GenerateTerrainMesh(BlockType[,,] terrain)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            var colors = new List<Color>();

            int width = terrain.GetLength(0);
            int height = terrain.GetLength(1);
            int depth = terrain.GetLength(2);

            int vertexIndex = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        BlockType block = terrain[x, y, z];
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
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();

            return mesh;

            // Local helpers
            void TryAddFace(int x, int y, int z, Vector3 dir, Vector3 pos, BlockType block)
            {
                int nx = x + (int)dir.x;
                int ny = y + (int)dir.y;
                int nz = z + (int)dir.z;

                // If neighbor is inside bounds and solid → skip
                if (nx >= 0 && ny >= 0 && nz >= 0 &&
                    nx < width && ny < height && nz < depth &&
                    terrain[nx, ny, nz] != null)
                {
                    return;
                }

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

                colors.Add(block.color);
                colors.Add(block.color);
                colors.Add(block.color);
                colors.Add(block.color);

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