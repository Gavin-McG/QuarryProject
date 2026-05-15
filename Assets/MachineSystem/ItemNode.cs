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
        [SerializeField] public float transitionTime = 1f;
        [NonSerialized] public ItemInstance itemInstance = null;

        //item animation info
        [SerializeField] private Vector3 takePosition = Vector3.zero;
        [SerializeField] private float takeTime = 0;
        
        //step info
        [SerializeField] private ItemNode reservedNode = null;
        [SerializeField] private bool reserved = false;

        public void Update()
        {
            if (itemInstance == null) return;
            
            // Set new position of the item
            float t = Mathf.Clamp01((Time.time - takeTime) / transitionTime);
            Vector3 itemPosition = Vector3.Lerp(takePosition, position, t);
            itemInstance.SetPosition(itemPosition);
        }

        public void OnDestroy()
        {
            if (itemInstance == null) return;
            
            UnityEngine.Object.Destroy(itemInstance.gameObject);
        }

        public void Evaluate()
        {
            if (itemInstance == null && (inputNode?.Reserve() ?? false))
            {
                reservedNode = inputNode;
            }
        }

        public void Operate()
        {
            if (reservedNode != null)
            {
                itemInstance = reservedNode.TakeItem();
                takePosition = reservedNode.position;
                takeTime = Time.time;
            }
            
            Reset();
        }

        /// <summary>
        /// Transfers ownership of this node's item
        /// </summary>
        /// <returns>The item belonging to the node</returns>
        private ItemInstance TakeItem()
        {
            ItemInstance temp = itemInstance;
            itemInstance = null;
            return temp;
        }

        /// <summary>
        /// Attempts to reserve the node to transfer its item in the Evaluate step
        /// </summary>
        /// <returns>true if the node was reserved</returns>
        private bool Reserve()
        {
            // only reserve if not reserved and has item
            if (reserved || itemInstance==null) return false;
            
            // Only reserve if item is fully transitioned
            if (Time.time - takeTime < transitionTime) return false;
            
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

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(position, Vector3.one * 0.1f);
            if (inputNode != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, inputNode.position);
            }
        }
    }
}
