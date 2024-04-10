using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template{
public class StateBasedWinParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem winParticle = null;
    [SerializeField] private StateGameController gameController = null;
    private void Awake() {
        gameController.StateChanged +=
        ()=> {
            if(gameController.CurrentState == StateGameController.State.GameEnded
            && gameController.CurrentSession.LevelController.IsVictory()) 
                winParticle.Play();
        };
    }
    private void OnValidate() {
        if(gameController == null && GetComponent<StateGameController>() != null)
            gameController = GetComponent<StateGameController>();
    }
}
}