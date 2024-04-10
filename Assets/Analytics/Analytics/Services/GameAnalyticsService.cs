using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if LatteGames_GA
using GameAnalyticsSDK;
#endif
namespace LatteGames.Analytics
{
        [OptionalDependency("GameAnalyticsSDK.GameAnalytics", "LatteGames_GA")]
    public class GameAnalyticsService : AnalyticsService, LevelAchievedEvent.ILogger, LevelStartedEvent.ILogger
    {
#if LatteGames_GA
        private void Awake() {
            GameAnalytics.Initialize();
        }
#endif
        public void LevelAchieved(int levelIndex)
        {
#if LatteGames_GA
            logger.Log($"Send level achieved event lv{levelIndex} to GA");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{levelIndex}");
#endif
        }

        public void LevelStarted(int levelIndex)
        {
#if LatteGames_GA
            logger.Log($"Send level started event lv{levelIndex} to GA");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Level_{levelIndex}");
#endif
        }

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
            logger.Log($"sending custom event log {eventKey} to GA");
        }
    }
}