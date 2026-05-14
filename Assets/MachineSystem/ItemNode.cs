using System;
using UnityEngine;
using ItemSystem;

namespace MachineSystem
{
    [Serializable]
    public class ItemNode
    {
        //node info
        [SerializeReference] public ItemNode inputNode = null;
        [SerializeField] public Vector3 position;
        [NonSerialized] public Item item = null;

        //item animation info
        [SerializeField] private Vector3 takePosition = Vector3.zero;
        [SerializeField] private float takeTime = 0;
        
        //step info
        [SerializeField] private ItemNode reservedNode = null;
        [SerializeField] private bool reserved = false;

        public void Evaluate()
        {
            if (item == null && (inputNode?.Reserve() ?? false))
            {
                reservedNode = inputNode;
            }
        }

        public void Operate()
        {
            if (reservedNode != null)
            {
                item = reservedNode.TakeItem();
                takePosition = reservedNode.position;
                takeTime = Time.time;
            }
            
            Reset();
        }

        /// <summary>
        /// Transfers ownership of this node's item
        /// </summary>
        /// <returns>The item belonging to the node</returns>
        private Item TakeItem()
        {
            Item temp = item;
            item = null;
            return temp;
        }

        /// <summary>
        /// Attempts to reserve the node to transfer its item in the Evaluate step
        /// </summary>
        /// <returns>true if the node was reserved</returns>
        private bool Reserve()
        {
            // only reserve if not reserved and has item
            if (reserved || item==null) return false;
            
            reserved = true;
            return true;
        }

        /// <summary>
        /// Reset values after the step
        /// </summary>
        private void Reset()
        {
            reserved = false;
            reservedNode = null;
        }
    }
}
