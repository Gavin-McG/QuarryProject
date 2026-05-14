using System;
using System.Collections.Generic;
using System.Linq;
using ManagerSystem;
using UnityEngine;

namespace ClickManager
{
    public class ClickRaycast : MonoBehaviour
    {
        private class RaycastHitComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
        private IReceiveClickCast currentLeftHover = null;
        private IReceiveClickCast currentRightHover = null;
        
        private void OnEnable()
        {
            ClickManager.leftButtonChanged.AddListener(HandleLeftButton);
            ClickManager.rightButtonChanged.AddListener(HandleRightButton);
        }

        private void OnDisable()
        {
            ClickManager.leftButtonChanged.RemoveListener(HandleLeftButton);
            ClickManager.rightButtonChanged.RemoveListener(HandleRightButton);
        }

        private void HandleLeftButton(LeftButtonInfo buttonInfo)
        {
            if (buttonInfo.pressed)
            {
                // Cast if mouse not over UI
                if (buttonInfo.HoverState == PointerHoverState.Game)
                {
                    CastLeftClick(buttonInfo.mousePosition);
                }
            }
            else if (currentLeftHover != null)
            {
                // Release click
                currentLeftHover.ReleaseLeftPress();
                currentLeftHover = null;
            }
        }

        private void HandleRightButton(RightButtonInfo buttonInfo)
        {
            if (buttonInfo.pressed)
            {
                // Cast if mouse not over UI
                if (buttonInfo.HoverState == PointerHoverState.Game)
                {
                    CastRightClick(buttonInfo.mousePosition);
                }
            }
            else if (currentRightHover != null)
            {
                // Release click
                currentRightHover.ReleaseRightPress();
                currentRightHover = null;
            }
        }

        private void CastLeftClick(Vector2 mousePosition)
        {
            Ray clickRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(clickRay.origin, clickRay.direction);
            
            // Iterate through hit objects in sorted order
            Array.Sort(hits, new RaycastHitComparer());
            foreach (RaycastHit hit in hits)
            {
                // Get the clickReceiver on the hit object
                GameObject hitGameObject = hit.collider.gameObject;
                IReceiveClickCast clickReceiver = hitGameObject.GetComponent<IReceiveClickCast>();

                // Check if the object should catch the click
                if (clickReceiver != null && clickReceiver.ReceiveLeftPress(hit))
                {
                    currentLeftHover = clickReceiver;
                    break;
                }
            }
        }
        
        private void CastRightClick(Vector2 mousePosition)
        {
            Ray clickRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(clickRay.origin, clickRay.direction);
            
            // Iterate through hit objects in sorted order
            Array.Sort(hits, new RaycastHitComparer());
            foreach (RaycastHit hit in hits)
            {
                // Get the clickReceiver on the hit object
                GameObject hitGameObject = hit.collider.gameObject;
                IReceiveClickCast clickReceiver = hitGameObject.GetComponent<IReceiveClickCast>();

                // Check if the object should catch the click
                if (clickReceiver != null && clickReceiver.ReceiveRightPress(hit))
                {
                    currentRightHover = clickReceiver;
                    break;
                }
            }
        }
    }
}