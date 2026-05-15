using System;
using UnityEngine;

namespace ItemSystem
{
    [Serializable]
    public struct ItemQuantity
    {
        [SerializeField] public ItemType type;
        [SerializeField] public int quantity;
        
        public override string ToString()
        {
            return "Item {Type:" + type?.name + ", Quantity:" + quantity.ToString() + '}';
        }
    }
}
