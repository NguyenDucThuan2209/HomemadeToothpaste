using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    [CreateAssetMenu(fileName = "LevelStorage", menuName = "LatteGames/ScriptableObject/LevelManagement/LevelStorage", order = 0)]
    public class LevelStorage : ScriptableObject
    {
        [SerializeField] private List<LevelAsset> levelAssets = new List<LevelAsset>();

        public enum EndOfLevelBehaviour
        {
            Stay,
            LoopBack,
            Random
        }

        public LevelAsset GetLevel(int levelIndex, EndOfLevelBehaviour endOfLevelBehaviour = EndOfLevelBehaviour.Stay)
        {
            switch (endOfLevelBehaviour)
            {
                case EndOfLevelBehaviour.LoopBack:
                    levelIndex = levelIndex % levelAssets.Count;
                    break;
                case EndOfLevelBehaviour.Stay:
                    levelIndex = Mathf.Clamp(levelIndex, 0, levelAssets.Count - 1);
                    break;
                case EndOfLevelBehaviour.Random:
                    {
                        int preLevelIndex = levelIndex;
                        if (levelIndex > levelAssets.Count - 1)
                        {

                            int previousLevelArchived = PlayerPrefs.HasKey("PreviousLevelArchived") ? PlayerPrefs.GetInt("PreviousLevelArchived") : 0;
                            Debug.Log(previousLevelArchived + "," + levelIndex);
                            if (previousLevelArchived == levelIndex)
                            {

                                int randomIndex = PlayerPrefs.HasKey("PreviousRandomLevelIndex") ? PlayerPrefs.GetInt("PreviousRandomLevelIndex") : Random.Range(0, levelAssets.Count);
                                PlayerPrefs.SetInt("PreviousRandomLevelIndex", randomIndex);
                                levelIndex = randomIndex;
                            }
                            else
                            {
                                int randomIndex = Random.Range(0, levelAssets.Count);
                                PlayerPrefs.SetInt("PreviousRandomLevelIndex", randomIndex);
                                levelIndex = randomIndex;
                            }
                            PlayerPrefs.SetInt("PreviousLevelArchived", preLevelIndex);
                        }
                        else
                        {
                            levelIndex = Mathf.Clamp(levelIndex, 0, levelAssets.Count - 1);

                        }
                        break;
                    }
            }
            return levelAssets[levelIndex];
        }

        public int GetLevelIndex(LevelAsset levelAsset)
        {
            return levelAssets.IndexOf(levelAsset);
        }
    }
}