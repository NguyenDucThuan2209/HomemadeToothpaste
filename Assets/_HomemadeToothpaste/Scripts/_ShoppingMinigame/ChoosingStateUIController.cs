using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Shopping
{
    public class ChoosingStateUIController : MonoBehaviour
    {
        [SerializeField] CanvasGroup textContainer;
        [SerializeField] TextMeshProUGUI correctText;
        [SerializeField] TextMeshProUGUI wrongText;
        [SerializeField] AnimationCurve scaleCurve;
        [SerializeField] AnimationCurve colorCurve;
        [SerializeField] float animationDuration;
        Coroutine textShowingCoroutine;
        private void Awake()
        {
            correctText.gameObject.SetActive(false);
            wrongText.gameObject.SetActive(false);
        }
        public void ShowStateText(bool isCorrect)
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
            correctText.gameObject.SetActive(isCorrect);
            wrongText.gameObject.SetActive(!isCorrect);
        }
    }
}