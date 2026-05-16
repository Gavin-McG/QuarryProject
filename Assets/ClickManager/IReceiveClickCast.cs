
using UnityEngine;

namespace ClickManager
{
    public interface IReceiveClickCast
    {
        /// <summary>
        /// Called if the right mouse button is pressed while hovering over the object
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        public void RightButtonPressed(RaycastHit hit) { }
        
        /// <summary>
        /// Called if the right mouse button is released while hovering over the object
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        /// <param name="pressedObject">The object which was pressed</param>
        public void RightButtonReleased(RaycastHit hit, IReceiveClickCast pressedObject) {}
        
        /// <summary>
        /// Called if the right mouse button is released on the same object it was pressed
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        public void RightButtonClicked(RaycastHit hit) {}
        
        /// <summary>
        /// Called if the left mouse button is pressed while hovering over the object
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        public void LeftButtonPressed(RaycastHit hit) { }
        
        /// <summary>
        /// Called if the left mouse button is released while hovering over the object
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        /// <param name="pressedObject">The object which was pressed</param>
        public void LeftButtonReleased(RaycastHit hit, IReceiveClickCast pressedObject) {}
        
        /// <summary>
        /// Called if the left mouse button is released on the same object it was pressed
        /// </summary>
        /// <param name="hit">The results of the cast</param>
        public void LeftButtonClicked(RaycastHit hit) {}
    }
}
