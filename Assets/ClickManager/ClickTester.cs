using UnityEngine;

namespace ClickManager
{
    /// <summary>
    /// Simple class for testing/demonstrating the use of ClickManager
    /// </summary>
    public class ClickTester : MonoBehaviour
    {
        private void OnEnable()
        {
            // Subscribe to Game-click events
            ClickManager.leftButtonChanged.AddListener(GetLeftClick);
            ClickManager.rightButtonChanged.AddListener(GetRightClick);
        }

        private void OnDisable()
        {
            // Unsubscribe to Game-click events
            ClickManager.leftButtonChanged.RemoveListener(GetLeftClick);
            ClickManager.rightButtonChanged.RemoveListener(GetRightClick);
        }

        private void GetLeftClick(LeftButtonInfo leftButtonInfo)
        {
            // Log click info
            Debug.Log("Left Click: " + leftButtonInfo.HoverState + ", " + leftButtonInfo.mousePosition + ", " + leftButtonInfo.pressed);
        }

        private void GetRightClick(RightButtonInfo rightButtonInfo)
        {
            // Log click info
            Debug.Log("Right Click: " + rightButtonInfo.HoverState + ", " + rightButtonInfo.mousePosition + ", " + rightButtonInfo.pressed);
        }
    }
}

