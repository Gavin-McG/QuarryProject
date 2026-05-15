using System;
using UnityEngine;

namespace ItemSystem
{
    public class ItemInstance : MonoBehaviour
    {
        [SerializeField] public ItemQuantity item;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
