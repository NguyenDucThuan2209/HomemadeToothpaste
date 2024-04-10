using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
using Tabtale.TTPlugins;

public class CustomLevelController : LevelController
{


    public enum State { Playing, Win, Lose }
    public State LevelState;
    StateGameController stateGameController;
    private void Start()
    {
        StartLevel();
    }

    public override void StartLevel()
    {
        stateGameController = FindObjectOfType<StateGameController>();
        TTPGameProgression.FirebaseEvents.MissionStarted(stateGameController.playerAchievedLevel.Value + 2, new Dictionary<string, object>());
        base.StartLevel();
    }

    public override bool IsVictory()
    {
        return LevelState == State.Win;
    }

    public override void EndLevel()
    {


        switch (LevelState)
        {
            case State.Win:
                {
                    TTPGameProgression.FirebaseEvents.LevelUp(stateGameController.playerAchievedLevel.Value + 2, new Dictionary<string, object>());
                    TTPGameProgression.FirebaseEvents.MissionComplete(new Dictionary<string, object>());
                    break;
                }
            case State.Lose:
                {

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("level", stateGameController.playerAchievedLevel.Value + 2);
                    TTPGameProgression.FirebaseEvents.MissionFailed(param);
                    break;
                }
        }
        base.EndLevel();
    }

}
