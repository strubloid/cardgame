using UnityEngine;

[CreateAssetMenu(fileName = "PowerScriptableObject", menuName = "Scriptable Objects/PowerScriptableObject")]
public class PowerScriptableObject : ScriptableObject
{
    [Header("Identity")]
    public string powerId;
    public string powerName;
    [TextArea(3, 6)]
    public string description;
    public float powerValue;

    [Header("Limits")]
    public int maxTriggersPerTurn;
    public bool oncePerGame;
    public float duration;
    public float cooldown;

    [Header("Effect")]
    public GameObject effect;
}