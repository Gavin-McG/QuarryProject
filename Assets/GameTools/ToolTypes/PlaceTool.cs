using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Place Tool", menuName = "Scriptable Objects/Tools/Place Tool")]
    public class PlaceTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private BlockType block;
        [SerializeField] private GameObject outlinePrefab;
        [SerializeField] private PointerEventData.InputButton button = PointerEventData.InputButton.Left;

        public override Sprite Sprite => toolSprite;

        private TerrainManager terrainManager;
        private GameObject outlineObject;
        private TerrainDragTracker dragTracker;

        public override void Select()
        {
            base.Select();

            terrainManager = Managers.GetManager<TerrainManager>();
            outlineObject = Instantiate(outlinePrefab);
            outlineObject.SetActive(false);
            dragTracker = new TerrainDragTracker();

            TerrainManager.onPointerDown.AddListener(TerrainDown);
            TerrainManager.onPointerUp.AddListener(TerrainUp);
            TerrainManager.onPointerEnter.AddListener(TerrainEnter);
            TerrainManager.onPointerExit.AddListener(TerrainExit);
        }

        public override void Deselect()
        {
            base.Deselect();

            terrainManager = null;
            Destroy(outlineObject);
            dragTracker = null;

            TerrainManager.onPointerDown.RemoveListener(TerrainDown);
            TerrainManager.onPointerUp.RemoveListener(TerrainUp);
            TerrainManager.onPointerEnter.RemoveListener(TerrainEnter);
            TerrainManager.onPointerExit.RemoveListener(TerrainExit);
        }

        public override void Update()
        {
            if (!dragTracker.IsDragging())
            {
                outlineObject.SetActive(false);
                return;
            }
            
            outlineObject.SetActive(true);

            var dragInfo = dragTracker.GetDragInfo();

            // Calculate min/max
            Vector3Int startBlock = dragInfo.start.FrontPosition;
            Vector3Int endBlock = dragInfo.end.FrontPosition;
            Vector3 min = Vector3.Min(startBlock, endBlock);
            Vector3 max = Vector3.Max(startBlock, endBlock);
            
            // Apply position & scale to preview
            Vector3 scale = (max - min) + Vector3.one * 1.02f;
            Vector3 center = (min + max + Vector3.one) * 0.5f;

            outlineObject.transform.position = center;
            outlineObject.transform.localScale = scale;
        }
        
        private void TerrainDown(PointerEventData eventData)
        {
            if (eventData.button != button) return;
            dragTracker.OnPointerDown(eventData);
        }

        private void TerrainUp(PointerEventData eventData)
        {
            if (eventData.button != button) return;

            // Delete blocks within drag Range
            if (dragTracker.IsDragging())
            {
                var dragInfo = dragTracker.GetDragInfo();
                Vector3Int startBlock = dragInfo.start.FrontPosition;
                Vector3Int endBlock = dragInfo.end.FrontPosition;
                Vector3Int min = Vector3Int.Min(startBlock, endBlock);
                Vector3Int max = Vector3Int.Max(startBlock, endBlock);
                
                for (int x = min.x; x <= max.x; x++)
                for (int y = min.y; y <= max.y; y++)
                for (int z = min.z; z <= max.z; z++)
                {
                    terrainManager.SetBlock(block, new Vector3Int(x, y, z));
                }
            }
            
            dragTracker.OnPointerUp(eventData);
        }

        private void TerrainEnter(PointerEventData eventData)
        {
            dragTracker.OnPointerEnter(eventData);
        }

        private void TerrainExit(PointerEventData eventData)
        {
            dragTracker.OnPointerExit(eventData);
        }
    }
}