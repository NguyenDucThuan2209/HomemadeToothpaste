using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tool_name", menuName = "Tool Data", order = 5)]
public class Tools : ScriptableObject
{
    public string toolName;
    public Sprite toolSprite;
    public Sprite toolShadow;
    public int unlockAtLevel;
    public bool HasUnlocked
    {
        get
        {
            return PlayerPrefs.HasKey(toolName + "hasUnlocked") ? (PlayerPrefs.GetInt(toolName + "hasUnlocked") == 1 ? true : false) : hasUnlocked;
        }
        set
        {
            //HasBought = value;
            PlayerPrefs.SetInt(toolName + "hasUnlocked", value == true ? 1 : 0);
        }
    }
    public bool hasUnlocked;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HasUnlocked = hasUnlocked;
    }
#endif
}
