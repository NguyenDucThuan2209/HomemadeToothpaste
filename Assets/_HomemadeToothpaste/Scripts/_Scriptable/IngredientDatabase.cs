using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ingredient_database", menuName = "Ingredient Database", order = 3)]
public class IngredientDatabase : ScriptableObject
{
    public List<IngredientItem> list;
}
