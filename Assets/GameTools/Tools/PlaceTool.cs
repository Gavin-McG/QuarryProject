using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;

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
            
            TerrainManager.onPointerClick.AddListener(TerrainClick);
        }

        public override void Deselect()
        {
            base.Deselect();
            TerrainManager.onPointerClick.RemoveListener(TerrainClick);
        }

        private void TerrainClick(PointerEventData eventData)
        {
            TerrainPointerInfo startInfo = TerrainManager.GetRaycastInfo(eventData.pointerPressRaycast);
            Vector3Int startPosition = startInfo.FrontPosition;
            
            TerrainPointerInfo endInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            Vector3Int endPosition = endInfo.FrontPosition;
            
            int minX = Mathf.Min(startPosition.x, endPosition.x);
            int minY = Mathf.Min(startPosition.y, endPosition.y);
            int minZ = Mathf.Min(startPosition.z, endPosition.z);
            int maxX = Mathf.Max(startPosition.x, endPosition.x);
            int maxY = Mathf.Max(startPosition.y, endPosition.y);
            int maxZ = Mathf.Max(startPosition.z, endPosition.z);
            
            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            for (int z = minZ; z <= maxZ; z++)
                terrainManager.SetBlock(new Vector3Int(x, y, z), block);
        }
    }
}