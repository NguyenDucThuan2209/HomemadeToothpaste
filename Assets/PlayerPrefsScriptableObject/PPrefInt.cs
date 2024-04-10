using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefInt", menuName = "ScriptableObjects/PPrefInt", order = 0)]
public class PPrefInt : ScriptableObject {
    [SerializeField] private string key;
    [SerializeField] private int defaultValue;

    [System.Serializable]
    public struct ValueChangedData{
        public int oldValue;
        public int newValue;
    }
    public event Action<ValueChangedData> ValueChanged;

    public static implicit operator int(PPrefInt obj)
    {
        return PlayerPrefs.GetInt(obj.key, obj.defaultValue);
    }

    public void Set(int newValue)
    {
        var oldVal = Get();
        PlayerPrefs.SetInt(key, newValue);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    public int Get()
    {
        return (int) this;
    }

    public void Clear()
    {
        var oldVal = Get();
        PlayerPrefs.DeleteKey(key);
        var newVal = Get();
        InvokeDataChangedEvent(oldVal, newVal);
    }

    private void InvokeDataChangedEvent(int oldVal, int newVal)
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