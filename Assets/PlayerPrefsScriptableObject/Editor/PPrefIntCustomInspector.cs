using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PPrefInt))]
public class PPrefIntCustomInspector : Editor
{
    int setValue;
    public override void OnInspectorGUI()
    {
        var obj = (PPrefInt)target;
        DrawDefaultInspector ();
        EditorGUILayout.LabelField($"Current Value: {obj.Get()}");
        if(GUILayout.Button("Clear"))
            obj.Clear();
        setValue = EditorGUILayout.IntField("Set value", setValue);
        if (GUILayout.Button("Set"))
            obj.Set(setValue);
    }
}
