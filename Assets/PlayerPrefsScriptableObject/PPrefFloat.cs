using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefFloat", menuName = "ScriptableObjects/PPrefFloat", order = 0)]
public class PPrefFloat : ScriptableObject {
    [SerializeField] private string key;
    [SerializeField] private float defaultValue;

    [System.Serializable]
    public struct ValueChangedData{
        public float oldValue;
        public float newValue;
    }
    public event Action<ValueChangedData> ValueChanged;

    public static implicit operator float(PPrefFloat obj)
    {
        return PlayerPrefs.GetFloat(obj.key, obj.defaultValue);
    }

    public float Get()
    {
        return (float) this;
    }

    public void Set(float newValue)
    {
        var oldVal = Get();
        PlayerPrefs.SetFloat(key, newValue);
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

    private void InvokeDataChangedEvent(float oldVal, float newVal)
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