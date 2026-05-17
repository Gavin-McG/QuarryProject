using ClickManager;
using ItemSystem;
using Terrain;
using UnityEngine;

namespace GameTools.Tools
{
    public abstract class GameTool : ScriptableObject
    {
        [SerializeField] protected LayerMask interactionLayerMask;
        
        public abstract Sprite Sprite { get; }

        public virtual void Select()
        {
            ClickRaycast.SetCastLayer(interactionLayerMask);
        }
        public virtual void Deselect() {}
        
        public virtual void Update() {}
        
        // Terrain methods
        public virtual void TerrainLeftButtonPressed(TerrainPointerInfo info) {}
        public virtual void TerrainLeftButtonReleased(TerrainPointerInfo info) {}
        public virtual void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo) {}
        
        public virtual void TerrainRightButtonPressed(TerrainPointerInfo info) {}
        public virtual void TerrainRightButtonReleased(TerrainPointerInfo info) {}
        public virtual void TerrainRightButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo) {}

        public virtual void ItemLeftButtonClicked(ItemInstance item) {}
        public virtual void ItemRightButtonClicked(ItemInstance item) {}
    }
}

