using MachineSystem;
using MachineSystem.Machines;
using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.UIElements;

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
        }
        
        // Left click - Place single block
        public override void TerrainLeftButtonPressed(TerrainPointerInfo info)
        {
            base.TerrainLeftButtonPressed(info);
            machineManager?.PlaceMachine(info.FrontPosition, machineType);
        }
    }
}