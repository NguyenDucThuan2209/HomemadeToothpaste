using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarBasedTextController : MonoBehaviour
{
    [SerializeField] CanvasGroup textContainer;
    [SerializeField] Text text;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] AnimationCurve colorCurve;
    [SerializeField] float animationDuration;
    [SerializeField] string keepGoingContent = "Keep Going";
    [SerializeField] string tooEarlyContent = "Too Early";
    [SerializeField] string perfectContent = "Perfect!";
    [SerializeField] string tooMuchContent = "Too Much";

    [SerializeField] Color perfectColor;
    [SerializeField] Color badColor;

    [SerializeField] ProgressBar progressBar;
    Coroutine textShowingCoroutine;
    private void Awake()
    {
        text.gameObject.SetActive(false);
    }

    public void ShowStateText()
    {
        if (textShowingCoroutine != null)
        {
            StopCoroutine(textShowingCoroutine);
        }
        textShowingCoroutine = StartCoroutine(CommonCoroutine.LerpFactor(animationDuration, (t) =>
        {
            textContainer.alpha = colorCurve.Evaluate(t);
            textContainer.transform.localScale = Vector3.one * scaleCurve.Evaluate(t);
        }));
        text.gameObject.SetActive(true);
        if (progressBar.currentState == ProgressState.NotEnough)
        {
            text.text = keepGoingContent;
        }
        else if (progressBar.currentState == ProgressState.Early)
        {
            text.text = tooEarlyContent;
            text.color = badColor;
            SFXManager.Instance.playFailSFX();
        }
        else if (progressBar.currentState == ProgressState.OK)
        {
            text.text = perfectContent;
            text.color = perfectColor;
            SFXManager.Instance.playPerfectSFX();
        }
        else if (progressBar.currentState == ProgressState.Late)
        {
            text.text = tooMuchContent;
            text.color = badColor;
            SFXManager.Instance.playFailSFX();
        }
    }
}