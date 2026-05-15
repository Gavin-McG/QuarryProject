using System;
using System.Collections.Generic;
using ItemSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Spawner
{
    public class Spawner : Machine
    {
        [SerializeField] private ItemInstance itemPrefab;
        [SerializeField, HideInInspector] public SpawnerType spawnerType;
        [SerializeReference, HideInInspector] public ItemNode node;

        public void Awake()
        {
            node = new ItemNode()
            {
                position = transform.position + Vector3.up*0.5f,
                itemInstance = null
            };
        }
        
        public void Update()
        {
            node.Update();
        }

        public void OnDestroy()
        {
            node.OnDestroy();
        }

        public override MachineType GetMachineType()
        {
            return spawnerType;
        }

        public override IEnumerable<Direction> GetInputDirections()
        {
            return new List<Direction>();
        }

        public override IEnumerable<Direction> GetOutputDirections()
        {
            return new List<Direction>() { Direction.Left ,Direction.Right, Direction.Forward, Direction.Back };
        }

        public override void ConnectInput(Direction direction, Machine machine) {}

        public override void DisconnectInput(Direction direction, Machine machine) {}


        public override ItemNode GetOutputNode(Direction direction)
        {
            return (direction != Direction.Up && direction != Direction.Down) ? node : null;
        }

        public override void Evaluate()
        {
            node.Evaluate();
        }

        public override void Operate()
        {
            node.Operate();

            // Spawn new item
            if (node.itemInstance != null) return;
            ItemInstance newInstance = Instantiate(itemPrefab);
            newInstance.item = spawnerType.itemQuantity;
            newInstance.SetPosition(node.position);
            
            node.itemInstance = newInstance;
        }
        
        [ContextMenu("Print Item")]
        public void PrintItem()
        {
            ItemInstance itemInstance = node.itemInstance;
            Debug.Log(itemInstance!=null ? itemInstance.item.ToString() : "No Item");
        }

        public void OnDrawGizmos()
        {
            node.OnDrawGizmos();
        }
    }
}
