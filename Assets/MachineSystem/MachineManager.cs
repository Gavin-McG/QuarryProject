using System;
using System.Collections.Generic;
using MachineSystem.Machines;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem
{
    public class MachineManager : MonoBehaviour
    {
        [SerializeField] private Vector3Int rangeMin;
        [SerializeField] private Vector3Int rangeMax;
        
        private Machine[,,] machines;
        
        private readonly HashSet<Machine> machineSet = new();
        private TerrainManager terrainManager;

        private void Start()
        {
            var rangeSize = rangeMax - rangeMin;
            machines = new Machine[rangeSize.x, rangeSize.y, rangeSize.z];
            terrainManager = Managers.GetManager<TerrainManager>();
            
            terrainManager.BlockRemoved.AddListener(RemoveMachine);
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
                return machines[arrayPosition.x, arrayPosition.y, arrayPosition.z];
            }
            
            return null;
        }

        private bool SetMachine(Vector3Int position, Machine machine)
        {
            if (!InBounds(position)) return false;
            
            Vector3Int arrayPosition = position - rangeMin;
            machines[arrayPosition.x, arrayPosition.y, arrayPosition.z] = machine;
            return true;
        }

        public MachineType GetMachineType(Vector3Int position)
        {
            Machine machine = GetMachine(position);
            return machine?.GetMachineType();
        }

        public Machine PlaceMachine(Vector3Int position, MachineType machineType)
        {
            if (!SpaceOpen(position)) return null;
            
            // Create new Machine
            terrainManager.SetBlock(position, machineType.block);
            Machine newMachine = machineType.CreateMachine(this, position);
            SetMachine(position, newMachine);
            machineSet.Add(newMachine);
            
            // Connect new machine's inputs
            IEnumerable<Direction> inputDirections = newMachine.GetInputDirections();
            foreach (Direction direction in inputDirections)
            {
                // Get neighboring machine
                Vector3Int inputPosition = DirectionUtility.MovePosition(position, direction);
                Machine inputMachine = GetMachine(inputPosition);
                
                // Update connection
                if (inputMachine == null) continue;
                newMachine?.ConnectInput(direction, inputMachine);
            }
            
            // Connect new machine's outputs
            IEnumerable<Direction> outputs = newMachine.GetOutputDirections();
            foreach (Direction direction in outputs)
            {
                // Get neighboring machine
                Vector3Int outputPosition = DirectionUtility.MovePosition(position, direction);
                Machine outputMachine = GetMachine(outputPosition);
                
                // Update connection
                if (outputMachine == null) continue;
                outputMachine?.ConnectInput(DirectionUtility.Opposite(direction), newMachine); //direction reversed
            }
            
            return newMachine;
        }

        public void RemoveMachine(Vector3Int position)
        {
            Machine machine = GetMachine(position);
            if (machine == null) return;
            
            // Disconnect machine's inputs
            IEnumerable<Direction> inputDirections = machine.GetInputDirections();
            foreach (Direction direction in inputDirections)
            {
                // Get neighboring machine
                Vector3Int inputPosition = DirectionUtility.MovePosition(position, direction);
                Machine inputMachine = GetMachine(inputPosition);
                
                // Update connection
                if (inputMachine == null) continue;
                machine?.DisconnectInput(direction, inputMachine);
            }
            
            // Disconnect machine's outputs
            IEnumerable<Direction> outputs = machine.GetOutputDirections();
            foreach (Direction direction in outputs)
            {
                // Get neighboring machine
                Vector3Int outputPosition = DirectionUtility.MovePosition(position, direction);
                Machine outputMachine = GetMachine(outputPosition);
                
                // Update connection
                if (outputMachine == null) continue;
                outputMachine?.DisconnectInput(DirectionUtility.Opposite(direction), machine); //direction reversed
            }
            
            // Remove machine
            SetMachine(position, null);
            machineSet.Remove(machine);
            Destroy(machine.gameObject);
        }

        private void PerformStep()
        {
            foreach (var machine in machineSet)
            {
                machine.Evaluate();
            }

            foreach (Machine machine in machineSet)
            {
                machine.Operate();
            }
        }
    }
}