using System.Collections.Generic;
using System.Linq;
using Terrain;
using Terrain.Blocks;
using UnityEngine;

namespace MachineSystem
{
    /// <summary>
    /// Abstract class representing a type of machine. Responsible for creating machine instances
    /// </summary>
    public abstract class MachineType : ScriptableObject
    {
        protected static readonly Vector3 PositionOffset = Vector3.one*0.5f;
        
        /// Get the blocks that make up a machine
        public abstract IEnumerable<LayoutBlock> GetLayout();
        
        /// Get the blocks that make up a machine, with a rotation and offset applied
        public IEnumerable<LayoutBlock> GetTransformedLayout(Vector3Int position, Rotation rotation) => GetLayout()
            .Select(layoutBlock =>
            {
                Vector3Int newPosition = position + RotationUtility.Rotate(layoutBlock.position, rotation);
                Rotation newRotation = RotationUtility.Rotate(layoutBlock.rotation, rotation);
                return new LayoutBlock()
                {
                    blockType = layoutBlock.blockType,
                    position = newPosition,
                    rotation = newRotation,
                };
            });
        
        /// Create a new instance of this machine type
        public abstract Machine CreateMachine(MachineManager manager, Vector3Int position, Rotation rotation = Rotation.Degrees0);
    }
}

