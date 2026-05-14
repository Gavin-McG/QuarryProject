using UnityEngine;

namespace Terrain.Generators
{
    /// <summary>
    /// The most simple Terrain Generator. Create a solid rectangle of a single block
    /// </summary>
    [CreateAssetMenu(fileName = "SolidTerrainGenerator", menuName = "Block Terrain/Generator/Checker Terrain")]
    public class CheckerTerrainGenerator : TerrainGenerator
    {
        [SerializeField] private BlockType block1;
        [SerializeField] private BlockType block2;

        public override BlockType[,,] GenerateTerrain(Vector3Int position, Vector3Int size)
        {
            // Create the multidimensional array
            BlockType[,,] terrain = new BlockType[size.x, size.y, size.z];

            // Populate the array with the block type
            int posOffset = position.x + position.y + position.z;

            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            for (int z = 0; z < size.z; z++)
                terrain[x, y, z] = ((x + y + z + posOffset) % 2 == 0) ? block1 : block2;

            // Return the array
            return terrain;
        }
    }
}