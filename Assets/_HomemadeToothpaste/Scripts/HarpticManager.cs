using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class HarpticManager : MonoBehaviour
{
    public static HarpticManager Instance;
    public HapticTypes hapticTypes = HapticTypes.LightImpact;
    public float timeBetweenHaptic;
    float lastTriggerTime;
    public bool triggerContinousHarptic;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void TriggerHaptics()
    {
        MMVibrationManager.Haptic(hapticTypes);
        // Debug.Log("Haptic Triggered");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (triggerContinousHarptic)
        {
            if (Time.time > lastTriggerTime + timeBetweenHaptic)
            {
                TriggerHaptics();
            }
        }
    }
}
