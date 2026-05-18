using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Mine Tool", menuName = "Scriptable Objects/Tools/Mine Tool")]
    public class DeleteTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private GameObject outlinePrefab;
        [SerializeField] private PointerEventData.InputButton deleteButton =
            PointerEventData.InputButton.Left;

        public override Sprite Sprite => toolSprite;

        private TerrainManager terrainManager;

        private GameObject outlineObject;

        private PointerEventData cachedEventData;

        private Vector3Int pressPosition;

        private bool overTerrain;
        private bool buttonHeld;

        public override void Select()
        {
            base.Select();

            terrainManager = Managers.GetManager<TerrainManager>();

            outlineObject = Instantiate(outlinePrefab);
            outlineObject.SetActive(false);

            TerrainManager.onPointerDown.AddListener(TerrainDown);
            TerrainManager.onPointerUp.AddListener(TerrainUp);
            TerrainManager.onPointerEnter.AddListener(TerrainEnter);
            TerrainManager.onPointerExit.AddListener(TerrainExit);
        }

        public override void Deselect()
        {
            base.Deselect();

            Destroy(outlineObject);

            TerrainManager.onPointerDown.RemoveListener(TerrainDown);
            TerrainManager.onPointerUp.RemoveListener(TerrainUp);
            TerrainManager.onPointerEnter.RemoveListener(TerrainEnter);
            TerrainManager.onPointerExit.RemoveListener(TerrainExit);
        }

        private bool IsDragging()
        {
            return overTerrain && cachedEventData != null;
        }

        private (Vector3Int min, Vector3Int max) GetDragBounds()
        {
            if (!IsDragging()) return default;
            
            TerrainPointerInfo currentInfo =
                TerrainManager.GetRaycastInfo(cachedEventData.pointerCurrentRaycast);

            Vector3Int currentPosition = currentInfo.BackPosition;
            
            // Hover only -> 1x1 preview
            if (!buttonHeld)
            {
                outlineObject.transform.position = currentPosition + Vector3.one * 0.5f;
                outlineObject.transform.localScale = Vector3.one;
                return (currentPosition, currentPosition);
            }

            // Drag selection preview
            Vector3Int min = Vector3Int.Min(pressPosition, currentPosition);
            Vector3Int max = Vector3Int.Max(pressPosition, currentPosition);
            return (min, max);
        }

        public override void Update()
        {
            if (!IsDragging())
            {
                outlineObject.SetActive(false);
                return;
            }
            
            outlineObject.SetActive(true);

            var bounds = GetDragBounds();

            Vector3 scale = (bounds.max - bounds.min) + Vector3.one * 1.02f;
            Vector3 center = (bounds.min + bounds.max + Vector3.one) * 0.5f;

            outlineObject.transform.position = center;
            outlineObject.transform.localScale = scale;
        }
        
        private void TerrainDown(PointerEventData eventData)
        {
            if (eventData.button != deleteButton)
                return;

            cachedEventData = eventData;

            buttonHeld = true;

            TerrainPointerInfo info =
                TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);

            pressPosition = info.BackPosition;
        }

        private void TerrainUp(PointerEventData eventData)
        {
            if (eventData.button != deleteButton)
                return;

            if (IsDragging())
            {
                var bounds = GetDragBounds();
                
                for (int x = bounds.min.x; x <= bounds.max.x; x++)
                for (int y = bounds.min.y; y <= bounds.max.y; y++)
                for (int z = bounds.min.z; z <= bounds.max.z; z++)
                {
                    terrainManager.SetBlock(new Vector3Int(x, y, z), null);
                }
            }

            cachedEventData = eventData;
            buttonHeld = false;
        }

        private void TerrainEnter(PointerEventData eventData)
        {
            cachedEventData = eventData;
            overTerrain = true;
        }

        private void TerrainExit(PointerEventData eventData)
        {
            cachedEventData = eventData;
            overTerrain = false;
        }
    }
}