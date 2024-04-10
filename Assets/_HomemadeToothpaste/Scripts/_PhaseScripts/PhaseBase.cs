using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhaseBase : MonoBehaviour
{
    [Header("Phase Common Attribute")]
    [SerializeField]
    public bool isActive;
    [SerializeField]
    public Transform phaseCameraPoint;



    public virtual void Start()
    {
        PhaseInitialization();
    }

    public abstract void PhaseProcessing();

    public abstract void PhaseInitialization();

}
