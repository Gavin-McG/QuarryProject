using UnityEngine;

namespace Terrain.Generators
{
    /// <summary>
    /// Abstract base class for an object to generate terrain
    /// </summary>
    public abstract class TerrainGenerator : ScriptableObject
    {
        /// <summary>
        /// Generate the terrain to use
        /// </summary>
        /// <returns>A 3D multidimensional array of block types. null represents nothing</returns>
        public abstract BlockType[,,] GenerateTerrain(Vector3Int position, Vector3Int size);
    }
}
