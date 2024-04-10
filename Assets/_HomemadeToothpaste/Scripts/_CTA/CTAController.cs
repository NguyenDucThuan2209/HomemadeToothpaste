using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;

public class CTAController : MonoBehaviour
{
    CanvasGroupVisibility canvasGroupVisibility;
    Animator animator;
    [SerializeField] Text CTAText;
    [SerializeField] Text secondCTAText;
    public static CTAController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        canvasGroupVisibility = GetComponent<CanvasGroupVisibility>();
        animator = GetComponent<Animator>();
    }
    public void Show(string content)
    {
        var textLines = content.Split('/');
        CTAText.text = textLines[0].ToUpper();
        secondCTAText.text = textLines.Length > 1 ? textLines[1].ToUpper() : "";
        canvasGroupVisibility.Show();
    }
    public void HideImmediately()
    {
        canvasGroupVisibility.HideImmediately();
    }
}