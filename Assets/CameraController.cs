using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camera Controller inspired by TownScaper controls
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private Transform orbitTransform;
    [SerializeField] private Camera targetCamera;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference mouseDeltaAction;
    [SerializeField] private InputActionReference scrollAction;

    [SerializeField] private InputActionReference rotateButtonAction;
    [SerializeField] private InputActionReference panButtonAction;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSensitivity = 0.2f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;

    [Header("Zoom")]
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("How aggressively zoom scales exponentially")]
    [SerializeField] private float zoomExponent = 1.15f;

    [Tooltip("Scroll sensitivity")]
    [SerializeField] private float zoomStep = 1f;

    [Header("Pan")]
    [SerializeField] private float panPlaneHeightOffset = 0f;

    private float yaw;
    private float pitch;

    private float distance = 10f;

    private bool isPanning;
    private Vector2 previousMousePosition;

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        mouseDeltaAction.action.Enable();
        scrollAction.action.Enable();

        rotateButtonAction.action.Enable();
        panButtonAction.action.Enable();
    }

    private void OnDisable()
    {
        mousePositionAction.action.Disable();
        mouseDeltaAction.action.Disable();
        scrollAction.action.Disable();

        rotateButtonAction.action.Disable();
        panButtonAction.action.Disable();
    }

    private void Start()
    {
        Vector3 angles = orbitTransform.rotation.eulerAngles;

        yaw = angles.y;
        pitch = NormalizePitch(angles.x);

        ApplyRotation();
        ApplyZoom();
    }

    private void Update()
    {
        HandleMove();
        HandleRotation();
        HandlePan();
        HandleZoom();
    }
    
    #region Move

    private void HandleMove()
    {
        Vector3 forward = Vector3.ProjectOnPlane(targetCamera.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(targetCamera.transform.right, Vector3.up).normalized;
        
        //Vector3 forward = targetCamera.transform.forward;
        //Vector3 right = targetCamera.transform.right;
        
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector2 scaledInput = moveInput.normalized * (moveSpeed * distance * Time.deltaTime);
        
        Vector3 movement = scaledInput.x * right + scaledInput.y * forward;
        focusPoint.position += movement;
    }
    
    #endregion

    #region Rotation

    private void HandleRotation()
    {
        if (!rotateButtonAction.action.IsPressed())
            return;

        Vector2 delta = mouseDeltaAction.action.ReadValue<Vector2>();

        yaw += delta.x * rotationSensitivity;
        pitch -= delta.y * rotationSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        orbitTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    #endregion

    #region Pan

    private void HandlePan()
    {
        // Check pan input
        bool panPressed = panButtonAction.action.IsPressed();
        if (!panPressed)
        {
            isPanning = false;
            return;
        }
        
        // Begin pan
        Vector2 mousePosition = mousePositionAction.action.ReadValue<Vector2>();
        if (!isPanning)
        {
            isPanning = true;
            previousMousePosition = mousePosition;
            return;
        }

        // Get values for plane cast
        Ray previousRay = targetCamera.ScreenPointToRay(previousMousePosition);
        Ray currentRay = targetCamera.ScreenPointToRay(mousePosition);
        Plane focusPlane = new Plane(targetCamera.transform.forward, focusPoint.position);

        // Skip if either ray misses (safety check)
        if (!focusPlane.Raycast(previousRay, out float previousRayDistance) ||
            !focusPlane.Raycast(currentRay, out float currentRayDistance))
        {
            return;
        }

        // Apply position change
        Vector3 previousPoint = previousRay.GetPoint(previousRayDistance);
        Vector3 currentPoint = currentRay.GetPoint(currentRayDistance);
        Vector3 delta = previousPoint - currentPoint;

        focusPoint.position += delta;

        // Update state
        previousMousePosition = mousePosition;
    }

    #endregion

    #region Zoom

    private void HandleZoom()
    {
        Vector2 scroll = scrollAction.action.ReadValue<Vector2>();

        float scrollValue = scroll.y;

        if (Mathf.Approximately(scrollValue, 0f))
            return;

        /*
         * Exponential zoom:
         * Smaller movement when close
         * Larger movement when far
         */

        float zoomFactor = Mathf.Pow(
            zoomExponent,
            -scrollValue * zoomStep * Time.deltaTime * 60f);

        distance *= zoomFactor;

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        ApplyZoom();
    }

    private void ApplyZoom()
    {
        targetCamera.transform.localPosition = new Vector3(0f, 0f, -distance);
    }

    #endregion

    #region Utility

    private float NormalizePitch(float angle)
    {
        while (angle > 180f)
            angle -= 360f;

        return angle;
    }

    public void SetFocusPoint(Vector3 position)
    {
        focusPoint.position = position;
    }

    public Vector3 GetFocusPoint()
    {
        return focusPoint.position;
    }

    public void SetDistance(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        ApplyZoom();
    }

    public float GetDistance()
    {
        return distance;
    }

    public void SetRotation(float newYaw, float newPitch)
    {
        yaw = newYaw;
        pitch = Mathf.Clamp(newPitch, minPitch, maxPitch);

        ApplyRotation();
    }

    public Vector2 GetRotation()
    {
        return new Vector2(yaw, pitch);
    }

    public void FocusBounds(Bounds bounds)
    {
        SetFocusPoint(bounds.center);
    }

    #endregion
}