using System;
using System.Collections;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;

namespace LatteGames
{
    public class StateGameController : MonoBehaviour
    {
        public event Action StateChanged = delegate { };

        public enum State
        {
            Prepare,
            Playing,
            Pause,
            GameEnded
        }

        private State currentState;
        public State CurrentState
        {
            get => currentState;
            private set
            {
                State oldState = currentState;
                currentState = value;
                if (currentState != oldState)
                    StateChanged.Invoke();
            }
        }
        [SerializeField] private LevelStorage levelStorage = null;
        public LevelStorage LevelStorage => levelStorage;
        public PlayerPrefPersistent.Int playerAchievedLevel = new PlayerPrefPersistent.Int("PLAYER_ACHIEVED_LEVEL", -1);
        private GameSession currentSession = null;
        public GameSession CurrentSession => currentSession;

        private CoroutineRunner levelLoadingRunner;
        private void Awake()
        {
            levelLoadingRunner = CoroutineRunner.CreateCoroutineRunner(false);
            levelLoadingRunner.transform.SetParent(transform);
        }

        public void StartNextLevel()
        {
            if (levelStorage == null)
            {
                Debug.LogWarning("LevelStorage is null");
                return;
            }
            var nextLevel = levelStorage.GetLevel(playerAchievedLevel.Value + 1, LevelStorage.EndOfLevelBehaviour.Random);
            StartLevel(nextLevel);
        }

        public void StartLevel(LevelAsset level)
        {


            levelLoadingRunner.StartManagedCoroutine(
                StartGameLoop(level),
                CoroutineRunner.InteruptBehaviour.Ignore
            );
        }

        private IEnumerator StartGameLoop(LevelAsset level)
        {
            if (currentSession != null)
            {
                var unload = currentSession.LevelAsset.UnLoadLevelAsync(currentSession.LevelController);
                yield return new WaitUntil(() => unload.Finished());
            }
            var loading = level.LoadLevelAsync();
            yield return new WaitUntil(() => loading.Finished());
            var newSession = new GameSession(level, loading.GetLevelController());
            currentState = State.Prepare;

            StartCoroutine(GameLoopCR(newSession));
        }

        private IEnumerator GameLoopCR(GameSession session)
        {
            currentSession = session;
            CurrentState = State.Playing;
            bool gameEnded = false;
            Action<LevelController> gameEndListener = _ => gameEnded = true;
            session.LevelController.LevelEnded += gameEndListener;
            yield return new WaitUntil(() => gameEnded);
            session.LevelController.LevelEnded -= gameEndListener;
            CurrentState = State.GameEnded;
            //if (session.LevelController.IsVictory())
            //{
            //    playerAchievedLevel.Value = levelStorage.GetLevelIndex(session.LevelAsset);
            //}

            playerAchievedLevel.Value = playerAchievedLevel.Value + 1;
        }

        public void Pause()
        {
            if (CurrentState != State.Playing)
                return;
            currentSession.LevelController.PauseLevel();
            CurrentState = State.Pause;
        }

        public void Resume()
        {
            if (CurrentState != State.Pause)
                return;
            currentSession.LevelController.ResumeLevel();
            CurrentState = State.Playing;
        }
    }
}