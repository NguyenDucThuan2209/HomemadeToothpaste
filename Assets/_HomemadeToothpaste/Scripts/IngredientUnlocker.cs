using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
public class IngredientUnlocker : MonoBehaviour
{
    [SerializeField]
    List<IngredientItem> unlockIngredients;

    CustomLevelController customLevelController;
    // Start is called before the first frame update
    void Start()
    {
        customLevelController = FindObjectOfType<CustomLevelController>();
        customLevelController.LevelEnded += UnlockIngredientOnEnding;
    }

    private void OnDisable()
    {
        customLevelController.LevelEnded -= UnlockIngredientOnEnding;
    }

    void UnlockIngredientOnEnding(LevelController levelController)
    {
        if (unlockIngredients.Count > 0)
        {
            foreach (var i in unlockIngredients)
            {
                i.HasBought = true;
                i.NewIngredient = true;
            }
        }
    }
}
