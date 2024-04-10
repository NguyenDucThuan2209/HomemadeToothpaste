using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LatteGames;
using System;

public class RatingUI : MonoBehaviour
{
    [HideInInspector]
    public int starsGained;

    public event Action Continue = delegate { };
    [SerializeField]
    Button continueButton;
    [SerializeField]
    float displaySpeed;
    [SerializeField]
    ParticleSystem winParticle;
    [SerializeField]
    List<Image> starFillImages;
    [SerializeField]
    float starFillSpeed;
    [SerializeField]
    TextMeshProUGUI titleText;
    [SerializeField]
    RatingTitle ratingTitle;


    [SerializeField]
    CanvasGroup canvasGroup;


    [Header("TextBox")]
    [SerializeField]
    Color normalTextBoxBGColor;
    [SerializeField]
    Color normalTextBoxShadowColor;

    [SerializeField]
    Color failTextBoxBGColor;
    [SerializeField]
    Color failTextBoxShadowColor;

    [SerializeField]
    Image textBoxBackground;
    [SerializeField]
    Image textBoxShadow;

    private void Awake()
    {
        continueButton.onClick.AddListener(() => { SFXManager.Instance.playButtonSFX(); Continue(); });
    }



    public void Display(bool show, System.Action callback = null)
    {
        StartCoroutine(CR_Display(show, callback));
    }

    IEnumerator CR_Display(bool show, System.Action callback)
    {
        if (show)
        {
            Rating(() => { continueButton.gameObject.SetActive(true); });
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;
        if (!show && canvasGroup.alpha == 0)
        {
            callback?.Invoke();
        }
        else
        {
            var t = 0f;
            while (t < 1f)
            {
                t += displaySpeed * Time.deltaTime;
                canvasGroup.alpha = show ? t : 1 - t;
                yield return null;
            }
            callback?.Invoke();
        }

    }

    public void Reset()
    {
        Continue = null;
        starsGained = 0;
        foreach (var i in starFillImages)
        {
            i.fillAmount = 0f;
        }
    }

    public void Rating(System.Action callback = null)
    {
        switch (starsGained)
        {
            case 0:
                {
                    textBoxBackground.color = failTextBoxBGColor;
                    textBoxShadow.color = failTextBoxShadowColor;
                    titleText.SetText(ratingTitle.failedComment);
                    break;
                }
            case 1:
                {
                    textBoxBackground.color = normalTextBoxBGColor;
                    textBoxShadow.color = normalTextBoxShadowColor;
                    titleText.SetText(ratingTitle.badComment);
                    break;
                }

            case 2:
                {
                    textBoxBackground.color = normalTextBoxBGColor;
                    textBoxShadow.color = normalTextBoxShadowColor;
                    titleText.SetText(ratingTitle.neutralComment);
                    break;
                }
            case 3:
                {
                    textBoxBackground.color = normalTextBoxBGColor;
                    textBoxShadow.color = normalTextBoxShadowColor;
                    titleText.SetText(ratingTitle.goodComment);
                    break;
                }
        }
        StartCoroutine(CR_Rating(callback));
    }



    IEnumerator CR_Rating(System.Action callback)
    {
        for (int i = 0; i < starsGained; i++)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += starFillSpeed * Time.deltaTime;
                t = Mathf.Clamp01(t);
                starFillImages[i].fillAmount = t;
                yield return null;
            }
        }
        CustomLevelController customLevelController = FindObjectOfType<CustomLevelController>();
        if (customLevelController.LevelState == CustomLevelController.State.Win)
        {
            winParticle.Play();
        }
        callback?.Invoke();
    }
}
