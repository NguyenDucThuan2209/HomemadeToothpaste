using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool_Database", menuName = "Tool Database", order = 4)]
public class ToolDatabase : ScriptableObject
{
    public List<Tools> list;
}
