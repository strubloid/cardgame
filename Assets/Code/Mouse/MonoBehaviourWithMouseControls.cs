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

        // Hover enter / exit detection
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

        if (!isHovering)
            return;

        // Checking for left click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftClick();
        }

        // Checking for right click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnRightClick();
        }

        
        // Checking for mouse wheel
        Vector2 scroll = Mouse.current.scroll.ReadValue();

        if (scroll.y > 0f)
        {
            OnWheelUp();
        }
        else if (scroll.y < 0f)
        {
            OnWheelDown();
        }

    }

    // Abstract methods for hover enter and exit
    protected virtual void OnHoverEnter() { }
    protected virtual void OnHoverExit() { }

    // Abstract methods for left and right clicks
    protected virtual void OnLeftClick() { }
    protected virtual void OnRightClick() { }

    // Abstract methods for mouse wheel
    protected virtual void OnWheelUp() { }
    protected virtual void OnWheelDown() { }
}
