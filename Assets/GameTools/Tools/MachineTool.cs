using MachineSystem;
using MachineSystem.Machines;
using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;


namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Machine Tool", menuName = "Scriptable Objects/Tools/Machine Tool")]
    public class MachineTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private MachineType machineType;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;

        public override void Select()
        {
            base.Select();
            machineManager = machineManager = Managers.GetManager<MachineManager>();
            
            TerrainManager.onPointerClick.AddListener(TerrainClick);
        }

        public override void Deselect()
        {
            base.Deselect();
            
            TerrainManager.onPointerClick.RemoveListener(TerrainClick);
        }

        private void TerrainClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                TerrainPointerInfo terrainInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
                machineManager.PlaceMachine(terrainInfo.FrontPosition, machineType);
            }
        }
    }
}