using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientDockUI : MonoBehaviour
{

    [SerializeField]
    IngredientDatabase ingredientDatabase;
    [SerializeField]
    List<IngredientUI> ingredientItemList = new List<IngredientUI>();
    [SerializeField]
    List<IngredientItem> unlockedItem = new List<IngredientItem>();


    [Header("UI Position")]
    [SerializeField]
    Vector3 hidePosition;
    [SerializeField]
    Vector3 showPosition;
    [SerializeField]
    float displaySpeed;
    RectTransform rectTransform;
    bool shown;
    // Start is called before the first frame update
    void Start()
    {

        shown = false;
        rectTransform = GetComponent<RectTransform>();
        LoadDock();

    }

    void LoadDock()
    {
        for (int i = 0; i < ingredientDatabase.list.Count; i++)
        {
            if (ingredientDatabase.list[i].HasBought)
            {
                if (GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend).GetType() == typeof(BlenderPhase))
                {
                    if (ingredientDatabase.list[i].Type == IngredientItem.type.Blendable)
                    {
                        if (ingredientDatabase.list[i].NewIngredient)
                        {
                            unlockedItem.Insert(0, ingredientDatabase.list[i]);
                        }
                        else
                        {
                            unlockedItem.Add(ingredientDatabase.list[i]);
                        }
                    }
                }
                else
                {
                    if (ingredientDatabase.list[i].Type == IngredientItem.type.Shatterable)
                    {
                        if (ingredientDatabase.list[i].NewIngredient)
                        {
                            unlockedItem.Insert(0, ingredientDatabase.list[i]);
                        }
                        else
                        {
                            unlockedItem.Add(ingredientDatabase.list[i]);
                        }
                    }
                }

            }
        }
        for (int i = 0; i < unlockedItem.Count; i++)
        {
            ingredientItemList[i].ingredientItem = unlockedItem[i];
            ingredientItemList[i].gameObject.SetActive(true);
        }
    }

    public void Display(bool show, System.Action callback = null)
    {
        if (show == shown)
            return;
        StartCoroutine(CR_Display(show, callback));
    }


    IEnumerator CR_Display(bool show, System.Action callback)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += displaySpeed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(show ? hidePosition : showPosition, show ? showPosition : hidePosition, t);
            yield return null;
        }
        shown = show;
        callback?.Invoke();
    }
}
