using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ItemSystem
{
    public class ItemInstance : MonoBehaviour, 
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerClick = new();
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerDown = new();
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerUp = new();
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerEnter = new();
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerExit = new();
        public static readonly UnityEvent<ItemInstance, PointerEventData> onPointerMove = new();
        
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private ItemQuantity item;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetItem(ItemQuantity item)
        {
            this.item = item;
            if (item.type.mesh != null)
                meshFilter.sharedMesh = item.type.mesh;
            if (item.type.material != null)
                meshRenderer.sharedMaterial = item.type.material;
        }
        
        public ItemQuantity GetItem() => item;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick.Invoke(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke(this, eventData);
        }
        
        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMove.Invoke(this, eventData);
        }
    }
}
