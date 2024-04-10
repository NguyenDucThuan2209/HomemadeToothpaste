using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LatteGames;
using System;

public class ToolUnlockUI : MonoBehaviour
{

    public event Action Continue = delegate { };

    [SerializeField]
    Button getItButton;

    public int currentLevelIndex;
    [HideInInspector]
    public float currentValue;
    [HideInInspector]
    public float completeLevelValue;

    [SerializeField]
    float delayBeforeGoNextLevel;
    [SerializeField]
    float displaySpeed;

    [SerializeField]
    float unlockFillSpeed;

    [SerializeField]
    Image progressBarFill;
    [SerializeField]
    TextMeshProUGUI progressBarText;

    [SerializeField]
    ToolDatabase toolDatabase;
    [SerializeField]
    Image toolShadow;
    [SerializeField]
    Image toolOverlay;
    [SerializeField]
    RectTransform shinnyEffectImage;
    [SerializeField]
    float shinnyRotationSpeed;

    public Tools nextUnlockedTool;
    Tools prevUnlockedTool;
    StateGameController stateGameController;
    CustomLevelController customLevelController;
    CanvasGroup ToolUnlockCanvas;
    // Start is called before the first frame update
    private void Awake()
    {
        stateGameController = FindObjectOfType<StateGameController>();
        stateGameController.StateChanged += SetCurrentIndexValue;

        getItButton.onClick.AddListener(() => { SFXManager.Instance.playButtonSFX(); Continue(); });
    }
    void Start()
    {
        shinnyEffectImage.gameObject.SetActive(false);
        ToolUnlockCanvas = GetComponent<CanvasGroup>();
    }


    void SetCurrentIndexValue()
    {
        if (stateGameController.CurrentState == StateGameController.State.Playing)
        {
            currentLevelIndex = stateGameController.playerAchievedLevel.Value + 2; //need to investigate this;
            CalculateFillValue();
        }

    }

    private void OnDestroy()
    {
        stateGameController.StateChanged -= SetCurrentIndexValue;
    }

    // Update is called once per frame
    void Update()
    {
        shinnyEffectImage.localEulerAngles = new Vector3(0f, 0f, Time.time * shinnyRotationSpeed);
    }

    public void CheckNextUnlockedTool()
    {
        //if (currentLevelIndex > toolDatabase.list[toolDatabase.list.Count - 1].unlockAtLevel)
        //{
        //    nextUnlockedTool = toolDatabase.list[toolDatabase.list.Count - 1];
        //    return;
        //}
        if (toolDatabase.list.Count > 0)
        {
            if (currentLevelIndex > toolDatabase.list[toolDatabase.list.Count - 1].unlockAtLevel)
            {
                nextUnlockedTool = null;
                return;
            }
            foreach (var tool in toolDatabase.list)
            {
                if (tool.unlockAtLevel > 0 && currentLevelIndex <= tool.unlockAtLevel)
                {
                    prevUnlockedTool = (toolDatabase.list.IndexOf(tool) - 1 >= 0 && toolDatabase.list.IndexOf(tool) < toolDatabase.list.Count) ? toolDatabase.list[toolDatabase.list.IndexOf(tool) - 1] : null;
                    nextUnlockedTool = tool.HasUnlocked ? null : tool;
                    break;
                }
            }
        }
        else
        {
            prevUnlockedTool = null;
            nextUnlockedTool = null;
        }
    }

    public void SetUnlockedNextTool()
    {
        if (nextUnlockedTool != null && completeLevelValue == 1f)
        {
            nextUnlockedTool.hasUnlocked = true;
            nextUnlockedTool.HasUnlocked = true;
        }
    }

    public void CalculateFillValue() //next unlocked tool progress was calculated by using this method, all we need to do is fill it to 100% if currentLevelIndex == nextUnlockedTool.unlockedAtLevel
    {

        CheckNextUnlockedTool();
        if (nextUnlockedTool != null)
        {
            float preUpperValue = Mathf.Clamp(currentLevelIndex - 1, 0f, 100f) - (prevUnlockedTool != null ? prevUnlockedTool.unlockAtLevel : 0);
            float upperValue = (currentLevelIndex) - (prevUnlockedTool != null ? prevUnlockedTool.unlockAtLevel : 0); // 2 - 0 = 2;
            float lowerValue = nextUnlockedTool.unlockAtLevel - (prevUnlockedTool != null ? prevUnlockedTool.unlockAtLevel : 0); // 2 - 0 = 2
            currentValue = Mathf.Clamp01(preUpperValue / lowerValue); // 2/2 = 1
            completeLevelValue = Mathf.Clamp01(upperValue / lowerValue);
            toolShadow.sprite = nextUnlockedTool.toolShadow;
            toolOverlay.sprite = nextUnlockedTool.toolSprite;
            toolOverlay.fillAmount = currentValue;
            progressBarFill.fillAmount = currentValue;
            progressBarText.text = Mathf.FloorToInt((currentValue * 100)).ToString("f0") + " %"; ;
        }
    }

    public void HideImmediately(System.Action callback = null)
    {
        shinnyEffectImage.gameObject.SetActive(false);
        getItButton.gameObject.SetActive(false);
        ToolUnlockCanvas.alpha = 0f;
        ToolUnlockCanvas.blocksRaycasts = false;
        ToolUnlockCanvas.interactable = false;
        callback?.Invoke();
    }

    public void Display(bool show, System.Action callback = null)
    {
        shinnyEffectImage.gameObject.SetActive(false);
        getItButton.gameObject.SetActive(false);
        StartCoroutine(CR_Display(show, callback));
    }

    public void UnlockFill(System.Action callback = null)
    {
        StartCoroutine(CR_FillTheSprite(() =>
        {
            if (!getItButton.gameObject.activeInHierarchy)
                StartCoroutine("CR_RunDelegate");
        }));
    }

    IEnumerator CR_RunDelegate()
    {
        yield return new WaitForSeconds(delayBeforeGoNextLevel);
        Continue?.Invoke();
    }

    IEnumerator CR_Display(bool show, System.Action callback)
    {
        ToolUnlockCanvas.blocksRaycasts = show;
        ToolUnlockCanvas.interactable = show;
        if (!show && ToolUnlockCanvas.alpha == 0)
        {
            callback?.Invoke();
        }
        else
        {
            var t = 0f;
            while (t < 1f)
            {
                t += displaySpeed * Time.deltaTime;
                ToolUnlockCanvas.alpha = show ? t : 1 - t;
                yield return null;
            }
            callback?.Invoke();
        }
        if (show)
        {
            UnlockFill();
        }
    }

    IEnumerator CR_FillTheSprite(System.Action callback = null)
    {
        var t = currentValue;
        while (t != completeLevelValue)
        {
            t = Mathf.MoveTowards(t, completeLevelValue, unlockFillSpeed * Time.deltaTime);
            //t = Mathf.Clamp(t, 0f, completeLevelValue);
            toolOverlay.fillAmount = t;
            progressBarFill.fillAmount = t;
            progressBarText.text = (t * 100f).ToString("f0") + " %"; ;
            yield return null;
        }
        if (completeLevelValue == 1f)
        {
            shinnyEffectImage.gameObject.SetActive(true);
            getItButton.gameObject.SetActive(true);
            SFXManager.Instance.playUnlockSFX();
        }
        //yield return new WaitForSeconds(0.75f);
        callback?.Invoke();
    }




}
