using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public enum ProgressState
{
    NotEnough,
    Early,
    OK,
    Late
}
public class ProgressBar : MonoBehaviour
{
    [SerializeField] Vector2 perfectRange;
    [SerializeField] Vector2 acceptableRange;
    [SerializeField] RectTransform lowestThreshold, perfectThreshold, highestThreshold, lowPerfectLine, highPerfectLine, insideRT;
    [SerializeField] Image coreImage, acceptableZoneImage;
    [SerializeField] float progressValue;
    [SerializeField] Text progressText;
    [SerializeField] Color lowProgressColor;
    [SerializeField] Color acceptableColor;
    [SerializeField] Color overredColor;

    [Header("UI Position")]
    [SerializeField]
    Vector3 hidePosition;
    [SerializeField]
    Vector3 showPosition;
    [SerializeField]
    float displaySpeed;
    RectTransform rectTransform;

    [SerializeField] bool shown;
    [SerializeField] float delayBeforeHide = 0.2f;

    public ProgressState currentState
    {
        get
        {
            if (coreImage.fillAmount < acceptableRange.x)
            {
                return ProgressState.NotEnough;
            }
            else if (coreImage.fillAmount < perfectRange.x)
            {
                return ProgressState.Early;
            }
            else if (coreImage.fillAmount < perfectRange.y)
            {
                return ProgressState.OK;
            }
            else
            {
                return ProgressState.Late;
            }
        }
    }


    private void Start()
    {
        // shown = false;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Display(bool show, System.Action callback = null)
    {
        if (show == shown)
            return;
        StartCoroutine(CR_Display(show, callback));
    }


    IEnumerator CR_Display(bool show, System.Action callback)
    {
        if (!show)
            yield return new WaitForSeconds(delayBeforeHide);
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

    [ContextMenu("SetTargetImage")]
    public void SetTargetImage()
    {
        SetTargetImage(perfectRange, acceptableRange);
    }
    public void SetTargetImage(Vector2 perfectRange, Vector2 acceptableRange)
    {
        this.perfectRange = perfectRange;
        this.acceptableRange = acceptableRange;
        SetRectTransformByPercent(lowestThreshold, acceptableRange.x);
        SetRectTransformByPercent(perfectThreshold, perfectRange.x + (perfectRange.y - perfectRange.x) / 2);
        SetRectTransformByPercent(highestThreshold, acceptableRange.y);
        SetRectTransformByPercent(lowPerfectLine, perfectRange.x);
        SetRectTransformByPercent(highPerfectLine, perfectRange.y);
        acceptableZoneImage.fillAmount = acceptableRange.y - acceptableRange.x;
    }

    [ContextMenu("SetProgress")]
    public void SetProgress()
    {
        SetProgress(progressValue);
    }
    public void SetProgress(float progressValue)
    {
        if (progressValue < perfectRange.x)
        {
            coreImage.color = lowProgressColor;
        }
        if (progressValue > perfectRange.x && progressValue < perfectRange.y)
        {
            coreImage.color = acceptableColor;
            //coreImage.color = Color.Lerp(lowProgressColor, acceptableColor, math.remap(0f, perfectRange.x, 0f, 1f, progressValue));
        }
        if (progressValue >= perfectRange.y)
        {
            coreImage.color = overredColor;
            //coreImage.color = Color.Lerp(acceptableColor, overredColor, math.remap(perfectRange.y, 1f, 0f, 1f, progressValue));

        }

        coreImage.fillAmount = progressValue;
        progressText.text = (coreImage.fillAmount * 100f).ToString("f0") + "%";
    }
    void SetRectTransformByPercent(RectTransform rectTransform, float percent)
    {
        var targetPos = insideRT.anchoredPosition + Vector2.right * (insideRT.rect.width * percent - insideRT.rect.width / 2);
        rectTransform.anchoredPosition = targetPos;
    }



}