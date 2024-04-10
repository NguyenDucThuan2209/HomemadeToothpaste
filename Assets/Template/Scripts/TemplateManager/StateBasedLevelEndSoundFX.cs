using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template
{
    [RequireComponent(typeof(StateGameController))]
    public class StateBasedLevelEndSoundFX : MonoBehaviour
    {
        private StateGameController _gameController;
        private StateGameController gameController
        {
            get{
                if(_gameController == null)
                    _gameController = GetComponent<StateGameController>();
                return _gameController;
            }
        }

        [SerializeField]
        private AudioSource winSound = null;
        [SerializeField]
        private AudioSource loseSound = null;

        private void Awake() {
            gameController.StateChanged += OnStateChanged;
        }

        private void OnDestroy() {
            gameController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged()
        {
            if(gameController.CurrentState == StateGameController.State.GameEnded)
            {
                if(gameController.CurrentSession.LevelController.IsVictory())
                {
                    winSound.Play();
                }
                else
                {
                    loseSound.Play();
                }
            }
        }
    }
}