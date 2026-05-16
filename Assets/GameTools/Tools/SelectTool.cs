using ItemSystem;
using MachineSystem;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Select Tool")]
    public class SelectTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;
        private TerrainManager terrainManager;

        public override void Select()
        {
            base.Select();
            machineManager = Managers.GetManager<MachineManager>();
            terrainManager = Managers.GetManager<TerrainManager>();
        }

        public override void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            base.TerrainLeftButtonDragged(startInfo, endInfo);

            // Ignore drags unless on same block
            if (startInfo.BackPosition != endInfo.BackPosition) return;

            // Check if block clicked is a machine
            var machine = machineManager.GetMachine(startInfo.FrontPosition);
            if (machine != null)
            {
                Debug.Log("Clicked Machine: " + machine.name);
                return;
            }

            // Click normal block
            var blockType = terrainManager.GetBlock(startInfo.position);
            if (blockType != null)
            {
                Debug.Log("Clicked Block: " + blockType.blockName);
                return;
            }
        }

        public override void ItemLeftButtonClicked(ItemInstance item)
        {
            base.ItemLeftButtonClicked(item);
            
            Debug.Log("Clicked Item: " + item.item);
        }
    }
}