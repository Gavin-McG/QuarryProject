using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

namespace MachineSystem.Machines.Spawner
{
    public class Spawner : Machine
    {
        [SerializeField, HideInInspector] public SpawnerType spawnerType;
        
        [SerializeField, HideInInspector] public Item item;
        
        [SerializeReference, HideInInspector] public ItemNode node;

        public void Awake()
        {
            node = new ItemNode()
            {
                position = transform.position,
                item = null
            };
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
            node.item = item.Copy;
        }
        
        [ContextMenu("Print Item")]
        public void PrintItem()
        {
            Item item = node.item;
            Debug.Log(item!=null ? item.ToString() : "No Item");
        }
    }
}
