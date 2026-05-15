using System;
using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Generators;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using ChunkCoord = UnityEngine.Vector3Int;
using ChunkPosition = UnityEngine.Vector3Int;

namespace Terrain
{
    public enum SurfaceDirection { Up, Down, Left, Right, Forward, Back }

    public struct TerrainHoverInfo
    {
        public bool overTerrain;
        public Vector3Int position;
        public SurfaceDirection direction;

        public Vector3Int BackPosition => position;

        public Vector3Int FrontPosition => direction switch
        {
            SurfaceDirection.Up => position + Vector3Int.up,
            SurfaceDirection.Down => position + Vector3Int.down,
            SurfaceDirection.Left => position + Vector3Int.left,
            SurfaceDirection.Right => position + Vector3Int.right,
            SurfaceDirection.Forward => position + Vector3Int.forward,
            SurfaceDirection.Back => position + Vector3Int.back,
            _ => position,
        };
    }

    public class TerrainManager : MonoBehaviour
    {
        [SerializeField] private TerrainChunk chunkPrefab;
        [SerializeField] private TerrainGenerator generator;
        [SerializeField] private Vector3Int chunkSize = new Vector3Int(16, 16, 16);
        [SerializeField] private LayerMask terrainMask;

        [SerializeField] private ChunkCoord initialChunkMin = new ChunkCoord(-2, -4, -2);
        [SerializeField] private ChunkCoord initialChunkMax = new ChunkCoord(2, 0, 2);

        class ChunkData
        {
            public TerrainChunk chunk;
            public NativeArray<int> blockIndex;

            ~ChunkData()
            {
                blockIndex.Dispose();
            }
        }

        private readonly Dictionary<ChunkCoord, ChunkData> chunks = new();
        private readonly HashSet<ChunkCoord> dirtyChunks = new();

        private void Start()
        {
            BlockData.InitializeBlockData();
            LoadInitialChunks();
        }

        private void OnDestroy()
        {
            BlockData.ClearBlockData();
        }

        /// <summary>
        /// Regenerate the terrain for each loaded chunk
        /// </summary>
        [ContextMenu("Reset Terrain")]
        private void ResetTerrain()
        {
            foreach (KeyValuePair<ChunkCoord, ChunkData> pair in chunks)
            {
                // Regenerate terrain
                Vector3Int position = pair.Key * chunkSize;
                NativeArray<int> chunkData = generator.GenerateTerrain(position, chunkSize);

                // Update chunk
                pair.Value.chunk.UpdateTerrain(this, chunkData, chunkSize);
                pair.Value.blockIndex.Dispose();
                pair.Value.blockIndex = chunkData;
            }
        }

        private void LoadInitialChunks()
        {
            for (int x = initialChunkMin.x; x < initialChunkMax.x; x++)
            for (int y = initialChunkMin.y; y < initialChunkMax.y; y++)
            for (int z = initialChunkMin.z; z < initialChunkMax.z; z++)
                LoadChunk(new ChunkCoord(x, y, z));
        }

        /// <summary>
        /// Loads a specific Chunk using the terrain generator
        /// </summary>
        private void LoadChunk(ChunkCoord chunkCoord)
        {
            // Create chunk object
            Vector3Int position = chunkCoord * chunkSize;
            TerrainChunk newChunk = Instantiate(chunkPrefab, position, Quaternion.identity, transform);

            // Generate chunk data
            NativeArray<int> chunkData = generator.GenerateTerrain(position, chunkSize);
            newChunk.UpdateTerrain(this, chunkData, chunkSize);

            // Add chunk
            chunks.Add(chunkCoord, new ChunkData()
            {
                chunk = newChunk,
                blockIndex = chunkData,
            });
        }

        bool RayCastTerrain(out RaycastHit hit)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);
            return Physics.Raycast(cameraRay.origin, cameraRay.direction, out hit, 1000, terrainMask);
        }
        
        #region Conversions

        /// <summary>
        /// Get the Chunk coord that a given block position falls under
        /// </summary>
        private ChunkCoord GetChunkCoord(Vector3Int position)
        {
            int FloorDiv(int a, int b)
            {
                int q = a / b;
                int r = a % b;

                if (r != 0 && ((r < 0) != (b < 0)))
                    q--;

                return q;
            }

            return new ChunkCoord(
                FloorDiv(position.x, chunkSize.x),
                FloorDiv(position.y, chunkSize.y),
                FloorDiv(position.z, chunkSize.z));
        }

        /// <summary>
        /// Get the positions of a Block position within its chunk
        /// </summary>
        private ChunkPosition GetChunkPosition(Vector3Int position)
        {
            int Mod(int a, int b)
            {
                int r = a % b;

                if (r < 0)
                    r += Math.Abs(b);

                return r;
            }

            return new ChunkPosition(
                Mod(position.x, chunkSize.x),
                Mod(position.y, chunkSize.y),
                Mod(position.z, chunkSize.z));
        }

        /// <summary>
        /// Converts a normal direction to SurfaceDirection based on largest component
        /// </summary>
        private static SurfaceDirection GetSurfaceDirection(Vector3 normal)
        {
            float absX = Mathf.Abs(normal.x);
            float absY = Mathf.Abs(normal.y);
            float absZ = Mathf.Abs(normal.z);
            
            if (absX > absY && absX > absZ) //X is largest component
                return normal.x > 0 ? SurfaceDirection.Right : SurfaceDirection.Left;
            if (absY > absZ) //Y is largest component
                return normal.y > 0 ? SurfaceDirection.Up : SurfaceDirection.Down;
            //Z is largest component
            return normal.z > 0 ? SurfaceDirection.Forward : SurfaceDirection.Back;
        }

        /// <summary>
        /// Converts continuous world position to block coordinate
        /// </summary>
        private Vector3Int GetBlockPosition(Vector3 position)
        {
            return new Vector3Int(
                Mathf.FloorToInt(position.x),
                Mathf.FloorToInt(position.y),
                Mathf.FloorToInt(position.z));
        }
        
        #endregion
        #region Public Methods

        /// <summary>
        /// Gets the block at a specific block position
        /// </summary>
        public BlockType GetBlock(Vector3Int position)
        {
            ChunkCoord chunkCoord = GetChunkCoord(position);

            // Get the block in the chunk if the chunk is loaded
            if (!chunks.TryGetValue(chunkCoord, out ChunkData chunkData)) return null;

            ChunkPosition chunkPosition = GetChunkPosition(position);
            int index = chunkPosition.x + (chunkPosition.y * chunkSize.x) + (chunkPosition.z * chunkSize.x * chunkSize.y);
            return BlockData.GetBlock(chunkData.blockIndex[index]);
        }

        /// <summary>
        /// Sets the block at a specific block position
        /// </summary>
        public void SetBlock(Vector3Int position, BlockType block, bool regenerateMesh = true)
        {
            ChunkCoord chunkCoord = GetChunkCoord(position);

            if (!chunks.TryGetValue(chunkCoord, out ChunkData chunkData))
            {
                Debug.LogWarning("TerrainManager: Attempting to write to a block in an unloaded chunk " + chunkCoord);
                return;
            }

            ChunkPosition chunkPosition = GetChunkPosition(position);
            int index = chunkPosition.x + (chunkPosition.y * chunkSize.x) + (chunkPosition.z * chunkSize.x * chunkSize.y);
            chunkData.blockIndex[index] = block?.Index ?? -1;

            if (regenerateMesh) {
                chunkData.chunk.UpdateTerrain(this, chunkData.blockIndex, chunkSize);
                //TODO update neighboring chunks 
            }
            else
            {
                dirtyChunks.Add(chunkCoord);
            }
        }

        public TerrainHoverInfo GetHoverInfo()
        {
            //TODO cache per-frame raycast result
            RaycastHit hit;
            if (RayCastTerrain(out hit))
            {
                // Mouse over terrain
                return new TerrainHoverInfo()
                {
                    overTerrain = true,
                    position = GetBlockPosition(hit.point - 0.2f * hit.normal),
                    direction = GetSurfaceDirection(hit.normal),
                };
            }
            // Mouse not over terrain
            return new TerrainHoverInfo()
            {
                overTerrain = false,
                position = Vector3Int.zero,
                direction = SurfaceDirection.Up,
            };
        }

        public void RegenerateDirtyChunks()
        {
            foreach (var chunkCoord in dirtyChunks)
            {
                if (chunks.TryGetValue(chunkCoord, out ChunkData chunkData))
                {
                    chunkData.chunk.UpdateTerrain(this, chunkData.blockIndex, chunkSize);
                }
            }
            dirtyChunks.Clear();
        }

        #endregion
        #region Mouse Interactions
        
        // Mouse methods
        public static readonly UnityEvent<TerrainHoverInfo> TerrainPressedLeft = new();
        public static readonly UnityEvent<TerrainHoverInfo> TerrainPressedRight = new();
        public static readonly UnityEvent<TerrainHoverInfo> TerrainReleasedLeft = new();
        public static readonly UnityEvent<TerrainHoverInfo> TerrainReleasedRight = new();

        // Left click begins being pressed
        public void ReceiveLeftPress(RaycastHit hit)
        {
            Vector3 position = hit.point - hit.normal * 0.2f; //ensure position inside block
            Vector3Int blockPosition = GetBlockPosition(position);
            
            TerrainPressedLeft.Invoke(new TerrainHoverInfo()
            {
                overTerrain = true,
                position = blockPosition,
                direction = GetSurfaceDirection(hit.normal)
            });
        }
        
        // Left click is released
        public void ReleaseLeftPress()
        {
            TerrainReleasedLeft.Invoke(GetHoverInfo());
        }

        // Right click begins being pressed
        public void ReceiveRightPress(RaycastHit hit)
        {
            Vector3 position = hit.point - hit.normal * 0.2f; //ensure position inside block
            Vector3Int blockPosition = GetBlockPosition(position);
            
            TerrainPressedRight.Invoke(new TerrainHoverInfo()
            {
                overTerrain = true,
                position = blockPosition,
                direction = GetSurfaceDirection(hit.normal)
            });
        }

        // Right click is released
        public void ReleaseRightPress()
        {
            TerrainReleasedRight.Invoke(GetHoverInfo());
        }
        
        #endregion
    }
}
