using System;
using Unity.Collections;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct BlockMeshInfo
    {
        [SerializeField] public int vertexOffset;
        [SerializeField] public int vertexCount;
        [SerializeField] public int indexOffset;
        [SerializeField] public int indexCount;

        // public BlockMeshInfo(Mesh mesh)
        // {
        //     // Copy vertex info
        //     var positions = mesh.vertices;
        //     var normals = mesh.normals;
        //     var uvs = mesh.uv;
        //     
        //     vertices = new NativeArray<TerrainVertex>(mesh.vertices.Length, Allocator.Persistent);
        //     for (int i = 0; i < mesh.vertices.Length; i++)
        //     {
        //         vertices[i] = new TerrainVertex()
        //         {
        //             position = positions[i],
        //             normal = normals[i],
        //             uv = uvs[i],
        //         };
        //     }
        //     
        //     // Copy index info
        //     var triangles = mesh.triangles;
        //     indices = new NativeArray<int>(triangles, Allocator.Persistent);
        // }
    }
}