using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template
{
    public class StateBasedGameLoopManager : MonoBehaviour
    {
        [SerializeField] private StateGameController gameController = null;
        [SerializeField] private StateUIController stateUI = null;
        private void Awake()
        {
            gameController.StateChanged +=
                () => stateUI.SetState(gameController.CurrentState);
            gameController.StateChanged +=
                () =>
                {
                    stateUI.GameOverUI.SetButtonGroup(false, true);

                };

            stateUI.GameOverUI.Replay +=
                () => gameController.StartLevel(gameController.CurrentSession.LevelAsset);
            stateUI.GameOverUI.Next +=
            () => gameController.StartNextLevel();
        }

        private void Start()
        {
            gameController.StartNextLevel();
        }

        private void OnValidate()
        {
            if (gameController == null && GetComponent<StateGameController>() != null)
                gameController = GetComponent<StateGameController>();
        }
    }
}