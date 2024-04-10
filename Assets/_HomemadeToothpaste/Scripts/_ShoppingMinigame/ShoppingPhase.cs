using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Shopping
{
    public class ShoppingPhase : MonoBehaviour
    {
        [SerializeField] List<IngredientItem> requiredIngredientItems;
        [SerializeField] List<IngredientItem> availableItems;
        [SerializeField] List<ShopItemController> shopItemHolders;
        [SerializeField] ChoosingStateUIController choosingStateUIController;
        [SerializeField] CustomLevelController customLevelController;
        [SerializeField] PPrefBool isDisableInput;
        TargetIngredientDockUI targetIngredientDockUI;
        BasketController basketController;
        private void Awake()
        {
            if (availableItems.Count > 0 && availableItems.Count <= 12)
            {
                for (int i = 0; i < availableItems.Count; i++)
                {
                    shopItemHolders[i].SetItemInfo(availableItems[i]);
                    shopItemHolders[i].gameObject.SetActive(true);
                }
            }
            basketController = GetComponentInChildren<BasketController>();
            targetIngredientDockUI = GetComponentInChildren<TargetIngredientDockUI>();
            basketController.OnItemAdded += OnItemIsAddedToBasket;
            targetIngredientDockUI.requiredIngredients = requiredIngredientItems;
            targetIngredientDockUI.OnMeetRequirement += EndGame;
            isDisableInput.Set(false);
        }
        void OnItemIsAddedToBasket(IngredientItem itemInfo)
        {
            var oldCheckedItemListCount = targetIngredientDockUI.checkedIngredients.Count;
            targetIngredientDockUI.OwnedIngredientItemAdd(itemInfo);
            var isWrong = oldCheckedItemListCount == targetIngredientDockUI.checkedIngredients.Count;
            choosingStateUIController.ShowStateText(!isWrong);
        }
        void EndGame()
        {
            isDisableInput.Set(true);
            customLevelController.LevelState = CustomLevelController.State.Win;
            targetIngredientDockUI.gameObject.SetActive(false);
            CustomGameLoopManager.Instance.SetStarGainedToRatingUI(3);
            customLevelController.EndLevel();
        }
    }
}