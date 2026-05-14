using UnityEngine;

namespace Terrain.Generators
{
    /// <summary>
    /// The most simple Terrain Generator. Create a solid rectangle of a single block
    /// </summary>
    [CreateAssetMenu(fileName = "SolidTerrainGenerator", menuName = "Block Terrain/Generator/Solid Terrain")]
    public class SolidTerrainGenerator : TerrainGenerator
    {
        [SerializeField] private BlockType block;

        public override BlockType[,,] GenerateTerrain(Vector3Int position, Vector3Int size)
        {
            // Create the multidimensional array
            BlockType[,,] terrain = new BlockType[size.x, size.y, size.z];

            // Populate the array with the block type
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            for (int z = 0; z < size.z; z++)
                terrain[x, y, z] = block;

            // Return the array
            return terrain;
        }
    }
}
