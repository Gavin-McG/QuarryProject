using ItemSystem;
using MachineSystem;
using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Select Tool", menuName = "Scriptable Objects/Tools/Select Tool")]
    public class SelectTool : GameTool
    {
        private enum HoverMode { None, Terrain, Item }
        
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private GameObject outlinePrefab;
        [SerializeField] private PointerEventData.InputButton selectButton = PointerEventData.InputButton.Left;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;
        private TerrainManager terrainManager;

        private GameObject outlineObject;
        
        private HoverMode hoverMode;
        private PointerEventData cachedEventData;

        public override void Select()
        {
            base.Select();
            machineManager = Managers.GetManager<MachineManager>();
            terrainManager = Managers.GetManager<TerrainManager>();
            
            outlineObject = Instantiate(outlinePrefab);
            outlineObject.SetActive(false);
            
            TerrainManager.onPointerClick.AddListener(TerrainClick);
            TerrainManager.onPointerEnter.AddListener(TerrainEnter);
            TerrainManager.onPointerExit.AddListener(TerrainExit);
            ItemInstance.onPointerClick.AddListener(ItemClick);
            ItemInstance.onPointerEnter.AddListener(ItemEnter);
            ItemInstance.onPointerExit.AddListener(ItemExit);
        }

        public override void Deselect()
        {
            base.Deselect();
            Destroy(outlineObject);
            
            TerrainManager.onPointerClick.RemoveListener(TerrainClick);
            TerrainManager.onPointerEnter.RemoveListener(TerrainEnter);
            TerrainManager.onPointerExit.RemoveListener(TerrainExit);
            ItemInstance.onPointerClick.RemoveListener(ItemClick);
            ItemInstance.onPointerEnter.RemoveListener(ItemEnter);
            ItemInstance.onPointerExit.RemoveListener(ItemExit);
        }
        
        public override void Update()
        {
            switch (hoverMode)
            {
                case HoverMode.Terrain:
                {
                    TerrainPointerInfo terrainInfo = TerrainManager.GetRaycastInfo(cachedEventData.pointerCurrentRaycast);
                    Vector3Int position = terrainInfo.BackPosition;
                    outlineObject.transform.position = position + (Vector3.one * 0.5f);
                    outlineObject.transform.localScale = Vector3.one * 1.02f;
                    outlineObject.SetActive(true);
                    break;
                }
                case HoverMode.Item:
                {
                    GameObject item = cachedEventData.pointerCurrentRaycast.gameObject;
                    outlineObject.transform.position = item.transform.position;
                    outlineObject.transform.localScale = Vector3.one * 0.62f;
                    outlineObject.SetActive(true);
                    break;
                }
                default:
                {
                    outlineObject.SetActive(false);
                    break;
                }
            }
        }

        private void TerrainClick(PointerEventData eventData)
        {
            if (eventData.button != selectButton) return;
            
            TerrainPointerInfo terrainInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            BlockType type = terrainManager.GetBlock(terrainInfo.BackPosition);
            
            Debug.Log(type.name + " " + terrainInfo.BackPosition);
        }

        private void TerrainEnter(PointerEventData eventData)
        {
            hoverMode = HoverMode.Terrain;
            cachedEventData = eventData;
        }

        private void TerrainExit(PointerEventData eventData)
        {
            hoverMode = HoverMode.None;
            cachedEventData = null;
        }

        private void ItemClick(ItemInstance instance, PointerEventData eventData)
        {
            if (eventData.button != selectButton) return;
            
            Debug.Log(instance.item);
        }

        private void ItemEnter(ItemInstance instance, PointerEventData eventData)
        {
            hoverMode = HoverMode.Item;
            cachedEventData = eventData;
        }

        private void ItemExit(ItemInstance instance, PointerEventData eventData)
        {
            hoverMode = HoverMode.None;
            cachedEventData = null;
        }
    }
}