using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Change Tool", menuName = "Scriptable Objects/Tools/Change Tool")]
    public class ChangeTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private BlockType block;
        [SerializeField] private PointerEventData.InputButton changeButton = PointerEventData.InputButton.Left;
        
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
            if (eventData.button != changeButton) return;
            
            TerrainPointerInfo terrainInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            terrainManager.SetBlock(block, terrainInfo.BackPosition);
        }
    }
}