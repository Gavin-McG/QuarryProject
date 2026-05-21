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
        [SerializeField] private PointerEventData.InputButton placeButton = PointerEventData.InputButton.Left;
        
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
            if (eventData.button != placeButton) return;
            
            TerrainPointerInfo terrainInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            machineManager.PlaceMachine(machineType, terrainInfo.FrontPosition, Rotation.Degrees0);
        }
    }
}