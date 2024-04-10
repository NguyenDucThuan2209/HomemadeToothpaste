using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingPhase : PhaseBase
{
    [Header("Phase Individual Attribute")]
    [SerializeField]
    bool bakingSodaPoured;
    [SerializeField]
    float stopPouringPercent;
    [SerializeField]
    float overPouredPercent;
    [SerializeField]
    SkinnedMeshRenderer bakingSodaPowder;
    [SerializeField]
    GameObject bakingSodaBox;
    [SerializeField]
    float bakingSodaBoxRotationSpeed;
    [SerializeField]
    Transform bakingSodaPourPosition;
    [SerializeField]
    ParticleSystem bakingSodaParticle;
    [SerializeField]
    Transform particleSpawnPoint;
    [SerializeField]
    bool coconutCreamScooped;
    [SerializeField]
    GameObject coconutCreamJar;
    [SerializeField]
    GameObject coconutCreamLid;
    [SerializeField]
    Transform cocounutCreamScoopPosition;
    [SerializeField]
    Transform lidOpenedPosition;
    [SerializeField]
    GameObject stick;
    [SerializeField]
    Transform stickReadyPoint;
    [SerializeField]
    Transform stickScoopingPoint;
    [SerializeField]
    Transform creamScoopingPoint;
    [SerializeField]
    AnimationCurve stickScoopingCurve;
    [SerializeField]
    GameObject creamObject;
    [SerializeField]
    GameObject spoon;
    public SkinnedMeshRenderer mixture;
    [SerializeField]
    Transform spoonReadyPosition;
    [SerializeField]
    Transform spoonStirPosition;
    [SerializeField]
    float mixtureStiringSpeed;
    [SerializeField]
    GameObject bowl;
    [SerializeField]
    Transform bowlWaitingPosition;
    [SerializeField]
    bool mixed;
    [SerializeField]
    SkinnedMeshRenderer ingredientProduct;
    [Header("UI Components")]
    //[SerializeField]
    //RectTransform cursor;
    //[SerializeField]
    //RectTransform progressBar;
    //[SerializeField]
    //RectTransform mileStone;
    [SerializeField]
    ProgressBar progressBar;
    [SerializeField]
    ProgressBar progressBarFill;


    float perfectAmount, perfectMin, perfectMax;
    bool pourableSoda, scoopableCream, scooped, mixable, secondMixed;
    float tapTimeCounter, pouringSodaProgress, scoopingProgress, stiringProgress, mixingProgress, secondMixingProgress;
    ParticleSystem bakingSodaParticleClone;
    Vector3 orgBakingSodaBoxPos, orgCoconutCreamPos, orgCoconutCreamLidPos, orgSpoonPos, orgBowlPos, orgIngredientProductPos, orgIngredientProductScale;
    Quaternion orgSpoonRotation;
    float bakingSodaAmount;
    Vector3 currentMousePos, prevMousePos, deltaMousePos;
    float mixtureColorLerpValue;

    public override void PhaseProcessing()
    {
        if (!mixed)
        {

            CameraController.Instance.MoveToTarget(phaseCameraPoint,
             () =>
             {
                 ScanningPhase scanningPhase = (ScanningPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.Scan);
                 scanningPhase.DeactiveScreenMonitor();
                 StartCoroutine(CR_MoveBakingSodaBoxToPourPosition(true));
             });
        }
        else
        {
            CameraController.Instance.MoveToTarget(phaseCameraPoint,
              () =>
              {
                  StartCoroutine("CR_MoveSpoonToReadyPosition");
                  mixable = true;
              });


        }

    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

    }

    IEnumerator CR_MoveSodaBoxToTable()
    {
        bakingSodaParticleClone.Stop();
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            Mathf.Clamp01(t);
            bakingSodaBox.transform.position = Vector3.Lerp(bakingSodaPourPosition.position, orgBakingSodaBoxPos, t);
            bakingSodaBox.transform.rotation = Quaternion.Lerp(bakingSodaBox.transform.rotation, Quaternion.Euler(0f, 180f, 0f), t);
            yield return null;
        }
        StartCoroutine("CR_MoveCoconutCreamToScoopPosition");
    }

    IEnumerator CR_MoveBakingSodaBoxToPourPosition(bool pour)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            Mathf.Clamp01(t);
            if (pour)
                bakingSodaBox.transform.position = Vector3.Lerp(orgBakingSodaBoxPos, bakingSodaPourPosition.position, t);
            else
                bakingSodaBox.transform.position = Vector3.Lerp(bakingSodaPourPosition.position, orgBakingSodaBoxPos, t);

            yield return null;
        }
        //Vector3 pos = new Vector3(progressBar.anchoredPosition.x, -300f, 0f);
        //StartCoroutine(CR_MoveUI(pos, 2f, progressBar, () =>
        //{
        //    pourableSoda = true; GamePhaseManager.Instance.infinityAnimationController.Hide();

        //    GamePhaseManager.Instance.CTAController.Show("TAP & HOLD/TO POUR");
        //}));
        progressBar.Display(true, () =>
        {
            pourableSoda = true; GamePhaseManager.Instance.infinityAnimationController.Hide();
            GamePhaseManager.Instance.CTAController.Show("TAP & HOLD/TO POUR");
        });
    }

    void PouringBakingSoda()
    {
        bakingSodaParticleClone.transform.position = particleSpawnPoint.position;
        bakingSodaAmount -= 40f * Time.deltaTime;
        bakingSodaAmount = Mathf.Clamp(bakingSodaAmount, 0f, 100f);
        bakingSodaParticleClone.Play();
        pouringSodaProgress += 40f * Time.deltaTime;
        pouringSodaProgress = Mathf.Clamp(pouringSodaProgress, 0f, 100f);
        if (bakingSodaPowder.transform.localScale != Vector3.one)
        {
            bakingSodaPowder.transform.position = new Vector3(bakingSodaPowder.transform.position.x, bakingSodaPowder.transform.position.y + 0.001f, bakingSodaPowder.transform.position.z);
            bakingSodaPowder.transform.localScale = Vector3.MoveTowards(bakingSodaPowder.transform.localScale, Vector3.one, 1.2f * Time.deltaTime);
        }
        if (bakingSodaPowder.GetBlendShapeWeight(0) < 100f)
        {
            float currentWeight = bakingSodaPowder.GetBlendShapeWeight(0);
            bakingSodaPowder.SetBlendShapeWeight(0, currentWeight + (40f * Time.deltaTime));
            bakingSodaPowder.SetBlendShapeWeight(0, Mathf.Clamp(bakingSodaPowder.GetBlendShapeWeight(0), 0f, 100f));
        }
    }


    void PourBakingSodaControl()
    {
        if (pourableSoda && !bakingSodaPoured)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tapTimeCounter = Time.time;

            }
            if (Input.GetMouseButton(0) && bakingSodaAmount > 0f)
            {
                if (Time.time - tapTimeCounter > 0.0f)
                {

                    HarpticManager.Instance.triggerContinousHarptic = true;
                    bakingSodaBox.transform.rotation = Quaternion.RotateTowards(bakingSodaBox.transform.rotation, bakingSodaPourPosition.rotation, bakingSodaBoxRotationSpeed * Time.deltaTime);
                    if (bakingSodaBox.transform.rotation.Equals(bakingSodaPourPosition.rotation))
                    {
                        PouringBakingSoda();
                        //cursor.anchoredPosition = Vector2.MoveTowards(cursor.anchoredPosition, Vector2.Lerp(new Vector2(0, cursor.anchoredPosition.y), new Vector2(progressBar.rect.width - 20f, cursor.anchoredPosition.y), pouringSodaProgress / 100f), 00f * Time.deltaTime);
                        //cursor.anchoredPosition = Vector2.Lerp(new Vector2(0, cursor.anchoredPosition.y), new Vector2(progressBar.rect.width, cursor.anchoredPosition.y), pouringSodaProgress / 100f);
                        progressBar.SetProgress(pouringSodaProgress / 100f);
                    }

                }
            }
            else
            {
                HarpticManager.Instance.triggerContinousHarptic = false;
                if (!bakingSodaPoured && pouringSodaProgress > stopPouringPercent)
                {
                    bakingSodaPoured = true;
                    GamePhaseManager.Instance.CTAController.HideImmediately();
                    GamePhaseManager.Instance.infinityAnimationController.Hide();
                    if (pouringSodaProgress / 100f <= perfectMax && pouringSodaProgress / 100f > perfectMin)
                    {
                        GamePhaseManager.Instance.FinalEffective += 15f;// 1 stars
                        GamePhaseManager.Instance.starsGained += 1;
                    }
                    StartCoroutine("CR_MoveSodaBoxToTable");
                    //Vector3 pos = new Vector3(progressBar.anchoredPosition.x, 200f, 0f);
                    //StartCoroutine(CR_MoveUI(pos, 2f, progressBar, null));
                    progressBar.Display(false);
                }
                else
                {
                    bakingSodaBox.transform.rotation = Quaternion.RotateTowards(bakingSodaBox.transform.rotation, Quaternion.Euler(0f, 180f, 0f), bakingSodaBoxRotationSpeed * Time.deltaTime);
                    bakingSodaParticleClone.Stop();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                progressBar.GetComponent<ProgressBarBasedTextController>().ShowStateText();
            }
        }
    }

    IEnumerator CR_MoveCoconutCreamToTable()
    {
        stick.gameObject.SetActive(false);
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            Mathf.Clamp01(t);
            coconutCreamJar.transform.position = Vector3.Lerp(cocounutCreamScoopPosition.position, orgCoconutCreamPos, t);
            coconutCreamJar.transform.rotation = Quaternion.Lerp(cocounutCreamScoopPosition.rotation, Quaternion.identity, t);
            coconutCreamLid.transform.localPosition = Vector3.Lerp(lidOpenedPosition.localPosition, orgCoconutCreamLidPos, t);
            coconutCreamLid.transform.localRotation = Quaternion.Lerp(coconutCreamLid.transform.localRotation, Quaternion.identity, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("CR_MoveSpoonToReadyPosition");
    }

    IEnumerator CR_MoveCoconutCreamToScoopPosition()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            Mathf.Clamp01(t);
            coconutCreamJar.transform.position = Vector3.Lerp(orgCoconutCreamPos, cocounutCreamScoopPosition.position, t);
            coconutCreamJar.transform.rotation = Quaternion.Lerp(Quaternion.identity, cocounutCreamScoopPosition.rotation, t);
            coconutCreamLid.transform.localPosition = Vector3.Lerp(orgCoconutCreamLidPos, lidOpenedPosition.localPosition, t);
            coconutCreamLid.transform.localRotation = Quaternion.Lerp(Quaternion.identity, lidOpenedPosition.localRotation, t);
            yield return null;
        }
        stick.gameObject.SetActive(true);
        t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            Mathf.Clamp01(t);
            stick.transform.position = Vector3.Lerp(stick.transform.position, stickReadyPoint.position, t);
            yield return null;
        }

        scoopableCream = true;
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        GamePhaseManager.Instance.CTAController.Show("DRAG DOWN/TO SCOOP");
    }

    void ScoopingCream()
    {
        stick.transform.position = Vector3.Lerp(stickReadyPoint.position, stickScoopingPoint.position, scoopingProgress);
        if (scoopingProgress == 1f && !scooped)
        {
            scooped = true;
            StartCoroutine("CR_StickScoopingCream");
        }
    }

    IEnumerator CR_StickScoopingCream()
    {
        var t = 0f;
        Quaternion stickOrgRotation = stick.transform.localRotation;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            t = Mathf.Clamp01(t);
            stick.transform.rotation = Quaternion.Lerp(stickOrgRotation, stickScoopingPoint.rotation, stickScoopingCurve.Evaluate(t));
            if (t > 0.5f)
                creamObject.SetActive(true);
            yield return null;
        }
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            t = Mathf.Clamp01(t);
            stick.transform.rotation = Quaternion.Lerp(stickOrgRotation, stickScoopingPoint.rotation, stickScoopingCurve.Evaluate(1 - t));
            stick.transform.position = Vector3.Lerp(stickScoopingPoint.position, creamScoopingPoint.position, t);
            yield return null;
        }
        creamObject.transform.parent = transform.parent;
        creamObject.GetComponent<Rigidbody>().isKinematic = false;
        GamePhaseManager.Instance.CTAController.HideImmediately();
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        yield return new WaitForSeconds(1f);
        StartCoroutine("CR_MoveCoconutCreamToTable");
    }

    void ScoopingCreamControl()
    {
        if (scoopableCream && scoopingProgress < 1f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prevMousePos = Input.mousePosition;

            }
            if (Input.GetMouseButton(0))
            {
                currentMousePos = Input.mousePosition;
                if (currentMousePos.y < prevMousePos.y)
                {
                    scoopingProgress += Mathf.Abs(currentMousePos.y - prevMousePos.y) * 0.03f;
                    scoopingProgress = Mathf.Clamp01(scoopingProgress);
                    ScoopingCream();
                }
                prevMousePos = currentMousePos;
            }
            if (Input.GetMouseButtonUp(0))
            {
                currentMousePos = Vector3.zero;
                prevMousePos = Vector3.zero;
            }
        }

    }

    IEnumerator CR_MoveSpoonToReadyPosition()
    {
        progressBarFill.SetProgress(0f);
        progressBarFill.Display(true);
        spoon.gameObject.SetActive(true);
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            spoon.transform.position = Vector3.Lerp(orgSpoonPos, spoonReadyPosition.position, t);
            spoon.transform.rotation = Quaternion.Lerp(orgSpoonRotation, spoonReadyPosition.rotation, t);
            yield return null;
        }
        mixable = true;
        GamePhaseManager.Instance.infinityAnimationController.Show();
        GamePhaseManager.Instance.CTAController.Show("DRAG TO/MIX UP");
    }

    void SpoonStiring()
    {
        mixture.transform.Rotate(Vector3.up * mixtureStiringSpeed * Time.deltaTime, Space.Self);

        if (mixture.transform.localScale != Vector3.one)
        {
            mixture.transform.localScale = Vector3.MoveTowards(mixture.transform.localScale, Vector3.one, 6f * Time.deltaTime);
        }
        else
        {
            float mixtureCurrentWeight = mixture.GetBlendShapeWeight(0);
            mixture.SetBlendShapeWeight(0, mixtureCurrentWeight + (80f * Time.deltaTime));
            mixture.SetBlendShapeWeight(0, Mathf.Clamp(mixture.GetBlendShapeWeight(0), 0f, 100f));
        }
        Color color = bakingSodaPowder.material.color;
        color.a -= 0.6f * Time.deltaTime;
        color.a = Mathf.Clamp01(color.a);
        bakingSodaPowder.material.SetColor("_Color", color);
        if (bakingSodaPowder.GetBlendShapeWeight(0) > 0)
        {
            float currentWeight = bakingSodaPowder.GetBlendShapeWeight(0);
            bakingSodaPowder.SetBlendShapeWeight(0, currentWeight - (80f * Time.deltaTime));
            bakingSodaPowder.SetBlendShapeWeight(0, Mathf.Clamp(bakingSodaPowder.GetBlendShapeWeight(0), 0f, 100f));

        }
        else
        {
            //bakingSodaPowder.gameObject.SetActive(false);
            bakingSodaPowder.transform.localScale = Vector3.MoveTowards(bakingSodaPowder.transform.localScale, Vector3.zero, 6f * Time.deltaTime);
            if (bakingSodaPowder.transform.localScale != Vector3.zero)
            {
                bakingSodaPowder.transform.position = new Vector3(bakingSodaPowder.transform.position.x, bakingSodaPowder.transform.position.y - 0.0015f, bakingSodaPowder.transform.position.z);
            }
        }


        if (creamObject.transform.localScale != Vector3.zero)
        {
            creamObject.transform.localScale = Vector3.MoveTowards(creamObject.transform.localScale, Vector3.zero, 1.6f * Time.deltaTime);

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

    void SpoonStiringControl()
    {
        if (mixable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prevMousePos = Input.mousePosition;

            }
            if (Input.GetMouseButton(0))
            {
                currentMousePos = Input.mousePosition;
                deltaMousePos = currentMousePos - prevMousePos;
                if (currentMousePos.x != prevMousePos.x && currentMousePos.y != prevMousePos.y)
                {

                    stiringProgress += 15f * Time.deltaTime;
                    mixingProgress = Mathf.Clamp(stiringProgress * 3f, 0f, 100f);
                    //Debug.Log(Mathf.Sin(stiringProgress));
                    progressBarFill.SetProgress(mixingProgress / 100f);
                    spoon.transform.position = new Vector3(spoonReadyPosition.position.x + 0.12f * Mathf.Sin(stiringProgress), spoon.transform.position.y, spoonReadyPosition.position.z + 0.12f * Mathf.Cos(stiringProgress));
                    if (!mixed)
                        SpoonStiring();
                    else
                        SpoonStiringWithIngredient();

                    if (Mathf.Sin(stiringProgress) > 0.99f)
                    {
                        SFXManager.Instance.playSpoonSFX();
                    }
                }
                prevMousePos = currentMousePos;
            }
            if (Input.GetMouseButtonUp(0))
            {
                currentMousePos = Vector3.zero;
                prevMousePos = Vector3.zero;
            }
            if (mixingProgress >= 100f && !mixed)
            {
                StartCoroutine("CR_FirstMixing");
            }
            if (secondMixingProgress >= 100f && !secondMixed)
            {
                StartCoroutine("CR_SecondMixing");

            }
        }
    }

    IEnumerator CR_FirstMixing()
    {

        GamePhaseManager.Instance.CTAController.HideImmediately();
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        mixed = true;
        creamObject.SetActive(false);
        StartCoroutine("CR_MoveSpoonBack");
        StartCoroutine("CR_NormalizeThePowder");

        mixable = false;
        currentMousePos = Vector3.zero;
        prevMousePos = Vector3.zero;
        yield return new WaitForSeconds(0.8f);
        progressBarFill.Display(false);
        StartCoroutine("CR_MoveBowlToWaitPosition");
        GamePhaseManager.Instance.GoToNextPhase();
    }

    IEnumerator CR_SecondMixing()
    {
        progressBarFill.Display(false);
        GamePhaseManager.Instance.CTAController.HideImmediately();
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        secondMixed = true;
        StartCoroutine("CR_MoveSpoonBack");
        mixable = false;
        currentMousePos = Vector3.zero;
        prevMousePos = Vector3.zero;
        yield return new WaitForSeconds(0.5f);

        GamePhaseManager.Instance.GoToNextPhase();
    }

    void SpoonStiringWithIngredient()
    {
        secondMixingProgress += 40f * Time.deltaTime;
        secondMixingProgress = Mathf.Clamp(secondMixingProgress, 0f, 100f);
        progressBarFill.SetProgress(secondMixingProgress / 100f);
        if (GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend).GetType() == typeof(StampingPhase))
        {
            if (ingredientProduct.GetBlendShapeWeight(0) > 0)
            {
                float currentWeight = ingredientProduct.GetBlendShapeWeight(0);
                ingredientProduct.SetBlendShapeWeight(0, currentWeight - (50f * Time.deltaTime));
                ingredientProduct.SetBlendShapeWeight(0, Mathf.Clamp(ingredientProduct.GetBlendShapeWeight(0), 0f, 100f));

            }
            else
            {
                if (ingredientProduct.transform.localScale != Vector3.zero)
                {
                    ingredientProduct.transform.localScale = Vector3.MoveTowards(ingredientProduct.transform.localScale, Vector3.zero, 4f * Time.deltaTime);
                }
                else
                {
                    ingredientProduct.gameObject.SetActive(false);

                }
            }
        }
        else
        {
            if (ingredientProduct.transform.localScale != Vector3.zero)
            {
                ingredientProduct.transform.localScale = Vector3.MoveTowards(ingredientProduct.transform.localScale, Vector3.zero, 0.2f * Time.deltaTime);
                //Vector3 ingredientProductPos = ingredientProduct.transform.localPosition;
                //ingredientProductPos.y -= 0.05f * Time.deltaTime;
                //ingredientProduct.transform.localPosition = ingredientProductPos;
                ingredientProduct.transform.localPosition = Vector3.MoveTowards(ingredientProduct.transform.localPosition, orgIngredientProductPos, 0.2f * Time.deltaTime);
            }
            else
            {
                ingredientProduct.gameObject.SetActive(false);

            }
        }
        mixture.transform.Rotate(Vector3.up * mixtureStiringSpeed * Time.deltaTime, Space.Self);
        float mixtureCurrentWeight = mixture.GetBlendShapeWeight(0);
        mixture.SetBlendShapeWeight(0, mixtureCurrentWeight + (40f * Time.deltaTime));
        mixture.SetBlendShapeWeight(0, Mathf.Clamp(mixture.GetBlendShapeWeight(0), 0f, 100f));
        mixtureColorLerpValue = Mathf.MoveTowards(mixtureColorLerpValue, 1f, Time.deltaTime);
        Color targetColor = mixture.material.GetColor("_TargetColor");
        targetColor = Color.Lerp(Color.white, GamePhaseManager.Instance.finalResultPrimaryColor, mixtureColorLerpValue);
        targetColor.a = Mathf.MoveTowards(targetColor.a, 1f, Time.deltaTime);
        Color currentColor = mixture.material.GetColor("_Color");
        currentColor.a = Mathf.MoveTowards(currentColor.a, 0f, Time.deltaTime);
        mixture.material.SetColor("_Color", currentColor);
        mixture.material.SetColor("_TargetColor", targetColor);
        mixture.material.SetColor("_SecondaryColor", GamePhaseManager.Instance.finalResultSecondaryColor);
        if (GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend).GetType() == typeof(StampingPhase))
        {
            StampingPhase stampingPhase = (StampingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend);
            mixture.material.SetFloat("_BlackThreshold", stampingPhase.StampingProgress / 150f);
        }
        //else
        //{
        //    //BlenderPhase blendingPhase = (BlenderPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend);
        //    //mixture.material.SetFloat("_BlackThreshold", blendingPhase.pro / 150f);
        //}



    }

    IEnumerator CR_MoveSpoonBack()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            spoon.transform.position = Vector3.Lerp(spoonReadyPosition.position, orgSpoonPos, t);
            yield return null;
        }
        spoon.SetActive(false);
    }

    IEnumerator CR_NormalizeThePowder()
    {
        var t = mixture.GetBlendShapeWeight(0) / 100f;
        while (t > 0.5f)
        {
            t -= Time.deltaTime;
            mixture.SetBlendShapeWeight(0, t * 100f);
            yield return null;
        }
        bakingSodaPowder.gameObject.SetActive(false);
    }

    IEnumerator CR_MoveBowlToWaitPosition()
    {
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            bowl.transform.position = Vector3.Lerp(orgBowlPos, bowlWaitingPosition.position, t);
            yield return null;
        }
        bowl.gameObject.SetActive(false);
    }

    public void MoveBowlToReadyPosition()
    {
        StartCoroutine("CR_MoveBowlToReadyPosition");
    }

    IEnumerator CR_MoveBowlToReadyPosition()
    {
        bowl.gameObject.SetActive(true);

        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            bowl.transform.position = Vector3.Lerp(bowlWaitingPosition.position, orgBowlPos, t);
            yield return null;
        }
    }

    public void PouringIngredient()
    {
        StartCoroutine("CR_PouringIngredient");
    }

    IEnumerator CR_PouringIngredient()
    {
        ingredientProduct.gameObject.SetActive(true);
        var t = 0f;
        if (GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend).GetType() == typeof(StampingPhase))
        {
            while (t < 1f)
            {
                t += Time.deltaTime;
                ingredientProduct.SetBlendShapeWeight(0, t * 100f);
                yield return null;
            }
        }
        else
        {
            Vector3 targetLiquidPos = ingredientProduct.transform.localPosition;
            targetLiquidPos.y += 0.024f;
            Vector3 targetScale = new Vector3(0.45f, 0.001f, 0.45f);
            while (t < 1f)
            {
                t += Time.deltaTime;
                ingredientProduct.transform.localPosition = Vector3.Lerp(orgIngredientProductPos, targetLiquidPos, t);
                ingredientProduct.transform.localScale = Vector3.Lerp(orgIngredientProductScale, targetScale, t);
                yield return null;
            }
        }

    }

    public void SetIngredientPowderColor(Color color)
    {
        if (GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend).GetType() == typeof(StampingPhase))
        {
            color.a = 0.5f;
            ingredientProduct.material.color = color;
            ingredientProduct.material.SetColor("_SecondaryMapColor", new Color(color.r, color.g, color.b, 1f));

            Debug.Log("stamping phase");
        }
        else
        {
            ingredientProduct.material.color = color;
        }

    }

    // Update is called once per frame
    void Update()
    {
        PourBakingSodaControl();
        ScoopingCreamControl();
        SpoonStiringControl();
    }

    public override void PhaseInitialization()
    {
        spoon.gameObject.SetActive(false);
        stick.gameObject.SetActive(false);
        bakingSodaPowder.transform.localScale = Vector3.zero;
        orgSpoonRotation = spoon.transform.rotation;
        bakingSodaAmount = 100f;
        tapTimeCounter = Time.time;
        orgBowlPos = bowl.transform.position;
        orgCoconutCreamPos = coconutCreamJar.transform.position;
        orgCoconutCreamLidPos = coconutCreamLid.transform.localPosition;
        orgSpoonPos = spoon.transform.position;
        orgBakingSodaBoxPos = bakingSodaBox.transform.position;
        GameObject instParticle = Instantiate(bakingSodaParticle.gameObject);
        instParticle.transform.position = particleSpawnPoint.position;
        instParticle.transform.parent = transform.parent;
        bakingSodaParticleClone = instParticle.GetComponent<ParticleSystem>();
        bakingSodaParticleClone.Stop();
        ParticleSystem.MainModule main = bakingSodaParticleClone.main;
        ParticleSystem.MinMaxGradient startColor = main.startColor;
        startColor.colorMin = Color.white;
        startColor.colorMax = new Color32(217, 217, 217, 255);
        main.startColor = startColor;
        //cursor.anchoredPosition = new Vector2(0, cursor.anchoredPosition.y);
        orgIngredientProductPos = ingredientProduct.transform.localPosition;
        orgIngredientProductScale = ingredientProduct.transform.localScale;
        perfectAmount = ((overPouredPercent - stopPouringPercent) / 4f); //ex: (80 - 60) / 4 = 5
        perfectMin = (stopPouringPercent + perfectAmount) / 100f;
        perfectMax = (overPouredPercent - perfectAmount) / 100f;
        progressBar.SetTargetImage(new Vector2(perfectMin, perfectMax), (new Vector2(stopPouringPercent / 100f, overPouredPercent / 100f)));
        //CalculatePercentForUI();
    }
}
