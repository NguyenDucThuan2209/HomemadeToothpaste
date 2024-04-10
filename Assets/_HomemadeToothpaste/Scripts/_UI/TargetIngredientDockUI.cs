using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;

public class TargetIngredientDockUI : MonoBehaviour
{
    public System.Action OnMeetRequirement;
    [SerializeField]
    List<TargetIngredientUI> targetIconUI;
    public List<IngredientItem> requiredIngredients;
    public List<IngredientItem> ownedIngredients;

    [Header("UI Position")]
    [SerializeField]
    Vector3 hidePosition;
    [SerializeField]
    Vector3 showPosition;
    [SerializeField]
    float displaySpeed;


    bool shown;
    public List<IngredientItem> checkedIngredients;
    RectTransform rectTransform;


    private void Start()
    {
        shown = false;
        checkedIngredients = new List<IngredientItem>();
        rectTransform = GetComponent<RectTransform>();
        LoadDataToIcon();
    }

    private void LoadDataToIcon()
    {
        for (int i = 0; i < requiredIngredients.Count; i++)
        {
            targetIconUI[i].ingredientItem = requiredIngredients[i];
            targetIconUI[i].LoadData();
            targetIconUI[i].gameObject.SetActive(true);
        }
    }

    public void OwnedIngredientItemAdd(IngredientItem item)
    {
        ownedIngredients.Add(item);
        foreach (var i in ownedIngredients)
        {
            if (requiredIngredients.Contains(i) && !checkedIngredients.Contains(i))
            {
                checkedIngredients.Add(i);
                targetIconUI[requiredIngredients.IndexOf(i)].SetCorrect();
            }
        }
        if (checkedIngredients.Count == requiredIngredients.Count)
        {
            OnMeetRequirement?.Invoke();
        }
    }
    public void RequiredIngredientItemAdd(IngredientItem item)
    {
        requiredIngredients.Add(item);
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
