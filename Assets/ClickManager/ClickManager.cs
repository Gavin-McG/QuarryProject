using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClickManager
{
    public enum PointerHoverState { UI, Game }
    
    /// <summary>
    /// Struct containing any relevant information to a game left click
    /// </summary>
    public struct LeftButtonInfo
    {
        public PointerHoverState HoverState;
        public Vector2 mousePosition;
        public bool pressed;
    }

    /// <summary>
    /// Struct containing any relevant information to a game right click
    /// </summary>
    public struct RightButtonInfo
    {
        public PointerHoverState HoverState;
        public Vector2 mousePosition;
        public bool pressed;
    }

    /// <summary>
    /// Manager class for easily distinguishing between UI-click events and Game-click events. 
    /// </summary>
    public class ClickManager : MonoBehaviour
    {
        /// Event to listen to for left mouse button changes
        public static readonly UnityEvent<LeftButtonInfo> leftButtonChanged = new();
        /// Event to listen to for right mouse button changes
        public static readonly UnityEvent<RightButtonInfo> rightButtonChanged = new();
    
        [Tooltip("Input Action for Left clicking")]
        [SerializeField] private InputActionReference leftClickAction;
        
        [Tooltip("Input Action for Right clicking")]
        [SerializeField] private InputActionReference rightClickAction;

        private bool isPointerOverUI;

        private void OnEnable()
        {
            // Subscribe to Input Action Performed event
            leftClickAction.action.performed += HandleLeftClick;
            rightClickAction.action.performed += HandleRightClick;
        }

        private void OnDisable()
        {
            // Unsubscribe to Input Action Performed event
            leftClickAction.action.performed -= HandleLeftClick;
            rightClickAction.action.performed -= HandleRightClick;
        }

        private void Update()
        {
            isPointerOverUI = GetPointerOverUI();
        }

        /// <summary>
        /// Use EventSystem to check whether the mouse pointer is over a UI element
        /// </summary>
        private bool GetPointerOverUI()
        {
            if (EventSystem.current)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            
            //Log Error if no EventSystem exists within the scene
            Debug.LogError("ClickManager: No EventSystem found!");
            return false;
        }

        /// <summary>
        /// Gets the mouse position of the current mouse
        /// </summary>
        private Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }
    
        /// <summary>
        /// Invokes a leftClick event if the mouse is not over UI
        /// </summary>
        private void HandleLeftClick(InputAction.CallbackContext context)
        {
            //Collect info for click
            LeftButtonInfo info = new()
            {
                HoverState = isPointerOverUI ? PointerHoverState.UI : PointerHoverState.Game,
                mousePosition = GetMousePosition(),
                pressed = context.ReadValueAsButton()
            };
        
            //Publish event for listeners
            leftButtonChanged.Invoke(info);
        }

        /// <summary>
        /// Invokes a leftClick event if the mouse is not over UI
        /// </summary>
        private void HandleRightClick(InputAction.CallbackContext context)
        {
            //Collect info for click
            RightButtonInfo info = new()
            {
                HoverState = isPointerOverUI ? PointerHoverState.UI : PointerHoverState.Game,
                mousePosition = GetMousePosition(),
                pressed = context.ReadValueAsButton()
            };
        
            //Publish event for listeners
            rightButtonChanged.Invoke(info);
        }
    }
}

