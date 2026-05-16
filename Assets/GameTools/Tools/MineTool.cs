using Terrain;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Mine Tool")]
    public class MineTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        
        public override Sprite Sprite => toolSprite;
        
        private TerrainManager terrainManager;
        private Vector3Int dragStartPosition;

        public override void Select()
        {
            base.Select();
            terrainManager = GameObject.Find("TerrainManager")?.GetComponent<TerrainManager>();
        }
        
        // Left click - Destroy single block

        public override void PressLeft(TerrainHoverInfo info)
        {
            base.PressLeft(info);
            terrainManager?.SetBlock(info.BackPosition, null);
        }

        // Right click - Drag destroy area
        
        public override void PressRight(TerrainHoverInfo info)
        {
            base.PressRight(info);
            dragStartPosition = info.BackPosition;
        }

        public override void ReleaseRight(TerrainHoverInfo info)
        {
            base.ReleaseRight(info);
            if (!info.overTerrain) return;
            
            Vector3Int dragEndPosition = info.BackPosition;
            
            int minX = Mathf.Min(dragStartPosition.x, dragEndPosition.x);
            int minY = Mathf.Min(dragStartPosition.y, dragEndPosition.y);
            int minZ = Mathf.Min(dragStartPosition.z, dragEndPosition.z);
            int maxX = Mathf.Max(dragStartPosition.x, dragEndPosition.x);
            int maxY = Mathf.Max(dragStartPosition.y, dragEndPosition.y);
            int maxZ = Mathf.Max(dragStartPosition.z, dragEndPosition.z);
            
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    for (int z = minZ; z <= maxZ; z++)
                        terrainManager.SetBlock(new Vector3Int(x, y, z), null, false);
            
            terrainManager.RegenerateDirtyChunks();
        }
    }
}