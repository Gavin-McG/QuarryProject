using System.Collections.Generic;
using ItemSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Spawner
{
    [CreateAssetMenu(fileName = "Conveyer", menuName = "Scriptable Objects/Machines/Spanwer")]
    public class SpawnerType : MachineType
    {
        [SerializeField] public MultiBlockLayout layout;
        [SerializeField] private Spawner prefab;
        [SerializeField] public ItemQuantity itemQuantity;
        
        public override IEnumerable<LayoutBlock> GetLayout()
        {
            return layout.GetLayout();
        }
        
        public override Machine CreateMachine(MachineManager manager, Vector3Int position, Rotation rotation = Rotation.Degrees0)
        {
            var machine = Instantiate(prefab, position + PositionOffset, Quaternion.identity, manager.transform);
            machine.spawnerType = this;
            return machine;
        }
    }
}
