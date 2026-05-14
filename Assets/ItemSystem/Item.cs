using System;
using UnityEngine;

namespace ItemSystem
{
    [Serializable]
    public class Item
    {
        [SerializeField] public ItemType type;
        [SerializeField] public int quantity;
        
        public Item Copy => MemberwiseClone() as Item;

        public override string ToString()
        {
            return "Item {Type:" + type?.name + ", Quantity:" + quantity.ToString() + '}';
        }
    }
}
