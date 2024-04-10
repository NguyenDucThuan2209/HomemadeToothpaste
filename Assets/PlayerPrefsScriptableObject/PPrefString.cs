using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefString", menuName = "ScriptableObjects/PPrefString", order = 0)]
public class PPrefString : ScriptableObject {
    [SerializeField] private string key;
    [SerializeField] private string defaultValue;

    [System.Serializable]
    public struct ValueChangedData{
        public string oldValue;
        public string newValue;
    }
    public event Action<ValueChangedData> ValueChanged;

    public static implicit operator string(PPrefString obj)
    {
        return PlayerPrefs.GetString(obj.key, obj.defaultValue);
    }

    public void Set(string newValue)
    {
        var oldVal = Get();
        PlayerPrefs.SetString(key, newValue);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    public string Get()
    {
        return (string) this;
    }

    public void Clear()
    {
        var oldVal = Get();
        PlayerPrefs.DeleteKey(key);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    private void InvokeDataChangedEvent(string oldVal, string newVal)
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