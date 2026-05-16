using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Place Tool", menuName = "Scriptable Objects/Tools/Place Tool")]
    public class PlaceTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private BlockType block;
        
        public override Sprite Sprite => toolSprite;
        
        private TerrainManager terrainManager;

        public override void Select()
        {
            base.Select();
            terrainManager = Managers.GetManager<TerrainManager>();
        }
        
        // Left Drag - Drag destroy area
        
        public override void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            base.TerrainRightButtonDragged(startInfo, endInfo);
            
            Vector3Int dragStartPosition = startInfo.FrontPosition;
            Vector3Int dragEndPosition = endInfo.FrontPosition;
            
            int minX = Mathf.Min(dragStartPosition.x, dragEndPosition.x);
            int minY = Mathf.Min(dragStartPosition.y, dragEndPosition.y);
            int minZ = Mathf.Min(dragStartPosition.z, dragEndPosition.z);
            int maxX = Mathf.Max(dragStartPosition.x, dragEndPosition.x);
            int maxY = Mathf.Max(dragStartPosition.y, dragEndPosition.y);
            int maxZ = Mathf.Max(dragStartPosition.z, dragEndPosition.z);
            
            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            for (int z = minZ; z <= maxZ; z++)
                terrainManager.SetBlock(new Vector3Int(x, y, z), block, false);
            
            terrainManager.RegenerateDirtyChunks();
        }
    }
}