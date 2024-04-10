using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class BlenderPhase : PhaseBase
{
    [Header("Phase Individual Attribute")]
    [SerializeField]
    MeshRenderer liquid;
    [SerializeField]
    GameObject blender;
    [SerializeField]
    GameObject blenderTop;
    [SerializeField]
    GameObject blenderBot;
    [SerializeField]
    GameObject blenderCap;
    [SerializeField]
    Collider blenderBlade;
    [SerializeField]
    Transform blenderReadyPosition;
    [SerializeField]
    Transform blenderPouringPosition;
    [SerializeField]
    Transform blenderCapOpenPosition;
    [SerializeField]
    ParticleSystem blendingParticle;
    [SerializeField]
    Transform blendingCameraPosition;
    [SerializeField]
    Transform blenderBotRemovePosition;
    [SerializeField]
    Transform blenderTopRemovePosition;
    [SerializeField]
    ParticleSystem pouringParticle;
    [SerializeField]
    Transform ingredientSpawnPoint;
    [Header("UI Attribute")]
    [SerializeField]
    Button finishDropButton;
    TargetIngredientDockUI targetIngredientDock;
    IngredientDockUI ingredientDock;
    [SerializeField] float progressIncreasingValuePerFrame = 0.01f;
    [SerializeField] AnimationCurve liquidLengthCurve;
    [SerializeField] AnimationCurve blendCurve;
    [SerializeField] ParticleSystem waterDropFX;
    [SerializeField] ProgressBar progressBar;

    public Color ingredientPrimaryColor;
    public Color ingredientSecondaryColor;

    List<IngredientItem> droppedIngredient = new List<IngredientItem>();
    public List<IngredientItem> DroppedIngredient { get => droppedIngredient; set => droppedIngredient = value; }

    Vector3 orgBlenderPos, orgBlenderCapPos, orgBlenderTopPos, orgBlenderBotPos;

    bool isAutoBalancing;
    bool blended;

    public override void Start()
    {
        base.Start();
    }

    public override void PhaseInitialization()
    {
        orgBlenderBotPos = blenderBot.transform.localPosition;
        orgBlenderPos = blender.transform.position;
        orgBlenderCapPos = blenderCap.transform.localPosition;
        orgBlenderTopPos = blenderTop.transform.localPosition;
        liquid.material.SetFloat("_HeightMax", 2f);
        finishDropButton.onClick.AddListener(() => OnDoneButtonClick());
        ingredientDock = FindObjectOfType<IngredientDockUI>();
        targetIngredientDock = FindObjectOfType<TargetIngredientDockUI>();
        targetIngredientDock.requiredIngredients = GamePhaseManager.Instance.requiredIngredients;
    }

    public override void PhaseProcessing()
    {
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        blender.gameObject.SetActive(true);
        isActive = true;
        CameraController.Instance.MoveToTarget(phaseCameraPoint);
        StartCoroutine("CR_MoveBlenderToReadyPosition");
        ingredientDock.Display(true);
        targetIngredientDock.Display(true);
    }

    public void DropIngredientIntoBlender(IngredientItem item)
    {
        GamePhaseManager.Instance.AddEffect(item);
        finishDropButton.interactable = true;
        StartCoroutine(CR_DropIngredient(item.spawnAmount, item.ingredientChunks));
        if (GamePhaseManager.Instance.droppedIngredients.Count == 0)
        {
            ingredientPrimaryColor = item.ingredientColor;
            ingredientSecondaryColor = item.ingredientSecondaryColor;
        }
        else
        {
            ColorMixing(item.ingredientColor, item.ingredientSecondaryColor);
        }
        targetIngredientDock.OwnedIngredientItemAdd(item);
        GamePhaseManager.Instance.AddIngredient(item);
        if (GamePhaseManager.Instance.droppedIngredients.Count == 3)
        {
            Debug.LogError("OnDoneButtonClick");
            finishDropButton.interactable = false;
            OnDoneButtonClick();
        }
    }

    void ColorMixing(Color addedColor, Color secondaryAddedColor)
    {
        Color newColor = Color.Lerp(ingredientPrimaryColor, addedColor, 0.5f);
        Color newSecondaryColor = Color.Lerp(ingredientSecondaryColor, secondaryAddedColor, 0.5f);
        GamePhaseManager.Instance.finalResultPrimaryColor = newColor;
        GamePhaseManager.Instance.finalResultSecondaryColor = newSecondaryColor;
        ingredientPrimaryColor = newColor;
        ingredientSecondaryColor = newSecondaryColor;
    }

    IEnumerator CR_MoveBlenderToReadyPosition()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blenderCap.transform.localPosition = Vector3.Lerp(orgBlenderCapPos, blenderCapOpenPosition.localPosition, t);
            blenderCap.transform.rotation = Quaternion.Lerp(Quaternion.identity, blenderCapOpenPosition.rotation, t);
            blender.transform.position = Vector3.Lerp(orgBlenderPos, blenderReadyPosition.position, t);
            yield return null;
        }
    }

    IEnumerator CR_DropIngredient(int spawnAmount, List<GameObject> ingredientChunks)
    {
        if (ingredientChunks.Count > 1)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                int randomChunkIndex = Random.Range(0, ingredientChunks.Count);
                GameObject inst = Instantiate(ingredientChunks[randomChunkIndex]);
                inst.transform.SetParent(transform.root);
                inst.transform.position = new Vector3(ingredientSpawnPoint.position.x + Random.Range(-0.01f, 0.01f), ingredientSpawnPoint.position.y, ingredientSpawnPoint.position.z + Random.Range(-0.01f, 0.01f));
                inst.GetComponent<Rigidbody>().isKinematic = false;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                int randomChunkIndex = 0;
                GameObject inst = Instantiate(ingredientChunks[randomChunkIndex]);
                inst.transform.SetParent(transform.root);
                inst.transform.position = new Vector3(ingredientSpawnPoint.position.x + Random.Range(-0.01f, 0.01f), ingredientSpawnPoint.position.y, ingredientSpawnPoint.position.z + Random.Range(-0.01f, 0.01f));
                inst.GetComponent<Rigidbody>().isKinematic = false;
                yield return new WaitForSeconds(0.05f);
            }
        }

    }

    IEnumerator CR_MoveUI(Vector3 targetPosition, float speed, RectTransform rect, System.Action callback = null)
    {
        var t = 0f;
        Vector3 orgAnchorPoint = rect.anchoredPosition;
        while (t < 1f)
        {
            t += speed * Time.deltaTime;
            rect.anchoredPosition = Vector3.Lerp(orgAnchorPoint, targetPosition, t);
            yield return null;
        }
        callback?.Invoke();
    }

    void OnDoneButtonClick()
    {
        //if (droppedIngredient.Count == 1 && droppedIngredient[0].ingredientName == "Chocolate")
        //{
        //    GamePhaseManager.Instance.stickyChocolate = true;
        //    GamePhaseManager.Instance.finalEffective = 0f;
        //}

        GamePhaseManager.Instance.CalculateEffectiveness();
        liquid.material.SetColor("_TopColor", ingredientSecondaryColor);
        liquid.material.SetColor("_BottomColor", ingredientSecondaryColor);
        liquid.material.SetColor("_SurfaceColor", ingredientPrimaryColor);
        var waterDropFXMain = waterDropFX.main;
        waterDropFXMain.startColor = ingredientPrimaryColor;
        ParticleSystem.MainModule main = pouringParticle.main;
        ParticleSystem.MinMaxGradient startColor = main.startColor;
        startColor = ingredientPrimaryColor;
        main.startColor = startColor;
        targetIngredientDock.Display(false);
        ingredientDock.Display(false, () =>
        {
            GamePhaseManager.Instance.CTAController.Show("PRESS & HOLD / TO BLEND");
        });
        StartCoroutine("CR_StartMixing");

        //StartCoroutine(CR_CheckAndBalanceIngredient(() =>
        //{


        //}));

    }

    IEnumerator CR_CheckAndBalanceIngredient(System.Action callback) //hard code version of balancing
    {
        if (GamePhaseManager.Instance.droppedIngredients.Count == 1)
        {
            isAutoBalancing = true;
            for (var i = 0; i < 3; i++)
            {
                DropIngredientIntoBlender(GamePhaseManager.Instance.droppedIngredients[0]);
                yield return null;
            }
        }
        if (GamePhaseManager.Instance.droppedIngredients.Count == 2)
        {
            isAutoBalancing = true;
            DropIngredientIntoBlender(GamePhaseManager.Instance.droppedIngredients[0]);
            yield return null;
            DropIngredientIntoBlender(GamePhaseManager.Instance.droppedIngredients[1]);
            yield return null;

        }
        callback?.Invoke();
    }

    IEnumerator CR_StartMixing()
    {
        progressBar.Display(true);
        var progressBarBasedTextController = progressBar.GetComponent<ProgressBarBasedTextController>();
        CameraController.Instance.MoveToTarget(blendingCameraPosition);
        var t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            blenderCap.transform.localPosition = Vector3.Lerp(blenderCapOpenPosition.localPosition, orgBlenderCapPos, t);
            blenderCap.transform.rotation = Quaternion.Lerp(blenderCapOpenPosition.rotation, Quaternion.identity, t);
            yield return null;
        }
        float fillAmount = 2f;
        float waveSpeed = 0f;
        float waveLength = 0f;

        //blendingParticle.gameObject.SetActive(true);
        t = Time.time + 0.8f;
        float startTime = Time.time;
        // while (Time.time < t) {
        //     //t += Time.deltaTime;
        //     waveSpeed = 5f;
        //     waveLength = 10f;
        //     blenderBlade.transform.Rotate (Vector3.up * 500f * Time.deltaTime, Space.Self);
        //     if (Time.time > startTime + 0.1f) {
        //         blenderBlade.isTrigger = false;
        //     }
        //     fillAmount = Mathf.MoveTowards (fillAmount, 2.42f, 0.8f * Time.deltaTime);
        //     liquid.material.SetFloat ("_HeightMax", fillAmount);
        //     liquid.material.SetFloat ("_WaveSpeed", waveSpeed);
        //     liquid.material.SetFloat ("_WaveLength", waveLength);
        //     yield return null;
        // }
        List<Ingredient> ingredients = new List<Ingredient>();
        foreach (var i in FindObjectsOfType<Ingredient>())
        {
            ingredients.Add(i);
        }
        float progress = 0;
        float targetBlendingState = 0;
        float currentBlendingState = 0;
        float blendingSpeed = 0.07f;
        var startWaterDropFXPos = waterDropFX.transform.position;
        var targetWaterDropFXPos = startWaterDropFXPos + Vector3.up * 0.25f;
        SFXManager.Instance.playBlenderSFX();
        while (progress < 1)
        {
            if (Input.GetMouseButton(0))
            {
                if (targetBlendingState == 0)
                {
                    targetBlendingState = 1;
                    GamePhaseManager.Instance.CTAController.HideImmediately();
                    var emission = waterDropFX.emission;
                    emission.enabled = true;
                    HarpticManager.Instance.triggerContinousHarptic = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (targetBlendingState == 1)
                {
                    blended = true;
                    targetBlendingState = 0;
                    var emission = waterDropFX.emission;
                    emission.enabled = false;
                    HarpticManager.Instance.triggerContinousHarptic = false;
                    if (progressBar.currentState != ProgressState.NotEnough)
                    {
                        progressBarBasedTextController.ShowStateText();

                        break;
                    }
                }
            }
            var oldCurrentBlendingState = currentBlendingState;
            currentBlendingState = Mathf.MoveTowards(currentBlendingState, targetBlendingState, blendingSpeed);
            if (oldCurrentBlendingState > 0 && currentBlendingState <= 0)
            {
                progressBarBasedTextController.ShowStateText();
            }
            blenderBlade.transform.Rotate(Vector3.up * 50000f * Time.deltaTime * currentBlendingState, Space.Self);
            // liquid.material.SetFloat ("_WaveSpeed", waveSpeed * targetBlendingState);
            SFXManager.Instance.SetBlenderSFXVolume(currentBlendingState);
            liquid.material.SetFloat("_WaveLength", waveLength * liquidLengthCurve.Evaluate(currentBlendingState));
            liquid.material.SetFloat("_T", liquid.material.GetFloat("_T") + 2 * Time.deltaTime * currentBlendingState);
            foreach (var ingredient in ingredients)
            {
                ingredient.BounceWhenCollideWithBlade(blenderBlade.transform.position, 0.15f, Random.Range(1, 3) * currentBlendingState);
            }
            progress += progressIncreasingValuePerFrame * currentBlendingState;
            progressBar.SetProgress(progress);
            liquid.material.SetFloat("_HeightMax", Mathf.Lerp(2.2f, 2.6f, progress));
            liquid.material.SetFloat("_BlendValue", blendCurve.Evaluate(progress));
            waterDropFX.transform.position = Vector3.Lerp(startWaterDropFXPos, targetWaterDropFXPos, progress);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        SFXManager.Instance.stopBlenderSFX();
        var emissionFX = waterDropFX.emission;
        emissionFX.enabled = false;
        liquid.material.SetFloat("_WaveSpeed", 0f);
        liquid.material.SetFloat("_WaveLength", 0f);
        if (progressBar.currentState == ProgressState.OK)
        {
            GamePhaseManager.Instance.FinalEffective += 15f;
            GamePhaseManager.Instance.starsGained += 1;
        }
        progressBar.Display(false);
        // blenderBlade.isTrigger = true;
        foreach (var i in ingredients)
        {
            Destroy(i.gameObject);
        }
        StartCoroutine("CR_RemoveBlenderBot");
        MixingPhase mixingPhase = (MixingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.SodaMix);
        mixingPhase.MoveBowlToReadyPosition();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine("CR_MoveBlenderToPourPosition");
        yield return new WaitForSeconds(0.5f);
        HarpticManager.Instance.triggerContinousHarptic = false;
        mixingPhase.SetIngredientPowderColor(ingredientPrimaryColor);
        GamePhaseManager.Instance.CheckForCorrectIngredient();
        StartCoroutine(CR_Pouring(() =>
        {

            //CameraController.Instance.MoveToTarget(phaseCameraPoint);
            FinalResultColorMixing(ingredientPrimaryColor, ingredientSecondaryColor);
            GamePhaseManager.Instance.GoToNextPhase();

        }));

    }

    void FinalResultColorMixing(Color addedColor, Color secondaryAddedColor)
    {
        Color newColor = Color.Lerp(GamePhaseManager.Instance.finalResultPrimaryColor, addedColor, 0.8f);
        Color newSecondaryColor = Color.Lerp(GamePhaseManager.Instance.finalResultSecondaryColor, secondaryAddedColor, 0.8f);
        GamePhaseManager.Instance.finalResultPrimaryColor = newColor;
        GamePhaseManager.Instance.finalResultSecondaryColor = newSecondaryColor;
    }

    IEnumerator CR_MoveBlenderToPourPosition()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blenderTop.transform.localPosition = Vector3.Lerp(orgBlenderTopPos, blenderPouringPosition.localPosition, t);
            //blenderTop.transform.localRotation = Quaternion.Lerp(Quaternion.identity, blenderPouringPosition.localRotation, t);
            yield return null;
        }
    }

    IEnumerator CR_Pouring(System.Action callback)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += 0.5f * Time.deltaTime;
            blenderTop.transform.localRotation = Quaternion.Lerp(Quaternion.identity, blenderPouringPosition.localRotation, t);
            if (t > 0.8f)
            {
                pouringParticle.gameObject.SetActive(true);
                pouringParticle.Play();
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        MixingPhase mixingPhase = (MixingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.SodaMix);
        mixingPhase.PouringIngredient();
        t = 0f;
        SFXManager.Instance.playLiquidSFX();
        while (t < 1f)
        {
            t += 0.8f * Time.deltaTime;
            liquid.material.SetFloat("_HeightMax", Mathf.Lerp(2.42f, 2f, t));
            if (t > 0.6f)
            {
                pouringParticle.Stop();

            }
            yield return null;
        }

        StartCoroutine("CR_RemoveBlenderTop");
        callback?.Invoke();
    }

    IEnumerator CR_RemoveBlenderTop()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blenderTop.transform.localPosition = Vector3.Lerp(blenderPouringPosition.localPosition, blenderTopRemovePosition.localPosition, t);
            blenderTop.transform.localRotation = Quaternion.Lerp(blenderPouringPosition.localRotation, Quaternion.identity, t);
            yield return null;
        }
        pouringParticle.gameObject.SetActive(false);
        blender.gameObject.SetActive(false);
    }

    IEnumerator CR_RemoveBlenderBot()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blenderBot.transform.localPosition = Vector3.Lerp(orgBlenderBotPos, blenderBotRemovePosition.localPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

    }
}