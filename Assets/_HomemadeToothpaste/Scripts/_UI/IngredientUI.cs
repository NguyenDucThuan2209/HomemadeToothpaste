using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientUI : MonoBehaviour
{
    public IngredientItem ingredientItem;
    Button UIButton;
    [SerializeField]
    Image icon;
    [SerializeField]
    Image background;
    [SerializeField]
    TextMeshProUGUI text;
    public GameObject NewIndicator;
    // Start is called before the first frame update
    void Start()
    {
        NewIndicator.gameObject.SetActive(ingredientItem.NewIngredient);

        //else
        //{
        //    NewIndicator.gameObject.SetActive(false);
        //}

        UIButton = GetComponent<Button>();
        icon.sprite = ingredientItem.ingredientSprite;
        text.text = ingredientItem.ingredientName.ToUpper();
        if (UIButton != null)
        {
            UIButton.onClick.AddListener(() => { OnClick(); });
        }
    }


    void OnClick()
    {
        UIButton.interactable = false;
        background.color = UIButton.colors.disabledColor;
        if (ingredientItem != null)
        {
            PhaseBase phaseBase = GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend);
            ingredientItem.NewIngredient = false;
            NewIndicator.gameObject.SetActive(ingredientItem.NewIngredient);
            if (phaseBase.GetType() == typeof(BlenderPhase))
            {
                BlenderPhase blenderPhase = (BlenderPhase)phaseBase;
                blenderPhase.DropIngredientIntoBlender(ingredientItem);
            }
            else
            {
                StampingPhase stampingPhase = (StampingPhase)phaseBase;
                stampingPhase.DropIngredientIntoMortal(ingredientItem);
            }

        }
    }

}
