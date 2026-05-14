using MachineSystem;
using MachineSystem.Machines;
using Terrain;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Machine Tool")]
    public class MachineTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private MachineType machineType;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;

        public override void Select()
        {
            machineManager = GameObject.Find("MachineManager")?.GetComponent<MachineManager>();
        }
        
        // Left click - Place single block
        public override void PressLeft(TerrainHoverInfo info)
        {
            Debug.Log("Attempting to place " + machineType.name);
            machineManager?.PlaceMachine(info.FrontPosition, machineType);
        }
    }
}