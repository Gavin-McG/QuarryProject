using System;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public class BlockMesh
    {
        [SerializeField] public Mesh mesh;
        [SerializeField] public Texture texture;
    }
}
