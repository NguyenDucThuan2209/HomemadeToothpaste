using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefBool", menuName = "ScriptableObjects/PPrefBool", order = 0)]
public class PPrefBool : ScriptableObject {
    [SerializeField] private string key;
    [SerializeField] private bool defaultValue;

    [System.Serializable]
    public struct ValueChangedData{
        public bool oldValue;
        public bool newValue;
    }
    public event Action<ValueChangedData> ValueChanged;

    public static implicit operator bool(PPrefBool obj)
    {
        return PlayerPrefs.GetInt(obj.key, obj.defaultValue?1:0)==1;
    }

    public bool Get()
    {
        return (bool) this;
    }

    public void Set(bool newValue)
    {
        var oldVal = Get();
        PlayerPrefs.SetInt(key, newValue?1:0);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    public void Clear()
    {
        var oldVal = Get();
        PlayerPrefs.DeleteKey(key);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    private void InvokeDataChangedEvent(bool oldVal, bool newVal)
    {
        if(oldVal == newVal)
            return;
        RaiseEvent(new ValueChangedData(){
            oldValue = oldVal,
            newValue = newVal
        });
    }

    private void RaiseEvent(ValueChangedData data)
    {
        if(ValueChanged != null)
        {
               foreach(var handler in ValueChanged.GetInvocationList())
               {
                    try
                    {
                        handler.Method.Invoke(handler.Target, new object[]{data});
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(e);
                    }
               }
        }
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        if(string.IsNullOrEmpty(key))
            key = name;
    }

    private void OnEnable() {
        if(string.IsNullOrEmpty(key))
            key = name;
    }
    #endif
}