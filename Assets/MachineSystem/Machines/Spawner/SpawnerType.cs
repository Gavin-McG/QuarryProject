using ItemSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Spawner
{
    [CreateAssetMenu(fileName = "Conveyer", menuName = "Scriptable Objects/Machines/Spanwer")]
    public class SpawnerType : MachineType
    {
        [SerializeField] private Spawner prefab;
        [SerializeField] public ItemQuantity itemQuantity;
        
        public override Machine CreateMachine(MachineManager manager, Vector3Int position, Rotation rotation = Rotation.Degrees0)
        {
            var machine = Instantiate(prefab, position + PositionOffset, Quaternion.identity, manager.transform);
            machine.position = position;
            machine.spawnerType = this;
            return machine;
        }
    }
}
