using System;
using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Generators;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ChunkCoord = UnityEngine.Vector3Int;
using ChunkPosition = UnityEngine.Vector3Int;

namespace Terrain
{
    public enum SurfaceDirection { Up, Down, Left, Right, Forward, Back }

    public struct TerrainPointerInfo
    {
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

    public class TerrainManager : MonoBehaviour, 
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private TerrainChunk chunkPrefab;
        [SerializeField] private TerrainGenerator generator;
        [SerializeField] private Vector3Int chunkSize = new Vector3Int(16, 16, 16);
        [SerializeField] private LayerMask terrainMask;

        [SerializeField] private ChunkCoord initialChunkMin = new ChunkCoord(-2, -4, -2);
        [SerializeField] private ChunkCoord initialChunkMax = new ChunkCoord(2, 0, 2);

        public readonly UnityEvent<Vector3Int> BlockRemoved = new();
        public readonly UnityEvent<Vector3Int> BlockAdded = new();

        class ChunkData
        {
            public TerrainChunk chunk;
            public NativeArray<BlockInfo> blockData;

            ~ChunkData()
            {
                blockData.Dispose();
            }
        }

        private readonly Dictionary<ChunkCoord, ChunkData> chunks = new();
        private readonly HashSet<ChunkCoord> dirtyChunks = new();

        private void Start()
        {
            BlockData.InitializeBlockData();
            LoadInitialChunks();
        }

        private void LateUpdate()
        {
            RegenerateDirtyChunks();
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
                NativeArray<BlockInfo> chunkData = generator.GenerateTerrain(position, chunkSize);

                // Update chunk
                pair.Value.chunk.UpdateTerrain(this, chunkData, chunkSize);
                pair.Value.blockData.Dispose();
                pair.Value.blockData = chunkData;
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
            NativeArray<BlockInfo> chunkData = generator.GenerateTerrain(position, chunkSize);
            newChunk.UpdateTerrain(this, chunkData, chunkSize);

            // Add chunk
            chunks.Add(chunkCoord, new ChunkData()
            {
                chunk = newChunk,
                blockData = chunkData,
            });
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
        private static Vector3Int GetBlockPosition(Vector3 position)
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
            return BlockData.GetBlock(chunkData.blockData[index].blockIndex);
        }

        /// <summary>
        /// Sets the block at a specific block position
        /// </summary>
        public void SetBlock(Vector3Int position, BlockType block)
        {
            SetBlock(position, Rotation.Degrees0, block);
        }

        /// <summary>
        /// Sets the block at a specific block position
        /// </summary>
        public void SetBlock(Vector3Int position, Rotation rotation, BlockType block)
        {
            // Get the correct chunk
            ChunkCoord chunkCoord = GetChunkCoord(position);
            if (!chunks.TryGetValue(chunkCoord, out ChunkData chunkData))
            {
                Debug.LogWarning("TerrainManager: Attempting to write to a block in an unloaded chunk " + chunkCoord);
                return;
            }
            
            // Calculate block Index within chunk
            ChunkPosition chunkPosition = GetChunkPosition(position);
            int index = chunkPosition.x + (chunkPosition.y * chunkSize.x) + (chunkPosition.z * chunkSize.x * chunkSize.y);
            
            // Change block
            int oldIndex = chunkData.blockData[index].blockIndex;
            int newIndex = block?.Index ?? -1;
            chunkData.blockData[index] = new BlockInfo()
            {
                blockIndex = newIndex,
                rotation = rotation,
            };
            
            // Call change events
            if (oldIndex != -1) BlockRemoved.Invoke(position);
            if (newIndex != -1) BlockAdded.Invoke(position);

            dirtyChunks.Add(chunkCoord);
        }

        private void RegenerateDirtyChunks()
        {
            foreach (var chunkCoord in dirtyChunks)
            {
                if (chunks.TryGetValue(chunkCoord, out ChunkData chunkData))
                {
                    chunkData.chunk.UpdateTerrain(this, chunkData.blockData, chunkSize);
                }
            }
            dirtyChunks.Clear();
        }

        #endregion
        #region EventSystem Methods
        
        public static readonly UnityEvent<PointerEventData> onPointerClick = new UnityEvent<PointerEventData>();
        public static readonly UnityEvent<PointerEventData> onPointerDown = new UnityEvent<PointerEventData>();
        public static readonly UnityEvent<PointerEventData> onPointerUp = new UnityEvent<PointerEventData>();
        public static readonly UnityEvent<PointerEventData> onPointerEnter = new UnityEvent<PointerEventData>();
        public static readonly UnityEvent<PointerEventData> onPointerExit = new UnityEvent<PointerEventData>();
        public static readonly UnityEvent<PointerEventData> onPointerMove = new UnityEvent<PointerEventData>();
        
        public static TerrainPointerInfo GetRaycastInfo(RaycastResult hit)
        {
            Vector3 position = hit.worldPosition - hit.worldNormal * 0.02f; //ensure position inside block
            Vector3Int blockPosition = GetBlockPosition(position);
            SurfaceDirection direction = GetSurfaceDirection(hit.worldNormal);
            return new TerrainPointerInfo()
            {
                position = blockPosition,
                direction = direction,
            };
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke(eventData);
        }
        
        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMove.Invoke(eventData);
        }
        
        #endregion
    }
}
