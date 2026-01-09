using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MonoBehaviourWithMouseControls : MonoBehaviour
{
    // Tracks whether the mouse is currently hovering this object
    protected bool isHovering;

    // Max ray distance for hover/click detection
    [SerializeField] private float rayDistance = 200f;

    // Optional layer mask to filter what can be hovered/clicked (defaults to Everything)
    [SerializeField] private LayerMask hoverMask = ~0;

    protected virtual void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        // RaycastAll so other colliders (frames/boards) don't block the card hover
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance, hoverMask, QueryTriggerInteraction.Collide);

        bool hitThisObject = false;

        // We consider a hit valid if it hits this transform OR any child collider of this transform
        for (int index = 0; index < hits.Length; index++)
        {
            Transform hitTransform = hits[index].transform;

            if (hitTransform == transform || hitTransform.IsChildOf(transform))
            {
                hitThisObject = true;
                break;
            }
        }

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

        // Mouse down (old Unity-style semantic)
        if (Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnMouseDown();
        }

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

        // Checkinf for scroll up or down
        if (scroll.y > 0f) OnWheelUp();
        else if (scroll.y < 0f) OnWheelDown();
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

    // Abstract method for mouse down
    protected virtual void OnMouseDown() { }
}
