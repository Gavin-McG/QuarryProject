using System;
using System.Collections.Generic;
using Terrain;
using UnityEngine;

namespace MachineSystem
{
    /// <summary>
    /// Abstract class representing an instance of a machine
    /// </summary>
    [Serializable]
    public abstract class Machine : MonoBehaviour
    {
        [HideInInspector] public int machineIndex;
        [HideInInspector] public Vector3Int position;
        [HideInInspector] public Rotation rotation;
        
        public abstract MachineType GetMachineType();
        
        
        /// Get the directions that the machine will accept input
        public abstract IEnumerable<MachineFace> GetInputFaces();
        /// Get the directions that the machine will provide output
        public abstract IEnumerable<MachineFace> GetOutputFaces();
        
        
        /// Connect an Input to this machine
        public virtual void ConnectInput(Machine machine, MachineFace face) {}
        /// Disconnect an input to this machine
        public virtual void DisconnectInput(MachineFace face) {}

        
        /// Get the ItemNode which the machine will use to output in a specific direction
        public virtual ItemNode GetOutputNode(MachineFace face) { return null; }

        
        /// Evaluate the item transfers for the machine's nodes in this step
        public abstract void Evaluate();
        /// Perform the item transfers based on the evalate section of the step
        public abstract void Operate();
    }
}
