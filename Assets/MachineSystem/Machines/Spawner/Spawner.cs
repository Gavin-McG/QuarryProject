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

        public override IEnumerable<MachineFace> GetInputFaces()
        {
            yield break;
        }

        public override IEnumerable<MachineFace> GetOutputFaces()
        {
            yield return new MachineFace
            {
                position = position,
                direction = Direction.Right
            };
            yield return new MachineFace
            {
                position = position,
                direction = Direction.Forward
            };
            yield return new MachineFace
            {
                position = position,
                direction = Direction.Left
            };
            yield return new MachineFace
            {
                position = position,
                direction = Direction.Back
            };
        }

        public override void ConnectInput(Machine machine, MachineFace face) {}

        public override void DisconnectInput(MachineFace face) {}


        public override ItemNode GetOutputNode(MachineFace face)
        {
            return (face.direction != Direction.Up && face.direction != Direction.Down) ? node : null;
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
            newInstance.SetItem(spawnerType.itemQuantity);
            newInstance.SetPosition(node.position);
            
            node.itemInstance = newInstance;
        }
        
        [ContextMenu("Print Item")]
        public void PrintItem()
        {
            ItemInstance itemInstance = node.itemInstance;
            Debug.Log(itemInstance!=null ? itemInstance.GetItem().ToString() : "No Item");
        }

        public void OnDrawGizmos()
        {
            node.OnDrawGizmos();
        }
    }
}
