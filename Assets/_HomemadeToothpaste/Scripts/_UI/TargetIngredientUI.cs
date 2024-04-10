using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TargetIngredientUI : MonoBehaviour
{
    bool isChecked;
    [SerializeField]
    GameObject checker;
    [SerializeField]
    Image background;
    [SerializeField]
    Color correctBackgroundColor;
    [SerializeField]
    Image icon;
    public IngredientItem ingredientItem;
    // Start is called before the first frame update
    private void Start()
    {
        isChecked = false;
    }


    public void LoadData()
    {
        icon.sprite = ingredientItem.ingredientSprite;

    }

    public void SetCorrect()
    {
        isChecked = true;
        background.color = correctBackgroundColor;
        checker.gameObject.SetActive(true);
    }

    //private void Update()
    //{
    //    if (!isChecked && GamePhaseManager.Instance.droppedIngredients.Contains(ingredientItem))
    //    {
    //        isChecked = true;
    //        SetCorrect();
    //    }
    //}
}
