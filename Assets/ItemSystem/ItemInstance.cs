using System;
using ClickManager;
using UnityEngine;
using UnityEngine.Events;

namespace ItemSystem
{
    public class ItemInstance : MonoBehaviour, IReceiveClickCast
    {
        public readonly static UnityEvent<ItemInstance> ItemLeftButtonClicked = new();
        public readonly static UnityEvent<ItemInstance> ItemRightButtonClicked = new();
        
        [SerializeField] public ItemQuantity item;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void LeftButtonClicked(RaycastHit hit)
        {
            ItemLeftButtonClicked.Invoke(this);
        }

        public void RightButtonClicked(RaycastHit hit)
        {
            ItemRightButtonClicked.Invoke(this);
        }
    }
}
