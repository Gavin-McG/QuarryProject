using System;
using ClickManager;
using UnityEngine;

namespace ItemSystem
{
    public class ItemInstance : MonoBehaviour, IReceiveClickCast
    {
        [SerializeField] public ItemQuantity item;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void LeftButtonClicked(RaycastHit hit)
        {
            Debug.Log("Clicked: " + item);
        }
    }
}
