using UnityEngine;

namespace ClickManager
{
    public interface ISelectable
    {
        public Bounds GetSelectionRect(RaycastHit hit);
    }
}
