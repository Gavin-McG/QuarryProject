using Terrain.Blocks;
using Unity.Collections;
using UnityEngine;

namespace Terrain.Generators
{
    /// <summary>
    /// The most simple Terrain Generator. Create a solid rectangle of a single block
    /// </summary>
    [CreateAssetMenu(fileName = "SolidTerrainGenerator", menuName = "Scriptable Objects/Terrain Generator/Solid Terrain")]
    public class SolidTerrainGenerator : TerrainGenerator
    {
        [SerializeField] private BlockType block;

        public override NativeArray<int> GenerateTerrain(Vector3Int position, Vector3Int size)
        {
            // Create the multidimensional array
            int totalSize = size.x * size.y * size.z;
            NativeArray<int> terrain = new NativeArray<int>(totalSize, Allocator.Persistent);

            // Populate the array with the block type
            int blockIndex = block.Index;
            for (int i = 0; i < totalSize; i++)
                terrain[i] = blockIndex;

            // Return the array
            return terrain;
        }
    }
}
