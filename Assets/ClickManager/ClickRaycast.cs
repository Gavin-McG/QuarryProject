using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private static readonly RaycastHitComparer comparer = new();

        private static LayerMask castLayer;
        public static void SetCastLayer(LayerMask layer) => castLayer = layer;

        [SerializeField] private LayerMask defaultLayer;

        // Cached hover state
        private int hoverFrame = -1;
        public static IClickReceiver CurrentHover { get; private set; }
        public static RaycastHit CurrentHit { get; private set; }

        private IClickReceiver currentLeftPress;
        private IClickReceiver currentRightPress;

        private Camera cachedCamera;

        private void Start()
        {
            SetCastLayer(defaultLayer);
            cachedCamera = Camera.main;
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

        private void Update()
        {
            EnsureHoverUpdated();
        }

        private void EnsureHoverUpdated()
        {
            // Skip if current frame already queried
            if (Time.frameCount == hoverFrame) return;
            
            // Perform raycast to get info
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            CurrentHover = CastClick<IClickReceiver>(mousePosition, out RaycastHit hit);
            CurrentHit = hit;
            hoverFrame = Time.frameCount;
        }

        private void HandleLeftButton(LeftButtonInfo buttonInfo)
        {
            if (buttonInfo.HoverState != PointerHoverState.Game)
                return;
            
            EnsureHoverUpdated();

            IClickReceiver currentFocus = CurrentHover;
            RaycastHit hit = CurrentHit;

            if (buttonInfo.pressed)
            {
                currentFocus?.LeftButtonPressed(hit);
                currentLeftPress = currentFocus;
            }
            else
            {
                if (currentFocus != null)
                {
                    if (currentFocus == currentLeftPress)
                    {
                        currentFocus.LeftButtonClicked(hit);
                    }

                    currentFocus.LeftButtonReleased(hit, currentLeftPress);
                }

                currentLeftPress = null;
            }
        }

        private void HandleRightButton(RightButtonInfo buttonInfo)
        {
            if (buttonInfo.HoverState != PointerHoverState.Game)
                return;

            EnsureHoverUpdated();
            
            IClickReceiver currentFocus = CurrentHover;
            RaycastHit hit = CurrentHit;

            if (buttonInfo.pressed)
            {
                currentFocus?.RightButtonPressed(hit);
                currentRightPress = currentFocus;
            }
            else
            {
                if (currentFocus != null)
                {
                    if (currentFocus == currentRightPress)
                    {
                        currentFocus.RightButtonClicked(hit);
                    }

                    currentFocus.RightButtonReleased(hit, currentRightPress);
                }

                currentRightPress = null;
            }
        }

        private T CastClick<T>(
            Vector2 mousePosition,
            out RaycastHit hit)
            where T : class
        {
            Ray clickRay = cachedCamera.ScreenPointToRay(mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(
                clickRay.origin,
                clickRay.direction,
                Mathf.Infinity,
                castLayer);

            Array.Sort(hits, comparer);

            foreach (RaycastHit nextHit in hits)
            {
                GameObject hitObject = nextHit.collider.gameObject;

                T receiver = hitObject.GetComponent<T>();

                if (receiver != null)
                {
                    hit = nextHit;
                    return receiver;
                }
            }

            hit = default;
            return null;
        }
    }
}