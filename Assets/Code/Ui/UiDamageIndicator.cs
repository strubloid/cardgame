using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class UiDamageIndicator : MonoBehaviour
{
    // Text element to display damage amount
    public TMP_Text damageText;

    // Speed at which the damage indicator moves
    public float moveSpeed = 20.0f;

    // Lifetime of the damage indicator before it disappears
    public float lifeTime = 1.2f;

    //  Reference to the RectTransform component
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Schedule the destruction of the damage indicator after its lifetime
        Destroy(gameObject, lifeTime);

        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the damage indicator upwards over time
        rectTransform.anchoredPosition += new Vector2(0, -moveSpeed * Time.deltaTime);
    }
}
