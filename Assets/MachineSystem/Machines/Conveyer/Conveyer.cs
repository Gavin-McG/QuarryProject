using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

namespace MachineSystem.Machines.Conveyer
{
    public class Conveyer : Machine
    {
        [SerializeField, HideInInspector] public ConveyerType conveyerType;
        
        [SerializeField, HideInInspector] public Direction inputDirection;
        [SerializeField, HideInInspector] public Direction outputDirection;
        
        [SerializeReference, HideInInspector] public ItemNode node;

        public void Awake()
        {
            node = new ItemNode()
            {
                position = transform.position,
            };
        }

        public override MachineType GetMachineType()
        {
            return conveyerType;
        }

        public override IEnumerable<Direction> GetInputDirections()
        {
            yield return inputDirection;
        }

        public override IEnumerable<Direction> GetOutputDirections()
        {
            yield return outputDirection;
        }

        public override void ConnectInput(Direction direction, Machine machine)
        {
            if (direction == inputDirection)
            {
                if (node.inputNode == null)
                {
                    node.inputNode = machine.GetOutputNode(DirectionUtility.Opposite(direction));
                }
                else
                {
                    Debug.LogError("Conveyer " + position + ": Attempting to connect already connected side");
                }
            }
        }

        public override void DisconnectInput(Direction direction, Machine machine)
        {
            if (direction == inputDirection)
            {
                if (node.inputNode == null)
                {
                    Debug.LogError("Conveyer " + position + ": Attempting to disconnect non-connected side");
                }
                else
                {
                    node.inputNode = null;
                }
            }
        }


        public override ItemNode GetOutputNode(Direction direction)
        {
            return direction == outputDirection ? node : null;
        }

        public override void Evaluate()
        {
            node.Evaluate();
        }

        public override void Operate()
        {
            node.Operate();
        }

        [ContextMenu("Print Item")]
        public void PrintItem()
        {
            Item item = node.item;
            Debug.Log(item!=null ? item.ToString() : "No Item");
        }
    }
}
