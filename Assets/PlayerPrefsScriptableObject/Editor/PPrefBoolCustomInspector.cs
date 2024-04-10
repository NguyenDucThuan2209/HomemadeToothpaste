﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PPrefBool))]
public class PPrefBoolCustomInspector : Editor
{
    bool setValue;
    public override void OnInspectorGUI()
    {
        var obj = (PPrefBool)target;
        DrawDefaultInspector ();
        EditorGUILayout.LabelField($"Current Value: {obj.Get()}");
        if(GUILayout.Button("Clear"))
            obj.Clear();
        setValue = EditorGUILayout.Toggle("Set value", setValue);
        if (GUILayout.Button("Set"))
            obj.Set(setValue);
    }
}
