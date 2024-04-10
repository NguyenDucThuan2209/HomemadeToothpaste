using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StarUI : MonoBehaviour
{
    [SerializeField]
    Image Fill;

    public void FillTheStar(float percent, System.Action callback = null)
    {
        StartCoroutine(CR_Fill(percent, callback));
    }

    IEnumerator CR_Fill(float percent, System.Action callback)
    {
        percent = percent / 100f;
        var t = 0f;
        while (t < percent)
        {

            t += 3f * Time.deltaTime;
            t = Mathf.Clamp01(t);
            Fill.fillAmount = t;
            yield return null;
        }
        callback?.Invoke();
    }

    public void ResetFill()
    {
        Fill.fillAmount = 0f;

    }
}
