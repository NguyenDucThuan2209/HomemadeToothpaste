using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushingPhase : PhaseBase
{
    [Header("Phase Individual Attribute")]
    public Vector2 randomBrushingRange = new Vector2(3, 5);
    [SerializeField]
    List<ParticleSystem> brushParticle;
    [SerializeField]
    Transform toothBrushVFXPosition;
    [SerializeField]
    GameObject toothBrush;
    [SerializeField]
    Transform GetToothpastePoint;
    [SerializeField]
    Transform toothBrushReadyPoint;
    [SerializeField]
    Vector2 toothBrushMinClamp;
    [SerializeField]
    Vector2 toothBrushMaxClamp;
    [SerializeField]
    float controlSensititvity;
    [SerializeField]
    float brushSpeed;
    [SerializeField]
    MeshRenderer toothPaste;
    [SerializeField]
    List<MeshRenderer> activeTeethDirts;
    [SerializeField]
    List<SkinnedMeshRenderer> activeBloodyGums;
    public bool brushing;

    [Header("UI Attribute")]
    [SerializeField]
    ProgressBar progressBar;

    float brushingProgress;
    bool brushable, brushed;
    Vector3 orgToothBrushPos;
    private Vector3 prevMousePos, curMousePos, deltaMousePos;
    float totalBrushingRequired, brushedTimes;

    // Start is called before the first frame update
    public override void Start()
    {
        brushedTimes = 0;
        foreach (var d in GamePhaseManager.Instance.femaleCharacter.mouth.dirts)
        {
            activeTeethDirts.Add(d.GetComponent<MeshRenderer>());
        }

        foreach (var b in GamePhaseManager.Instance.femaleCharacter.mouth.bloodyGums)
        {
            activeBloodyGums.Add(b.GetComponent<SkinnedMeshRenderer>());
        }



        foreach (var go in GameObject.FindGameObjectsWithTag("BrushVFX"))
        {
            ParticleSystem p = go.GetComponent<ParticleSystem>();
            brushParticle.Add(p);
            p.gameObject.SetActive(false);
        }

        foreach (var go in FindObjectsOfType<BrushVFXTrigger>())
        {
            totalBrushingRequired += go.brushedTimes;
        }
        totalBrushingRequired = Mathf.RoundToInt(totalBrushingRequired);
        base.Start();
    }

    public override void PhaseProcessing()
    {

        foreach (var p in brushParticle)
        {
            ParticleSystem.MainModule main = p.main;
            ParticleSystem.MinMaxGradient startColor = main.startColor;
            startColor.colorMin = Color.Lerp(Color.white, GamePhaseManager.Instance.finalResultPrimaryColor, 0.8f);
            startColor.colorMax = Color.Lerp(new Color(97f / 255f, 97f / 255f, 97f / 255f), GamePhaseManager.Instance.finalResultSecondaryColor, 0.8f);
            main.startColor = startColor;
        }


        MixingPhase mixingPhase = (MixingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.IngredientMix);

        toothPaste.material = mixingPhase.mixture.material;
        GamePhaseManager.Instance.femaleCharacter.Sit();
        GamePhaseManager.Instance.femaleCharacter.MouthMotion(true);
        StartCoroutine(CR_GetToothPasteAndMoveToReadyPosition(() =>
        {
            CameraController.Instance.MoveToTarget(phaseCameraPoint,
            () =>
            {
                StartCoroutine("CR_Brushable");
            });


        }));

    }

    public void IncreaseBrushedTime(float increment)
    {
        brushedTimes += increment;
    }

    IEnumerator CR_Brushable()
    {
        yield return new WaitForSeconds(0.5f);
        progressBar.Display(true, () =>
        {
            brushable = true;
            GamePhaseManager.Instance.CTAController.Show("DRAG TO BRUSH");
            GamePhaseManager.Instance.infinityAnimationController.Show();

        });

    }

    public override void PhaseInitialization()
    {
        orgToothBrushPos = toothBrush.transform.position;

    }
    // Update is called once per frame
    void Update()
    {
        if (brushable)
        {
            if (Input.GetMouseButtonDown(0))
            {

                prevMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                curMousePos = Input.mousePosition;
                deltaMousePos = curMousePos - prevMousePos;
                Vector3 newPos = new Vector3(toothBrush.transform.position.x + (deltaMousePos.x) * controlSensititvity, toothBrush.transform.position.y + (deltaMousePos.y) * controlSensititvity, toothBrush.transform.position.z);
                newPos.x = Mathf.Clamp(newPos.x, toothBrushMinClamp.x, toothBrushMaxClamp.x);
                newPos.y = Mathf.Clamp(newPos.y, toothBrushMinClamp.y, toothBrushMaxClamp.y);

                if (deltaMousePos != Vector3.zero)
                {
                    if (!brushed)
                    {
                        HarpticManager.Instance.triggerContinousHarptic = true;
                        brushing = true;
                        SFXManager.Instance.playBrushingSFX();
                        toothPaste.transform.localScale = Vector3.MoveTowards(toothPaste.transform.localScale, Vector3.zero, 0.1f * Time.deltaTime);
                        //brushingProgress = Mathf.MoveTowards(brushingProgress, 100f, 30f * Time.deltaTime);
                        // brushingProgress = (((float)brushParticle.FindAll(x => x.gameObject.activeInHierarchy).Count / (float)brushParticle.Count)) * 100f;
                        brushingProgress = Mathf.Clamp(brushedTimes / totalBrushingRequired, 0f, 100f) * 100f;
                        progressBar.SetProgress(brushingProgress / 100f);
                    }
                }
                else
                {
                    brushing = false;
                    HarpticManager.Instance.triggerContinousHarptic = false;
                }
                toothBrush.transform.position = Vector3.MoveTowards(toothBrush.transform.position, newPos, brushSpeed * Time.deltaTime);
                prevMousePos = curMousePos;
            }
            if (Input.GetMouseButtonUp(0))
            {
                curMousePos = Vector3.zero;
                prevMousePos = Vector3.zero;
                deltaMousePos = Vector3.zero;
                if (brushingProgress >= 100f && !brushed)
                {
                    GamePhaseManager.Instance.CTAController.HideImmediately();
                    GamePhaseManager.Instance.infinityAnimationController.Hide();

                    brushed = true;
                    StartCoroutine("CR_StopBrushing");
                }
                HarpticManager.Instance.triggerContinousHarptic = false;
            }
        }
    }


    public void DisableAllFoam()
    {
        foreach (var p in brushParticle)
        {
            p.gameObject.SetActive(false);
        }
    }

    public void CleanTheDirts()
    {
        foreach (var m in activeTeethDirts)
        {
            //Color newColor = m.material.color;
            //newColor.a = 1 - (GamePhaseManager.Instance.FinalEffective / 100f);
            //m.material.color = newColor;

            float Cleanliness = (GamePhaseManager.Instance.FinalEffective / 100f);
            m.material.SetFloat("_Cleanliness", Cleanliness);
            //if(m.material.color.a == 0f)
            //{
            //    m.gameObject.SetActive(false);
            //}
        }
        float jawCleanliness = (GamePhaseManager.Instance.FinalEffective / 100f);
        GamePhaseManager.Instance.femaleCharacter.mouth.upperJaw.material.SetFloat("_Cleanliness", jawCleanliness);
        GamePhaseManager.Instance.femaleCharacter.mouth.lowerJaw.material.SetFloat("_Cleanliness", jawCleanliness);
    }

    IEnumerator CR_StopBrushing()
    {

        progressBar.Display(false);
        GamePhaseManager.Instance.femaleCharacter.mouth.DisableAllParticles();
        //if (GamePhaseManager.Instance.toothpasteEffects.Contains(IngredientItem.Effect.RemoveYellowTeeth))
        //{
        //    foreach (var m in activeTeethDirts)
        //    {
        //        Color newColor = m.material.color;
        //        newColor.a = 1 - (GamePhaseManager.Instance.finalEffective / 100f);
        //        m.material.color = newColor;
        //    }
        //}
        //if (GamePhaseManager.Instance.toothpasteEffects.Contains(IngredientItem.Effect.RemoveBloodyGum))
        //{
        //    foreach (var b in activeBloodyGums)
        //    {
        //        float newWeight = b.GetBlendShapeWeight(0);

        //        //b.SetBlendShapeWeight(2, newWeight);
        //        newWeight = GamePhaseManager.Instance.finalEffective / 100f;
        //        b.SetBlendShapeWeight(0, newWeight);
        //        b.SetBlendShapeWeight(1, newWeight);
        //        Color newColor = b.material.color;
        //        newColor.a = 1 - (GamePhaseManager.Instance.finalEffective / 100f);
        //        b.material.color = newColor;
        //    }
        //}
        //if (GamePhaseManager.Instance.stickedOnTeeth)
        //{
        //    foreach (var c in fakeChocolateFragment)
        //    {
        //        Color newColor = c.material.color;
        //        newColor.a = 1f;
        //        c.material.color = newColor;
        //    }
        //}

        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            toothBrush.transform.position = Vector3.Lerp(toothBrushReadyPoint.position, orgToothBrushPos, t);
            toothBrush.transform.rotation = Quaternion.Lerp(toothBrushReadyPoint.rotation, Quaternion.identity, t);
            yield return null;
        }
        GamePhaseManager.Instance.EndPhase();
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

    IEnumerator CR_GetToothPasteAndMoveToReadyPosition(System.Action callback = null)
    {
        toothBrush.gameObject.SetActive(true);
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            toothBrush.transform.position = Vector3.Lerp(orgToothBrushPos, GetToothpastePoint.position, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        toothPaste.gameObject.SetActive(true);
        t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            toothBrush.transform.rotation = Quaternion.Lerp(toothBrush.transform.rotation, GetToothpastePoint.rotation, t);
            yield return null;
        }
        t = 0f;

        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            toothBrush.transform.rotation = Quaternion.Lerp(toothBrush.transform.rotation, Quaternion.identity, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            toothBrush.transform.position = Vector3.Lerp(GetToothpastePoint.position, toothBrushReadyPoint.position, t);
            toothBrush.transform.rotation = Quaternion.Lerp(Quaternion.identity, toothBrushReadyPoint.rotation, t);
            yield return null;
        }
        toothBrush.transform.localScale = Vector3.one;
        callback?.Invoke();
    }

}
