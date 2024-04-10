using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ingredient_name", menuName = "Ingredient Data", order = 1)]
public class IngredientItem : ScriptableObject
{
    public enum Effect { RemoveYellowTeeth, RemoveBloodyGum, Vomit }
    public enum type { Shatterable, Blendable }
    public bool NewIngredient
    {
        get
        {
            return PlayerPrefs.HasKey(ingredientName + "newIngredient") ? (PlayerPrefs.GetInt(ingredientName + "newIngredient") == 1 ? true : false) : newIngredient;
        }
        set
        {
            //NewIngredient = value;
            PlayerPrefs.SetInt(ingredientName + "newIngredient", value == true ? 1 : 0);
        }
    }
    public bool HasBought
    {
        get
        {
            return PlayerPrefs.HasKey(ingredientName + "hasBought") ? (PlayerPrefs.GetInt(ingredientName + "hasBought") == 1 ? true : false) : hasBought;
        }
        set
        {
            //HasBought = value;
            PlayerPrefs.SetInt(ingredientName + "hasBought", value == true ? 1 : 0);
        }
    }
    [SerializeField]
    bool newIngredient;
    [SerializeField]
    bool hasBought;
    public string ingredientName;
    public type Type;
    public Sprite ingredientSprite;
    public Color ingredientColor;
    public Color ingredientSecondaryColor;
    public Effect effect;
    public bool isMultipleFragmentShape;
    public List<GameObject> ingredientChunks;
    public GameObject ingredientShopPrefab;
    public int spawnAmount;
#if UNITY_EDITOR
    private void OnValidate()
    {
        NewIngredient = PlayerPrefs.HasKey(ingredientName + "newIngredient") ? (PlayerPrefs.GetInt(ingredientName + "newIngredient") == 1 ? true : false) : newIngredient;
        HasBought = PlayerPrefs.HasKey(ingredientName + "hasBought") ? (PlayerPrefs.GetInt(ingredientName + "hasBought") == 1 ? true : false) : hasBought; ;
    }

#endif
}
