using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowderPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pestle"))
        {
            StampingPhase stampingPhase = (StampingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend);
            stampingPhase.Powderization(false);
        }
    }
}
