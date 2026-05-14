using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRayDebugger : MonoBehaviour
{
    private Vector3 hitPoint;
    private bool hasHit;

    private void Update()
    {
        if (Camera.main == null || Mouse.current == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        hasHit = Physics.Raycast(ray, out RaycastHit hit);

        if (hasHit)
            hitPoint = hit.point;
    }

    private void OnDrawGizmos()
    {
        if (!hasHit)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint, 0.2f);
    }
}