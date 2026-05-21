using System;
using System.Collections.Generic;
using System.Linq;
using MachineSystem.Machines;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem
{
    public struct MachineCell
    {
        public Machine machine;
        public Vector3Int localPosition;
    }

    public struct MachineFace
    {
        public Vector3Int position;
        public Direction direction;

        public MachineFace ApplyTransform(Vector3Int offset, Rotation rotation)
        {
            return new MachineFace()
            {
                position = RotationUtility.Rotate(position, rotation) + offset,
                direction = RotationUtility.Rotate(direction, rotation)
            };
        }

        public Vector3Int Neighbor => DirectionUtility.MovePosition(position, direction);

        public MachineFace GetOpposite()
        {
            return new MachineFace()
            {
                position = Neighbor,
                direction = DirectionUtility.Opposite(direction)
            };
        } 
    }
    
    public class MachineManager : MonoBehaviour
    {
        [SerializeField] private Vector3Int rangeMin;
        [SerializeField] private Vector3Int rangeMax;
        
        private MachineCell[,,] machineGrid;
        private readonly List<Machine> machines = new();
        
        private TerrainManager terrainManager;

        private void Start()
        {
            var rangeSize = rangeMax - rangeMin;
            machineGrid = new MachineCell[rangeSize.x, rangeSize.y, rangeSize.z];
            
            if (terrainManager == null)
            {
                terrainManager = Managers.GetManager<TerrainManager>();
                terrainManager.BlockRemoved.AddListener(RemoveMachine);
            }
        }

        private void OnEnable()
        {
            if (terrainManager == null)
            {
                terrainManager = Managers.GetManager<TerrainManager>();
                terrainManager.BlockRemoved.AddListener(RemoveMachine);
            }
        }

        private void OnDisable()
        {
            terrainManager.BlockRemoved.RemoveListener(RemoveMachine);
            terrainManager = null;
        }

        private void Update()
        {
            PerformStep();
        }

        bool InBounds(Vector3Int position)
        {
            return 
                position.x >= rangeMin.x && position.x < rangeMax.x && 
                position.y >= rangeMin.y && position.y < rangeMax.y && 
                position.z >= rangeMin.z && position.z < rangeMax.z;
        }

        bool SpaceOpen(Vector3Int position)
        {
            return InBounds(position) && GetMachine(position) == null;
        }

        public Machine GetMachine(Vector3Int position)
        {
            if (InBounds(position))
            {
                Vector3Int arrayPosition = position - rangeMin;
                return machineGrid[arrayPosition.x, arrayPosition.y, arrayPosition.z].machine;
            }
            
            return null;
        }

        private Machine GetFacingMachine(MachineFace face)
        {
            Vector3Int position = face.Neighbor;
            Vector3Int arrayPosition = position - rangeMin;
            return machineGrid[arrayPosition.x, arrayPosition.y, arrayPosition.z].machine;
        }

        public Machine PlaceMachine(MachineType machineType, Vector3Int position, Rotation rotation)
        {
           // Check that all spaces for the machine are open 
           IEnumerable<LayoutBlock> machineLayout = machineType.GetTransformedLayout(position, rotation);
           foreach (LayoutBlock block in machineLayout)
           {
               if (!SpaceOpen(block.position)) return null;
           }
           
           // Create new machine and place in containers
           Machine newMachine = machineType.CreateMachine(this, position, rotation);
           newMachine.machineIndex = machines.Count;
           newMachine.position = position;
           newMachine.rotation = rotation;
           machines.Add(newMachine);
           foreach (LayoutBlock block in machineLayout)
           {
               terrainManager.SetBlock(block.blockType, block.position, block.rotation);
               Vector3Int arrayIndex = block.position - rangeMin;
               machineGrid[arrayIndex.x, arrayIndex.y, arrayIndex.z] = new MachineCell()
               {
                   machine = newMachine,
                   localPosition = block.position - position,
               };
           }
           
           // Connect Machine Inputs
           var machineInputs = newMachine.GetInputFaces();
           foreach (var input in machineInputs)
           {
               Machine neighborMachine = GetFacingMachine(input);
               if (neighborMachine == null) continue;
               newMachine.ConnectInput(neighborMachine, input);
           }
           
           // Connect Machine Outputs
           var machineOutputs = newMachine.GetOutputFaces();
           foreach (var output in machineOutputs)
           {
               Machine neighborMachine = GetFacingMachine(output);
               if (neighborMachine == null) continue;
               neighborMachine.ConnectInput(newMachine, output.GetOpposite());
           }
           
           return newMachine;
        }

        public void RemoveMachine(Vector3Int position)
        {
            if (!InBounds(position)) return;
            
            // Get machine at position
            Vector3Int arrayPosition = position - rangeMin;
            MachineCell machineCell = machineGrid[arrayPosition.x, arrayPosition.y, arrayPosition.z];
            Machine machine = machineCell.machine;
            if (machine == null) return;
            
            // Disconnect machine Inputs
            var machineInputs = machine.GetInputFaces();
            foreach (var input in machineInputs)
            {
                Machine neighborMachine = GetFacingMachine(input);
                if (neighborMachine == null) continue;
                machine.DisconnectInput(input);
            }
            
            // Disconnect machine Outputs
            var machineOutputs = machine.GetOutputFaces();
            foreach (var output in machineOutputs)
            {
                Machine neighborMachine = GetFacingMachine(output);
                if (neighborMachine == null) continue;
                neighborMachine.DisconnectInput(output.GetOpposite());
            }
            
            // Remove Machine blocks
            IEnumerable<LayoutBlock> machineLayout = machine
                .GetMachineType()
                .GetTransformedLayout(machine.position, machine.rotation);
            foreach (var block in machineLayout)
            {
                Vector3Int arrayIndex = block.position - rangeMin;
                machineGrid[arrayIndex.x, arrayIndex.y, arrayIndex.z].machine = null;
                terrainManager.SetBlock(null, block.position);
            }
            
            // Remove Machine from machine List (swap last machine with removed machine)
            Machine lastMachine = machines.Last();
            lastMachine.machineIndex = machine.machineIndex;
            machines[machine.machineIndex] = lastMachine;
            machines.RemoveAt(machines.Count - 1);
            
            Destroy(machine.gameObject);
        }

        private void PerformStep()
        {
            foreach (var machine in machines)
            {
                machine.Evaluate();
            }

            foreach (var machine in machines)
            {
                machine.Operate();
            }
        }
    }
}