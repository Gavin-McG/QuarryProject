using Terrain;
using UnityEngine;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Mine Tool")]
    public class MineTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        
        public override Sprite Sprite => toolSprite;
        
        private TerrainManager terrainManager;

        public override void Select()
        {
            base.Select();
            terrainManager = GameObject.Find("TerrainManager")?.GetComponent<TerrainManager>();
        }
        
        // Left Drag - Drag destroy area
        
        public override void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            base.TerrainRightButtonDragged(startInfo, endInfo);
            
            Vector3Int dragStartPosition = startInfo.BackPosition;
            Vector3Int dragEndPosition = endInfo.BackPosition;
            
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