using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PPrefFloat))]
public class PPrefFloatCustomInspector : Editor
{
    float setValue;
    public override void OnInspectorGUI()
    {
        var obj = (PPrefFloat)target;
        DrawDefaultInspector ();
        EditorGUILayout.LabelField($"Current Value: {obj.Get()}");
        if(GUILayout.Button("Clear"))
            obj.Clear();
        setValue = EditorGUILayout.FloatField("Set value", setValue);
        if (GUILayout.Button("Set"))
            obj.Set(setValue);
    }
}
