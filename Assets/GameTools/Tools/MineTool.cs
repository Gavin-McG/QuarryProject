using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Mine Tool", menuName = "Scriptable Objects/Tools/Mine Tool")]
    public class MineTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        
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
            Vector3Int startPosition = startInfo.BackPosition;
            
            TerrainPointerInfo endInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            Vector3Int endPosition = endInfo.BackPosition;
            
            int minX = Mathf.Min(startPosition.x, endPosition.x);
            int minY = Mathf.Min(startPosition.y, endPosition.y);
            int minZ = Mathf.Min(startPosition.z, endPosition.z);
            int maxX = Mathf.Max(startPosition.x, endPosition.x);
            int maxY = Mathf.Max(startPosition.y, endPosition.y);
            int maxZ = Mathf.Max(startPosition.z, endPosition.z);
            
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    for (int z = minZ; z <= maxZ; z++)
                        terrainManager.SetBlock(new Vector3Int(x, y, z), null);
        }
    }
}