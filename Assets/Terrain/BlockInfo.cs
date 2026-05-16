using System;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct BlockInfo
    {
        [SerializeField] public int blockIndex;
        [SerializeField] public Rotation rotation;
    }
}
