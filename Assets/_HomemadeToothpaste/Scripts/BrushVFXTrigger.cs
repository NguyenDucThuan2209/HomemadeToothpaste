using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushVFXTrigger : MonoBehaviour
{

    public float brushedTimes;

    [SerializeField]
    ParticleSystem myParticle;
    bool triggered;


    private void Start()
    {
        BrushingPhase brushingPhase = (BrushingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.Brush);
        brushedTimes = Random.Range(brushingPhase.randomBrushingRange.x, brushingPhase.randomBrushingRange.y);
    }
    private void OnTriggerEnter(Collider other)
    {
        BrushingPhase brushingPhase = (BrushingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.Brush);
        if (other.CompareTag("Brush") && brushingPhase.brushing)
        {
            if (!triggered)
            {
                triggered = true;
                brushingPhase.IncreaseBrushedTime(1f);
                myParticle.gameObject.SetActive(true);
            }
            else
            {
                brushingPhase.IncreaseBrushedTime(1f);
            }
        }

    }
}
