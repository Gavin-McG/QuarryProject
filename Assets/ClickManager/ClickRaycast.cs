using System;
using System.Collections.Generic;
using System.Linq;
using ManagerSystem;
using UnityEngine;

namespace ClickManager
{
    public class ClickRaycast : MonoBehaviour
    {
        /// <summary>
        /// Class for sorting RayCastHits by distance
        /// </summary>
        private class RaycastHitComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
        private static LayerMask castLayer;
        public static void SetCastLayer(LayerMask layer) { castLayer = layer; }
        
        [SerializeField] LayerMask defaultLayer;
        
        private IReceiveClickCast currentLeftHover = null;
        private IReceiveClickCast currentRightHover = null;

        private void Start()
        {
            SetCastLayer(defaultLayer);
        }

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
            // Skip when the mouse is over UI
            if (buttonInfo.HoverState != PointerHoverState.Game) return;
            
            // Get the currently hovering object
            IReceiveClickCast currentFocus = CastClick<IReceiveClickCast>(buttonInfo.mousePosition, out RaycastHit hit);
            
            if (buttonInfo.pressed)
            {
                // Press Button
                currentFocus?.LeftButtonPressed(hit);
                currentLeftHover = currentFocus;
            }
            else if (currentFocus != null)
            {
                // Click if hover and release are on same object
                if (currentFocus == currentLeftHover)
                {
                    currentFocus.LeftButtonClicked(hit);
                }
                
                // Release Button
                currentFocus.LeftButtonReleased(hit, currentLeftHover);
                currentLeftHover = null;
            }
        }

        private void HandleRightButton(RightButtonInfo buttonInfo)
        {
            // Skip when the mouse is over UI
            if (buttonInfo.HoverState != PointerHoverState.Game) return;
            
            // Get the currently hovering object
            IReceiveClickCast currentFocus = CastClick<IReceiveClickCast>(buttonInfo.mousePosition, out RaycastHit hit);
            
            if (buttonInfo.pressed)
            {
                // Press Button
                currentFocus?.RightButtonPressed(hit);
                currentRightHover = currentFocus;
            }
            else if (currentFocus != null)
            {
                // Click if hover and release are on same object
                if (currentFocus == currentRightHover)
                {
                    currentFocus.RightButtonClicked(hit);
                }
                
                // Release Button
                currentFocus.RightButtonReleased(hit, currentRightHover);
                currentRightHover = null;
            }
        }

        private T CastClick<T>(Vector2 mousePosition, out RaycastHit hit) where T : class
        {
            Ray clickRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(clickRay.origin, clickRay.direction, Mathf.Infinity, castLayer);
            
            // Iterate through hit objects in sorted order
            Array.Sort(hits, new RaycastHitComparer());
            foreach (RaycastHit nextHit in hits)
            {
                // Get the clickReceiver on the hit object
                GameObject hitGameObject = nextHit.collider.gameObject;
                T clickReceiver = hitGameObject.GetComponent<T>();

                // Check if the object should catch the click
                if (clickReceiver != null)
                {
                    hit = nextHit;
                    return clickReceiver;
                }
            }

            hit = new RaycastHit();
            return null;
        }
    }
}