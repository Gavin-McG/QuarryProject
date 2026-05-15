using Terrain.Blocks;
using UnityEngine;

namespace MachineSystem.Machines
{
    /// <summary>
    /// Abstract class representing a type of machine. Responsible for creating machine instances
    /// </summary>
    public abstract class MachineType : ScriptableObject
    {
        protected static readonly Vector3 PositionOffset = Vector3.one*0.5f;

        [SerializeField] public BlockType block;
        
        public abstract Machine CreateMachine(MachineManager manager, Vector3Int position);
    }
}

