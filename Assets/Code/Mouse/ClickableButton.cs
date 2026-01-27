using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ClickableButton : MonoBehaviourWithMouseControls
{
    [SerializeField] private UnityEvent onClick;

    protected override void Update()
    {
        base.Update();
    }

    /**
     * This will be called when the mouse clicks on the button
     */
    protected override void OnMouseDown()
    {
        onClick?.Invoke();
    }
}