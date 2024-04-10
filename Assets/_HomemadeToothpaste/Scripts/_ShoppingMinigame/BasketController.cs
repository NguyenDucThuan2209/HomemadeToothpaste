using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Shopping {
    public class BasketController : MonoBehaviour {
        public Action<IngredientItem> OnItemAdded;
        [SerializeField] public GameObject droppedPoint;
        List<IngredientItem> ingredientItems = new List<IngredientItem> ();
        public void AddItem (IngredientItem itemInfo) {
            ingredientItems.Add (itemInfo);
            OnItemAdded?.Invoke (itemInfo);
        }
    }
}