using System.Collections.Generic;
using ItemSystem;
using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Conveyer
{
    public class Conveyer : Machine
    {
        [SerializeField, HideInInspector] public ConveyerType conveyerType;
        [SerializeField] public Direction inputDirection;
        [SerializeField] public Direction outputDirection;
        [SerializeReference, HideInInspector] public ItemNode node;

        public void Awake()
        {
            node = new ItemNode()
            {
                position = transform.position + Vector3.up*0.2f,
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
            return conveyerType;
        }

        public override IEnumerable<MachineFace> GetInputFaces()
        {
            yield return new MachineFace
            {
                position = position,
                direction = DirectionUtility.Opposite(inputDirection)
            };
        }

        public override IEnumerable<MachineFace> GetOutputFaces()
        {
            yield return new MachineFace
            {
                position = position,
                direction = outputDirection
            };
        }

        public override void ConnectInput(Machine machine, MachineFace face)
        {
            if (face.direction == DirectionUtility.Opposite(inputDirection))
            {
                node.inputNode = machine.GetOutputNode(face.GetOpposite());
            }
        }

        public override void DisconnectInput(MachineFace face)
        {
            if (face.direction == DirectionUtility.Opposite(inputDirection))
            {
                node.inputNode = null;
            }
        }


        public override ItemNode GetOutputNode(MachineFace face)
        {
            return face.direction == outputDirection ? node : null;
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
            ItemInstance itemInstance = node.itemInstance;
            Debug.Log(itemInstance != null ? itemInstance.GetItem().ToString() : "No Item");
        }

        public void OnDrawGizmos()
        {
            node.OnDrawGizmos();
        }
    }
}
