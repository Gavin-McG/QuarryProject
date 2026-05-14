
using UnityEngine;

namespace ClickManager
{
    public interface IReceiveClickCast
    {
        /// <summary>
        /// Called when a right click cast hits an object
        /// </summary>
        /// <returns>true if the object should consume the click</returns>
        public bool ReceiveRightPress(RaycastHit hit) { return false; }
        
        /// <summary>
        /// Called after a captured right click when the mouse is released
        /// </summary>
        public void ReleaseRightPress() {}
        
        /// <summary>
        /// Called when a left click cast hits an object
        /// </summary>
        /// <returns>true if the object should consume the click</returns>
        public bool ReceiveLeftPress(RaycastHit hit) { return false; }
        
        /// <summary>
        /// Called after a captured left click when the mouse is released
        /// </summary>
        public void ReleaseLeftPress() {}
    }
}
