
using UnityEngine;

namespace ClickManager
{
    public interface IReceiveClickCast
    {
        /// <summary>
        /// Called if the right mouse button is pressed while hovering over the object
        /// </summary>
        public void RightButtonPressed(RaycastHit hit) { }
        
        /// <summary>
        /// Called if the right mouse button is released while hovering over the object
        /// </summary>
        public void RightButtonReleased(RaycastHit hit) {}
        
        /// <summary>
        /// Called if the right mouse button is released on the same object it was pressed
        /// </summary>
        public void RightButtonClicked(RaycastHit hit) {}
        
        /// <summary>
        /// Called if the left mouse button is pressed while hovering over the object
        /// </summary>
        public void LeftButtonPressed(RaycastHit hit) { }
        
        /// <summary>
        /// Called if the left mouse button is released while hovering over the object
        /// </summary>
        public void LeftButtonReleased(RaycastHit hit) {}
        
        /// <summary>
        /// Called if the left mouse button is released on the same object it was pressed
        /// </summary>
        public void LeftButtonClicked(RaycastHit hit) {}
    }
}
