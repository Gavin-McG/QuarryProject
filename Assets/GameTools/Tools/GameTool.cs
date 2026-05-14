using Terrain;
using UnityEngine;

namespace GameTools.Tools
{
    public abstract class GameTool : ScriptableObject
    {
        public abstract Sprite Sprite { get; }

        public virtual void Select() {}
        public virtual void Deselect() {}
        
        public virtual void PressLeft(TerrainHoverInfo info) {}
        public virtual void ReleaseLeft(TerrainHoverInfo info) {}
        public virtual void PressRight(TerrainHoverInfo info) {}
        public virtual void ReleaseRight(TerrainHoverInfo info) {}
    }
}

