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

        [SerializeField] public Vector2 uvPosition;
        [SerializeField] public Vector2 uvScale;
    }
}