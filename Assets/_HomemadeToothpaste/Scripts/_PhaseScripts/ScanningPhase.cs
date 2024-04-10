using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class ScanningPhase : PhaseBase
{
    [Header("Phase Individual Attribute")]
    [SerializeField]
    float goNextPhaseDelay = 1f;
    [SerializeField]
    Material upperJawMaterial;
    [SerializeField]
    Material lowerJawMaterial;
    [SerializeField]
    Material whiteTeethMaterial;
    [SerializeField]
    Material dirtMaterial;
    [SerializeField]
    Material bloodMaterial;



    [Header("UI Components")]
    [SerializeField]
    CanvasGroup progressBarCanvasGroup;
    [SerializeField]
    Image barFill;
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    CanvasGroup buttonCanvasGroup;
    [SerializeField]
    Button nextPhaseButton;

    [Header("Scanner Tool")]
    [SerializeField]
    GameObject scanner;
    [SerializeField]
    Transform scannerTarget;

    [Header("Teeth Rendering")]
    [SerializeField]
    GameObject incompleteRenderingMouth;
    [SerializeField]
    GameObject completeRenderingMouth;
    [SerializeField]
    float teethRenderMagnitude = 0.1f;
    [SerializeField]
    GameObject monitorScreen;

    Vector3 currentMousePos, prevMousePos, scannerOriginalPos;

    bool scannable;
    bool clicked;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        scannable = false;
        //CalculatePercentForUI();
    }

    public override void PhaseProcessing()
    {

        isActive = true;
        CameraController.Instance.MoveToTarget(phaseCameraPoint, () =>
        {
            StartCoroutine("CR_SetScannable");
        });

    }

    IEnumerator CR_SetScannable()
    {
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        GamePhaseManager.Instance.CTAController.Show("DRAG RIGHT / TO SCAN");
        yield return new WaitForSeconds(0.5f);
        scannable = true;
    }

    public override void PhaseInitialization()
    {
        GameObject dissolveMouth = GameObject.Find("teeth_dissolve Variant");
        upperJawMaterial = dissolveMouth.GetComponent<Mouth>().upperJaw.material;
        lowerJawMaterial = dissolveMouth.GetComponent<Mouth>().lowerJaw.material;
        //for (int i = 0; i < completeRenderingMouth.transform.childCount; i++)
        //{
        //    completeRenderingMouth.transform.GetChild(i).gameObject.SetActive(false);
        //}
        upperJawMaterial.SetFloat("_DissolveScale", 1f);
        bloodMaterial.SetFloat("_DissolveScale", 1f);
        dirtMaterial.SetFloat("_DissolveScale", 1f);
        whiteTeethMaterial.SetFloat("_DissolveScale", 1f);
        text.text = ((int)barFill.fillAmount * 100f).ToString() + "%";
        scannerOriginalPos = scanner.transform.position;
        //DisplayButton(false);
        //nextPhaseButton.onClick.AddListener(() => { NextPhaseButtonClick(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (scannable)
        {
            InputChecker();
            if (!completeRenderingMouth.activeInHierarchy && upperJawMaterial.GetFloat("_DissolveScale") == 0f)
            {
                GamePhaseManager.Instance.CTAController.HideImmediately();
                GamePhaseManager.Instance.infinityAnimationController.Hide();
                completeRenderingMouth.gameObject.SetActive(true);
                incompleteRenderingMouth.SetActive(false);
                scanner.SetActive(false);
                SFXManager.Instance.playScannedSFX();
                StartCoroutine("CR_GoNextPhase");
                //DisplayButton(true);
                //GamePhaseManager.Instance.femaleCharacter.MouthMotion(false);
                HarpticManager.Instance.triggerContinousHarptic = false;


            }
        }

    }


    IEnumerator CR_GoNextPhase()
    {
        yield return new WaitForSeconds(goNextPhaseDelay);
        GamePhaseManager.Instance.GoToNextPhase();
        progressBarCanvasGroup.alpha = 0f;
    }

    public void DeactiveScreenMonitor()
    {
        monitorScreen.gameObject.SetActive(false);
    }

    public void NextPhaseButtonClick()
    {
        if (!clicked)
        {
            clicked = true;
            GamePhaseManager.Instance.GoToNextPhase();
            DisplayButton(false);
            progressBarCanvasGroup.alpha = 0f;

        }
    }

    void DisplayButton(bool show)
    {
        StartCoroutine(CR_DisplayButton(show));
    }

    IEnumerator CR_DisplayButton(bool show)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            t = Mathf.Clamp01(t);
            buttonCanvasGroup.alpha = show ? t : 1 - t;
            yield return null;
        }
        buttonCanvasGroup.interactable = show;
        buttonCanvasGroup.blocksRaycasts = show;
    }

    void TeethRendering()
    {
        float dissolveProgress = upperJawMaterial.GetFloat("_DissolveScale");
        if (currentMousePos.x > prevMousePos.x && dissolveProgress > 0f && (currentMousePos.x - prevMousePos.x < 100f))
        {
            HarpticManager.Instance.triggerContinousHarptic = true;
            dissolveProgress -= (currentMousePos.x - prevMousePos.x) * teethRenderMagnitude;
            upperJawMaterial.SetFloat("_DissolveScale", Mathf.Clamp01(dissolveProgress));
            lowerJawMaterial.SetFloat("_DissolveScale", Mathf.Clamp01(dissolveProgress));
            bloodMaterial.SetFloat("_DissolveScale", Mathf.Clamp01(dissolveProgress));
            dirtMaterial.SetFloat("_DissolveScale", Mathf.Clamp01(dissolveProgress));
            whiteTeethMaterial.SetFloat("_DissolveScale", Mathf.Clamp01(dissolveProgress));
            barFill.fillAmount = 1f - (dissolveProgress * 1f);
            scanner.transform.position = Vector3.Lerp(scannerOriginalPos, scannerTarget.position, barFill.fillAmount);
            text.text = ((int)(barFill.fillAmount * 100f)).ToString() + "%";
        }
    }


    void InputChecker()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {

            currentMousePos = Input.mousePosition;

            TeethRendering();
            prevMousePos = currentMousePos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            HarpticManager.Instance.triggerContinousHarptic = false;
            currentMousePos = Vector3.zero;
            prevMousePos = Vector3.zero;
        }
    }


}
