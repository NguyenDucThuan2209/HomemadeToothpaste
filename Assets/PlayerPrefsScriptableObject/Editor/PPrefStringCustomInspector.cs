using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PPrefString))]
public class PPrefStringCustomInspector : Editor
{
    string setValue;
    public override void OnInspectorGUI()
    {
        var obj = (PPrefString)target;
        DrawDefaultInspector ();
        EditorGUILayout.LabelField($"Current Value: {obj.Get()}");
        if(GUILayout.Button("Clear"))
            obj.Clear();
        setValue = EditorGUILayout.TextField("Set value", setValue);
        if (GUILayout.Button("Set"))
            obj.Set(setValue);
    }
}
