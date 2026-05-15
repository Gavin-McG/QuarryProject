using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Conveyer
{
    [CreateAssetMenu(fileName = "Conveyer", menuName = "Scriptable Objects/Machines/Conveyer")]
    public class ConveyerType : MachineType
    {
        [SerializeField] private Conveyer prefab;
        [SerializeField] private Direction inputDirection;
        [SerializeField] private Direction outputDirection;
        
        public override Machine CreateMachine(MachineManager manager, Vector3Int position)
        {
            var machine = Instantiate(prefab, position + PositionOffset, Quaternion.identity, manager.transform);
            machine.position = position;
            machine.conveyerType = this;
            machine.inputDirection = inputDirection;
            machine.outputDirection = outputDirection;
            return machine;
        }
    }
}
