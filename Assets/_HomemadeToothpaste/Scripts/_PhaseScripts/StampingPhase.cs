using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using Tabtale.TTPlugins;

public class StampingPhase : PhaseBase
{

    [Header("Phase Individual Attribute")]
    [SerializeField]
    float stopStampingPercent;
    [SerializeField]
    float overStampingPercent;
    [SerializeField]
    MeshFilter powderMeshFilter;
    [SerializeField]
    MeshRenderer ingredientPowder;
    [SerializeField]
    float progressIncreamentOnStampingPowder = 2f;
    [SerializeField]
    Transform ingredientSpawnPoint;
    [SerializeField]
    float stampingProgress;
    [SerializeField]
    ParticleSystem ingredientPowderParticle;
    [SerializeField]
    Vector2 heightRange = new Vector2(-0.1f, 0.1f);
    [SerializeField]
    Vector2 planeLimitedHeight = new Vector2(-0.114f, -0.044f);
    [Header("UI Components")]
    [SerializeField]
    Button finishDroppingButton;
    [SerializeField]
    ProgressBar progressBar;
    //[SerializeField]
    //RectTransform cursor;
    //[SerializeField]
    //RectTransform progressBar;
    //[SerializeField]
    //RectTransform mileStone;
    TargetIngredientDockUI targetIngredientDock;
    IngredientDockUI ingredientDock;


    [Header("Pestle Objects")]
    [SerializeField]
    float pestleControlMagnitude;
    [SerializeField]
    Transform pestle;
    [SerializeField]
    float stampingSpeed;
    [SerializeField]
    Transform pestlePivotPoint;
    [SerializeField]
    Transform pestleStartPoint;
    [SerializeField]
    Transform pestleEndPoint;
    [Header("Mortal Objects")]
    [SerializeField]
    Transform powderParticlePouringPosition;
    [SerializeField]
    public GameObject mortal;
    [SerializeField]
    Transform mortalReadyPoint;
    [SerializeField]
    Transform mortalPouringPoint;
    [SerializeField]
    float mortalRestrictionRadius;

    public Color ingredientPrimaryColor;
    public Color ingredientSecondaryColor;

    float perfectAmount, perfectMin, perfectMax;
    bool stampable, stamped;
    Vector3 orgEndPoint, orgMortalPos, orgPestlePoint;
    private Vector3 prevMousePos, curMousePos, deltaMousePos;
    float tapTimeCounter;
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> orgVerticesPos = new List<Vector3>();
    Mesh powderMesh;

    public float StampingProgress { get => stampingProgress; set { stampingProgress = value; stampingProgress = Mathf.Clamp(stampingProgress, 0f, 100f); } }

    private List<Ingredient> ingredientFragments;

    //how to calculate the progress percent: 
    //ex: we have 6 big chunk (60%) mean each chunk is 10% (40% left is for the powder phase)
    //each chunk can be smashed into 16 small pieces


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void PhaseInitialization()
    {
        //CalculatePercentForUI();
        ingredientFragments = new List<Ingredient>();
        perfectAmount = ((overStampingPercent - stopStampingPercent) / 4f); //ex: (80 - 60) / 4 = 5
        perfectMin = (stopStampingPercent + perfectAmount) / 100f;
        perfectMax = (overStampingPercent - perfectAmount) / 100f;
        progressBar.SetTargetImage(new Vector2(perfectMin, perfectMax), (new Vector2(stopStampingPercent / 100f, overStampingPercent / 100f)));
        ingredientDock = FindObjectOfType<IngredientDockUI>();
        targetIngredientDock = FindObjectOfType<TargetIngredientDockUI>();
        ingredientPowderParticle.gameObject.SetActive(false);
        tapTimeCounter = Time.time;
        orgEndPoint = pestleEndPoint.transform.localPosition;
        orgMortalPos = mortal.transform.position;
        orgPestlePoint = pestle.transform.position;
        stampable = false;
        finishDroppingButton.onClick.AddListener(() => { OnDoneButtonClick(); });
        powderMesh = powderMeshFilter.mesh;
        Vector3[] meshVertices = powderMesh.vertices;
        foreach (var v in meshVertices)
        {
            //Vector3 newPos = gameObject.transform.TransformPoint(v);
            vertices.Add(v);
        }
        foreach (var v in vertices)
        {
            //Vector3 newPos = gameObject.transform.TransformPoint(v);
            orgVerticesPos.Add(v);
        }
        targetIngredientDock.requiredIngredients = GamePhaseManager.Instance.requiredIngredients;
    }

    public override void PhaseProcessing()
    {
        isActive = true;

        //progressBar.anchoredPosition = new Vector3(progressBar.anchoredPosition.x, 200f, 0f);

        CameraController.Instance.MoveToTarget(phaseCameraPoint);
        StartCoroutine("CR_MoveMortalToReadyPosition");
        //show the 2 docks
        ingredientDock.Display(true);
        targetIngredientDock.Display(true);

        //Vector3 pos = new Vector3(ingredientDock.anchoredPosition.x, 175f, 0f);
        //StartCoroutine(CR_MoveUI(pos, 2f, ingredientDock, null));

        //Vector3 pos2 = new Vector3(correctIngredientDock.anchoredPosition.x, -350f, 0f);
        //StartCoroutine(CR_MoveUI(pos2, 2f, correctIngredientDock, null));
    }

    // Update is called once per frame
    void Update()
    {
        if (stampable)
            Stamping();
    }

    //void CalculatePercentForUI()
    //{
    //    float xPos = (progressBar.rect.width - 3f) * (stopStampingPercent / 100f);
    //    float width = (overStampingPercent - stopStampingPercent) * ((progressBar.rect.width - 3f) / 100f);
    //    mileStone.sizeDelta = new Vector2(width, mileStone.rect.height);
    //    mileStone.anchoredPosition = new Vector2(xPos, mileStone.anchoredPosition.y);
    //}

    void Stamping()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tapTimeCounter = Time.time;
            prevMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Time.time - tapTimeCounter > 0.0f)
            {
                HarpticManager.Instance.triggerContinousHarptic = true;
                curMousePos = Input.mousePosition;
                deltaMousePos = curMousePos - prevMousePos;
                float pingPongValue = Mathf.PingPong(stampingSpeed * Time.time, 1f);
                pestle.transform.position = Vector3.Lerp(pestleStartPoint.position, pestleEndPoint.position, pingPongValue);
                if (pingPongValue >= 0.8f)
                {
                    SFXManager.Instance.playStampingSFX();

                }
                pestleEndPoint.transform.localPosition = new Vector3(pestleEndPoint.transform.localPosition.x + deltaMousePos.x * pestleControlMagnitude, pestleEndPoint.transform.localPosition.y, pestleEndPoint.transform.localPosition.z + deltaMousePos.y * pestleControlMagnitude);
                float dist = Vector3.Distance(pestleEndPoint.transform.localPosition, pestlePivotPoint.localPosition);
                //cursor.anchoredPosition = Vector2.MoveTowards(cursor.anchoredPosition, Vector2.Lerp(new Vector2(0, cursor.anchoredPosition.y), new Vector2(progressBar.rect.width - 10f, cursor.anchoredPosition.y), stampingProgress / 100f), 300f * Time.deltaTime);
                //cursor.anchoredPosition = Vector2.Lerp(new Vector2(0, cursor.anchoredPosition.y), new Vector2(progressBar.rect.width, cursor.anchoredPosition.y), stampingProgress / 100f);
                progressBar.SetProgress(stampingProgress / 100f);
                if (dist > mortalRestrictionRadius)
                {
                    Vector3 fromOrigintoObject = pestleEndPoint.transform.localPosition - pestlePivotPoint.localPosition;
                    fromOrigintoObject *= mortalRestrictionRadius / dist;
                    Vector3 pestleLimitRadius = pestlePivotPoint.localPosition + fromOrigintoObject;
                    pestleLimitRadius.y = pestleEndPoint.transform.localPosition.y;
                    pestleEndPoint.transform.localPosition = pestleLimitRadius;
                }

                //prevMousePos = curMousePos;
            }
        }
        else
        {
            HarpticManager.Instance.triggerContinousHarptic = false;
            if (!stamped && StampingProgress > stopStampingPercent)
            {
                progressBar.Display(false);
                stampable = false;
                curMousePos = Vector3.zero;
                prevMousePos = Vector3.zero;
                deltaMousePos = Vector3.zero;
                //Vector3 pos = new Vector3(progressBar.anchoredPosition.x, 200f, 0f);
                //StartCoroutine(CR_MoveUI(pos, 2f, progressBar, null));
                if (stampingProgress < 100f)
                    progressBar.GetComponent<ProgressBarBasedTextController>().ShowStateText();
                StartCoroutine("CR_StopStamping");
            }
            else
            {
                curMousePos = Vector3.zero;
                prevMousePos = Vector3.zero;
                deltaMousePos = Vector3.zero;
                pestle.transform.position = Vector3.MoveTowards(pestle.transform.position, pestleStartPoint.position, 20f * Time.deltaTime);
                pestleEndPoint.transform.localPosition = orgEndPoint;
            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            if (stampingProgress < 100f)
                progressBar.GetComponent<ProgressBarBasedTextController>().ShowStateText();

        }
    }

    public void DropIngredientIntoMortal(IngredientItem item)
    {
        GamePhaseManager.Instance.AddEffect(item);
        finishDroppingButton.interactable = true;
        StartCoroutine(CR_DropIngredient(item));
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
            finishDroppingButton.interactable = false;
            OnDoneButtonClick();
        }
    }

    void ColorMixing(Color addedColor, Color secondaryAddedColor)
    {
        Color newColor = Color.Lerp(ingredientPrimaryColor, addedColor, 0.5f);
        Color newSecondaryColor = Color.Lerp(ingredientSecondaryColor, secondaryAddedColor, 0.5f);
        //GamePhaseManager.Instance.finalResultPrimaryColor = newColor;
        //GamePhaseManager.Instance.finalResultSecondaryColor = newSecondaryColor;
        ingredientPrimaryColor = newColor;
        ingredientSecondaryColor = newSecondaryColor;
    }

    void FinalResultColorMixing(Color addedColor, Color secondaryAddedColor)
    {
        Color newColor = Color.Lerp(GamePhaseManager.Instance.finalResultPrimaryColor, addedColor, 0.8f);
        Color newSecondaryColor = Color.Lerp(GamePhaseManager.Instance.finalResultSecondaryColor, secondaryAddedColor, 0.8f);
        GamePhaseManager.Instance.finalResultPrimaryColor = newColor;
        GamePhaseManager.Instance.finalResultSecondaryColor = newSecondaryColor;
    }

    IEnumerator CR_StopStamping()
    {
        if (stampingProgress / 100f <= perfectMax && stampingProgress / 100f >= perfectMin)
        {
            GamePhaseManager.Instance.FinalEffective += 15f;
            GamePhaseManager.Instance.starsGained += 1;
        }

        GamePhaseManager.Instance.CTAController.HideImmediately();
        GamePhaseManager.Instance.infinityAnimationController.Hide();
        GamePhaseManager.Instance.CheckForCorrectIngredient();
        MixingPhase mixingPhase = (MixingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.SodaMix);
        /*        foreach (var go in GameObject.FindGameObjectsWithTag("Spider"))
                {
                    go.GetComponent<Rigidbody>().isKinematic = true;
                }*/
        //FlatteningThePowder();
        //ingredientPowderParticle.transform.position = mortal.transform.TransformPoint(powderParticlePouringPosition.position);
        ParticleSystem.MainModule main = ingredientPowderParticle.main;
        ParticleSystem.MinMaxGradient startColor = main.startColor;
        startColor.colorMin = ingredientPrimaryColor;
        startColor.colorMax = ingredientSecondaryColor;
        main.startColor = startColor;
        // move the pestle out of camera view and move the mortal to pouring position
        mixingPhase.MoveBowlToReadyPosition();
        foreach (var o in FindObjectsOfType<Ingredient>())
        {
            Destroy(o.gameObject);
        }
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            mortal.transform.position = Vector3.Lerp(mortal.transform.position, mortalPouringPoint.position, t);
            mortal.transform.rotation = Quaternion.Lerp(Quaternion.identity, mortalPouringPoint.rotation, t);
            pestle.transform.position = Vector3.Lerp(pestleStartPoint.position, orgPestlePoint, t);
            yield return null;
        }
        pestle.gameObject.SetActive(false);
        mixingPhase.SetIngredientPowderColor(ingredientPrimaryColor);

        //if (droppedIngredient.Count == 1 && droppedIngredient[0].ingredientName == "SPIDER") //only 1 ingredient dropped and it a spider ingredient
        //{
        //    //enable spider parts
        //    foreach (var go in GameObject.FindGameObjectsWithTag("Spider"))
        //    {
        //        go.transform.parent = GamePhaseManager.Instance.mixingPhase.mixture.transform;
        //        go.GetComponent<Rigidbody>().isKinematic = false;
        //    }
        //}
        //else // more than 1 ingredient and it could be contains a spider ingredient
        //{
        //    ingredientPowderParticle.gameObject.SetActive(true);
        //    GamePhaseManager.Instance.mixingPhase.PouringIngredient();
        //    if (droppedIngredient.Find(x => x.ingredientName == "SPIDER"))
        //    {
        //        // enable spider parts
        //        foreach (var go in GameObject.FindGameObjectsWithTag("Spider"))
        //        {
        //            go.transform.parent = GamePhaseManager.Instance.mixingPhase.mixture.transform;
        //            go.GetComponent<Rigidbody>().isKinematic = false;

        //        }
        //    }
        //}

        ingredientPowderParticle.gameObject.SetActive(true);
        mixingPhase.PouringIngredient();
        ingredientPowder.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            mortal.transform.position = Vector3.Lerp(mortalPouringPoint.position, orgMortalPos, t);
            mortal.transform.rotation = Quaternion.Lerp(mortalPouringPoint.rotation, Quaternion.identity, t);
            yield return null;
        }
        FinalResultColorMixing(ingredientPrimaryColor, ingredientSecondaryColor);
        GamePhaseManager.Instance.GoToNextPhase();
    }

    IEnumerator CR_DropIngredient(IngredientItem item)
    {
        if (item.isMultipleFragmentShape)
        {
            for (int i = 0; i < item.spawnAmount; i++)
            {
                int rand = Random.Range(0, item.ingredientChunks.Count);
                GameObject inst = Instantiate(item.ingredientChunks[rand]);
                inst.transform.position = new Vector3(ingredientSpawnPoint.position.x + Random.Range(-0.1f, 0.1f), ingredientSpawnPoint.position.y, ingredientSpawnPoint.position.z + Random.Range(-0.1f, 0.1f));
                inst.GetComponent<Rigidbody>().isKinematic = false;
                inst.transform.parent = GamePhaseManager.Instance.customLevelController.transform;
                yield return null;
            }
        }
        else
        {
            for (int i = 0; i < item.spawnAmount; i++)
            {
                GameObject inst = Instantiate(item.ingredientChunks[0]);
                inst.transform.position = new Vector3(ingredientSpawnPoint.position.x + Random.Range(-0.1f, 0.1f), ingredientSpawnPoint.position.y, ingredientSpawnPoint.position.z + Random.Range(-0.1f, 0.1f));
                inst.GetComponent<Rigidbody>().isKinematic = false;
                inst.transform.parent = GamePhaseManager.Instance.customLevelController.transform;
                yield return null;
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

    IEnumerator CR_MoveMortalToReadyPosition()
    {
        mortal.SetActive(true);
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            mortal.transform.position = Vector3.Lerp(orgMortalPos, mortalReadyPoint.position, t);
            yield return null;
        }
    }

    IEnumerator CR_MovePestleToReadyPosition()
    {
        pestle.gameObject.SetActive(true);
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            pestle.transform.position = Vector3.Lerp(orgPestlePoint, pestleStartPoint.position, t);
            yield return null;
        }
        stampable = true;
    }

    void OnDoneButtonClick()
    {
        //if (droppedIngredient.Count == 1 && droppedIngredient[0].ingredientName == "Chocolate")
        //{
        //    //GamePhaseManager.Instance.stickedOnTeeth = true;
        //    GamePhaseManager.Instance.finalEffective = 0f;
        //}

        GamePhaseManager.Instance.CalculateEffectiveness();
        //StartCoroutine(CR_MoveUI(new Vector3(ingredientDock.anchoredPosition.x, -500, 0f), 2f, ingredientDock));
        //StartCoroutine(CR_MoveUI(new Vector3(progressBar.anchoredPosition.x, -300f, 0f), 2f, progressBar));
        //Vector3 pos2 = new Vector3(correctIngredientDock.anchoredPosition.x, 500f, 0f);
        //StartCoroutine(CR_MoveUI(pos2, 2f, correctIngredientDock, null));
        progressBar.Display(true);
        targetIngredientDock.Display(false);
        ingredientDock.Display(false);
        StartCoroutine("CR_MovePestleToReadyPosition");
        GamePhaseManager.Instance.infinityAnimationController.Show();
        foreach (var i in FindObjectsOfType<Ingredient>())
        {
            ingredientFragments.Add(i);
        }
        int countIngredientFragments = ingredientFragments.Count;
        foreach (var item in FindObjectsOfType<Ingredient>())
        {
            item.percentIncreament = 75f / countIngredientFragments;
        }
        GamePhaseManager.Instance.CTAController.Show("DRAG TO STAMP");

        //StartCoroutine(CR_CheckAndBalanceIngredient(() =>
        //{

        //}));
    }

    public void RemoveFromFragmentList(Ingredient ingredient)
    {
        if (ingredientFragments.Contains(ingredient))
        {
            ingredientFragments.Remove(ingredient);
        }
    }

    IEnumerator CR_CheckAndBalanceIngredient(System.Action callback) //hard code version of balancing
    {
        if (GamePhaseManager.Instance.droppedIngredients.Count == 1)
        {
            for (var i = 0; i < 3; i++)
            {
                DropIngredientIntoMortal(GamePhaseManager.Instance.droppedIngredients[0]);
                yield return null;
            }
        }
        if (GamePhaseManager.Instance.droppedIngredients.Count == 2)
        {
            DropIngredientIntoMortal(GamePhaseManager.Instance.droppedIngredients[0]);
            yield return null;
            DropIngredientIntoMortal(GamePhaseManager.Instance.droppedIngredients[1]);
            yield return null;

        }
        callback?.Invoke();
    }

    public void Powderization(bool movePlaneUp, float moveUpAmount = 0.1f)
    {
        //if (ingredientFragments.Count < 3)
        //StampingProgress += progressIncreamentOnStampingPowder;
        if (stampingProgress > 65f)
            stampingProgress += progressIncreamentOnStampingPowder;
        Color newColor = ingredientPowder.material.color;
        newColor.r = ingredientPrimaryColor.r;
        newColor.g = ingredientPrimaryColor.g;
        newColor.b = ingredientPrimaryColor.b;
        newColor.a -= 0.01f * Time.deltaTime;
        Color secondaryMapColor = ingredientPowder.material.GetColor("_SecondaryMapColor");
        secondaryMapColor.r = newColor.r;
        secondaryMapColor.g = newColor.g;
        secondaryMapColor.b = newColor.b;
        ingredientPowder.material.SetColor("_Color", newColor);
        ingredientPowder.material.SetColor("_SecondaryMapColor", secondaryMapColor);
        //if (movePlaneUp)
        //{
        //    //ingredientPowder.transform.localPosition = new Vector3(ingredientPowder.transform.localPosition.x, Mathf.Clamp(ingredientPowder.transform.position.y + moveUpAmount * Time.deltaTime, planeLimitedHeight.x, planeLimitedHeight.y), ingredientPowder.transform.localPosition.z);
        //}
        if (movePlaneUp)
        {
            ingredientPowder.transform.localPosition = Vector3.MoveTowards(ingredientPowder.transform.localPosition, new Vector3(ingredientPowder.transform.localPosition.x, Mathf.Clamp(ingredientPowder.transform.localPosition.y, planeLimitedHeight.x, planeLimitedHeight.y), ingredientPowder.transform.localPosition.z), 10f * Time.deltaTime);
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            if (Vector3.Distance(vertices[i], ingredientPowder.transform.InverseTransformPoint(pestle.transform.position)) < 0.2f)
            {
                vertices[i] = new Vector3(vertices[i].x, movePlaneUp ? vertices[i].y - 0.02f * Time.deltaTime : vertices[i].y - 0.01f * Time.deltaTime, vertices[i].z);
            }
            else
            {
                float newY = Mathf.Clamp(vertices[i].y + (0.006f / Vector3.Distance(vertices[i], ingredientPowder.transform.InverseTransformPoint(pestle.transform.position))) * Time.deltaTime, heightRange.x, heightRange.y);
                vertices[i] = new Vector3(vertices[i].x, newY, vertices[i].z);
            }
        }
        powderMesh.vertices = vertices.ToArray();
        powderMesh.RecalculateBounds();
        powderMesh.RecalculateNormals();
        //if (ingredientPowder.transform.localScale != Vector3.one)
        //{
        //    ingredientPowder.transform.localScale = Vector3.MoveTowards(ingredientPowder.transform.localScale, Vector3.one, 5f * Time.deltaTime);
        //}
        //else
        //{
        //    float currentWeight = ingredientPowder.GetBlendShapeWeight(0);
        //    ingredientPowder.SetBlendShapeWeight(0, currentWeight + (40f * Time.deltaTime));
        //    ingredientPowder.SetBlendShapeWeight(0, Mathf.Clamp(ingredientPowder.GetBlendShapeWeight(0), 0f, 100f));
        //}
    }

    void FlatteningThePowder()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = orgVerticesPos[i];
        }
        powderMesh.vertices = vertices.ToArray();
        powderMesh.RecalculateBounds();
        powderMesh.RecalculateNormals();
    }


}
