using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Shopping {
    public class RequiredIngredientsHeader : MonoBehaviour {
        [SerializeField] HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField] RequiredIngredientsCell requiredIngredientsCell;
        [SerializeField] float switchToGreyScaleDuration = 0.3f;
        List<RequiredIngredientsCell> requiredIngredientsCells = new List<RequiredIngredientsCell> ();
        public void Init (List<IngredientItem> ingredientItems) {
            for (var i = 0; i < ingredientItems.Count; i++) {
                var itemInfo = ingredientItems[i];
                var cell = Instantiate (requiredIngredientsCell);
                cell.gameObject.SetActive (true);
                cell.transform.SetParent (horizontalLayoutGroup.transform);
                cell.transform.localScale = Vector3.one;
                cell.Init (itemInfo);
                requiredIngredientsCells.Add (cell);
            }
        }
        public bool CheckCorrectChosenItem (IngredientItem itemInfo) {
            foreach (var cell in requiredIngredientsCells) {
                if (cell.IsSameKindOfIngredient (itemInfo) && !cell.hasGrey) {
                    cell.SwitchToGreyScale (switchToGreyScaleDuration);
                    return true;
                }
            }
            return false;
        }
    }
}