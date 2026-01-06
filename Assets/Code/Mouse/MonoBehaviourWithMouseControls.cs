using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MonoBehaviourWithMouseControls : MonoBehaviour
{
    protected bool isHovering;

    protected virtual void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        bool hitThisObject =
            Physics.Raycast(ray, out RaycastHit hit) &&
            hit.transform == transform;

        if (hitThisObject && !isHovering)
        {
            isHovering = true;
            OnHoverEnter();
        }
        else if (!hitThisObject && isHovering)
        {
            isHovering = false;
            OnHoverExit();
        }
    }

    protected virtual void OnHoverEnter() { }
    protected virtual void OnHoverExit() { }
}
