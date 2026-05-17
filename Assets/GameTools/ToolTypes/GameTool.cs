using UnityEngine;

namespace GameTools.Tools
{
    public abstract class GameTool : ScriptableObject
    {
        [SerializeField] public LayerMask interactionLayerMask;
        
        public abstract Sprite Sprite { get; }

        public virtual void Select() {}
        public virtual void Deselect() {}
        public virtual void Update() {}
    }
}

