using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools
{
    public class TerrainDragTracker
    {
        private PointerEventData cachedEventData;
        private TerrainPointerInfo pressInfo;
        private bool overTerrain;
        private bool buttonHeld;

        public bool IsDragging()
        {
            return overTerrain && cachedEventData != null;
        }

        public (TerrainPointerInfo start, TerrainPointerInfo end) GetDragInfo()
        {
            if (!IsDragging()) return default;

            TerrainPointerInfo currentInfo = TerrainManager.GetRaycastInfo(cachedEventData.pointerCurrentRaycast);

            // Hover only -> 1x1 preview
            if (!buttonHeld)
            {
                return (currentInfo, currentInfo);
            }

            // Drag selection preview
            return (pressInfo, currentInfo);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            cachedEventData = eventData;
            pressInfo = TerrainManager.GetRaycastInfo(eventData.pointerCurrentRaycast);
            buttonHeld = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            cachedEventData = eventData;
            buttonHeld = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            cachedEventData = eventData;
            overTerrain = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cachedEventData = eventData;
            overTerrain = false;
        }
    }
}