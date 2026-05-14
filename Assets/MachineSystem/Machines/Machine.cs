using System;
using System.Collections.Generic;
using ClickManager;
using UnityEngine;

namespace MachineSystem.Machines
{
    /// <summary>
    /// Abstract class representing an instance of a machine
    /// </summary>
    [Serializable]
    public abstract class Machine : MonoBehaviour, IReceiveClickCast
    {
        [HideInInspector] public Vector3Int position;
        
        public abstract MachineType GetMachineType();
        
        public bool ReceiveLeftPress(RaycastHit hit)
        {
            Debug.Log("Clicked " + gameObject.name);
            return true;
        }

        public bool ReceiveRightPress(RaycastHit hit)
        {
            Debug.Log("Clicked " + gameObject.name);
            return true;
        }
        
        /// Get the directions that the machine will accept input
        public abstract IEnumerable<Direction> GetInputDirections();
        /// Get the directions that the machine will provide output
        public abstract IEnumerable<Direction> GetOutputDirections();
        
        
        /// Connect an Input to this machine
        public virtual void ConnectInput(Direction direction, Machine machine) {}
        /// Disconnect an input to this machine
        public virtual void DisconnectInput(Direction direction, Machine machine) {}

        
        /// Get the ItemNode which the machine will use to output in a specific direction
        public virtual ItemNode GetOutputNode(Direction direction) { return null; }

        
        /// Evaluate the item transfers for the machine's nodes in this step
        public abstract void Evaluate();
        /// Perform the item transfers based on the evalate section of the step
        public abstract void Operate();
    }
}
