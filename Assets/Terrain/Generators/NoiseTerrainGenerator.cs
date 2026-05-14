using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Generators
{

    /// <summary>
    /// The most simple Terrain Generator. Create a solid rectangle of a single block
    /// </summary>
    [CreateAssetMenu(fileName = "SolidTerrainGenerator", menuName = "Block Terrain/Generator/Noise Terrain")]
    public class NoiseTerrainGenerator : TerrainGenerator
    {
        [Serializable]
        struct Layer
        {
            [SerializeField] public BlockType block;
            [SerializeField] public float noiseThreshold;
        }

        [Header("Noise Settings")] [SerializeField]
        private float noiseScale = 0.1f;

        [SerializeField] private List<Layer> layers;

        public override BlockType[,,] GenerateTerrain(Vector3Int position, Vector3Int size)
        {
            // Create the multidimensional array
            BlockType[,,] terrain = new BlockType[size.x, size.y, size.z];

            // Populate the array with the block type
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            for (int z = 0; z < size.z; z++)
            {
                float noise = PerlinNoise3D(x + position.x, y + position.y, z + position.z);
                terrain[x, y, z] = null;
                foreach (Layer layer in layers)
                {
                    if (noise < layer.noiseThreshold)
                    {
                        terrain[x, y, z] = layer.block;
                        break;
                    }
                }
            }

            // Return the array
            return terrain;
        }

        private float PerlinNoise3D(float x, float y, float z)
        {
            x += 100000;
            y += 200000;
            z += 300000;

            x *= noiseScale;
            y *= noiseScale;
            z *= noiseScale;

            float xy = Mathf.PerlinNoise(x, y);
            float xz = Mathf.PerlinNoise(x, z);
            float yz = Mathf.PerlinNoise(y, z);
            float yx = Mathf.PerlinNoise(y, x);
            float zx = Mathf.PerlinNoise(z, x);
            float zy = Mathf.PerlinNoise(z, y);

            return (xy + xz + yz + yx + zx + zy) / 6;
        }
    }
}
