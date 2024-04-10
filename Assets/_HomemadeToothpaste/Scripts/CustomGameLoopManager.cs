using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
using LatteGames.Template;

public class CustomGameLoopManager : MonoBehaviour
{
    [SerializeField] private StateGameController gameController = null;
    [SerializeField] private ToolUnlockUI toolUnlockUI = null;
    [SerializeField] private RatingUI ratingUI = null;

    public static CustomGameLoopManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void CheckForNextUnlockTool()
    {
        if (gameController.CurrentState == StateGameController.State.GameEnded)
        {

            if (toolUnlockUI.nextUnlockedTool != null)
            {
                ratingUI.Continue += () =>
                {
                    ratingUI.Display(false); toolUnlockUI.Display(true);
                };
                toolUnlockUI.Continue += () =>
                {
                    toolUnlockUI.SetUnlockedNextTool();
                    toolUnlockUI.HideImmediately(() => { gameController.StartNextLevel(); ratingUI.Reset(); });
                };

            }
            else
            {
                ratingUI.Continue += () =>
                {
                    ratingUI.Display(false,
                    () =>
                    {
                        gameController.StartNextLevel();
                        ratingUI.Reset();
                    }
                        ); toolUnlockUI.HideImmediately();
                };

            }

        }
    }

    private void CheckToDisplay()
    {
        if (gameController.CurrentState == StateGameController.State.GameEnded)
        {
            ratingUI.Display(true);
        }
        else
        {
            ratingUI.Display(false);
        }
    }

    public void SetStarGainedToRatingUI(int starsGained)
    {
        ratingUI.starsGained = starsGained;
    }

    private void Start()
    {
        gameController.StartNextLevel();
        gameController.StateChanged += CheckForNextUnlockTool;
        gameController.StateChanged += CheckToDisplay;

    }

    private void OnValidate()
    {
        if (gameController == null && GetComponent<StateGameController>() != null)
            gameController = GetComponent<StateGameController>();
    }

    private void OnDestroy()
    {
        gameController.StateChanged -= CheckForNextUnlockTool;
    }
}
