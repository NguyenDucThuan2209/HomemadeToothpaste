using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LatteGames;
using Unity.Mathematics;

public class GamePhaseManager : MonoBehaviour
{

    public static GamePhaseManager Instance;

    public enum GamePhaseEnum { Scan, SodaMix, StampOrBlend, IngredientMix, Brush }

    [Header("Management Attribute")]
    private float finalEffective;
    public Color finalResultPrimaryColor = Color.white;
    public Color finalResultSecondaryColor = Color.white;
    public List<IngredientItem> requiredIngredients;
    List<IngredientItem.Effect> requiredEffect = new List<IngredientItem.Effect>();
    public List<IngredientItem> droppedIngredients;
    public List<IngredientItem.Effect> toothpasteEffects;
    [SerializeField]
    List<PhaseBase> gamePhases;
    [SerializeField]
    private GamePhaseEnum gamePhaseName;
    [SerializeField]
    Transform endPhaseCameraPoint;
    [SerializeField]
    Transform endPhaseCameraPointFail;
    [SerializeField]
    Transform secondEndPhaseCameraPoint;

    [Header("UI Attribute")]
    RatingUI ratingUI;
    public CTAController CTAController;
    public InfinityAnimationController infinityAnimationController;



    [Header("Character Animation")]
    [SerializeField]
    public FemaleAnimationController femaleCharacter;
    public CustomLevelController customLevelController;
    [HideInInspector]
    public int starsGained;

    public float FinalEffective { get => finalEffective; set { finalEffective = Mathf.Clamp(value, 0f, 100f); } }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        starsGained = 0;
        femaleCharacter = FindObjectOfType<FemaleAnimationController>();
        CTAController = FindObjectOfType<CTAController>();
        infinityAnimationController = FindObjectOfType<InfinityAnimationController>();
        customLevelController = FindObjectOfType<CustomLevelController>();
        foreach (var i in requiredIngredients)
        {
            requiredEffect.Add(i.effect);
        }
        ProcessCurrentPhase();
    }

    public void GoToNextPhase()
    {
        gamePhaseName++;
        ProcessCurrentPhase();
    }

    public void EndPhase()
    {
        StartCoroutine("CR_EndPhase");
    }

    public PhaseBase GetPhase(GamePhaseEnum phaseName)
    {
        return gamePhases[(int)phaseName];
    }

    public void AddEffect(IngredientItem item)
    {
        if (!toothpasteEffects.Contains(item.effect))
        {
            toothpasteEffects.Add(item.effect);
        }
    }

    public void AddIngredient(IngredientItem item)
    {
        droppedIngredients.Add(item);
    }

    IEnumerator CR_EndPhase() //rating and end level
    {

        if (toothpasteEffects.Contains(IngredientItem.Effect.Vomit))
        {

            CameraController.Instance.MoveToTarget(endPhaseCameraPointFail, null);
            femaleCharacter.Vomit(true, () =>
            {
                BrushingPhase brushingPhase = (BrushingPhase)GetPhase(GamePhaseEnum.Brush);
                brushingPhase.DisableAllFoam();
            });
            yield return new WaitForSeconds(3f);
            femaleCharacter.Vomit(false);
            femaleCharacter.AngryEmotion();
            femaleCharacter.MouthMotion(false);
            yield return new WaitForSeconds(0.5f);
            femaleCharacter.SetLayerWeight(0f);
            femaleCharacter.SetAnimatorSpeed(0f);
            femaleCharacter.OpenMouthTeethUpdatePosition();
            CameraController.Instance.MoveToTarget(secondEndPhaseCameraPoint);
            starsGained = 0;
            customLevelController.LevelState = CustomLevelController.State.Lose;
            CustomGameLoopManager.Instance.SetStarGainedToRatingUI(starsGained);
            femaleCharacter.mouth.EnableStinkyParticles();
            //ratingUI.Rating(starsGained, () =>
            //{
            //    StartCoroutine("CR_EndLevel");
            //});

        }
        else //rating only happens if our patient is not vomiting
        {
            CameraController.Instance.MoveToTarget(endPhaseCameraPoint, null);
            femaleCharacter.MouthMotion(true);
            femaleCharacter.Drink();
            yield return new WaitForSeconds(1.5f);
            femaleCharacter.MouthMotion(false);
            femaleCharacter.Gargling();
            yield return new WaitForSeconds(1.5f);
            femaleCharacter.MouthMotion(true);
            femaleCharacter.SetLayerWeight(0f);
            femaleCharacter.SetAnimatorSpeed(0f);
            femaleCharacter.OpenMouthTeethUpdatePosition();
            CameraController.Instance.MoveToTarget(secondEndPhaseCameraPoint);
            femaleCharacter.SetAllowHeadIK(1);
            if (starsGained == 0)
            {
                customLevelController.LevelState = CustomLevelController.State.Lose;
                femaleCharacter.mouth.EnableStinkyParticles();
            }
            else
            {
                customLevelController.LevelState = CustomLevelController.State.Win;
                femaleCharacter.mouth.DisableAllParticles();

            }
            BrushingPhase brushingPhase = (BrushingPhase)GetPhase(GamePhaseEnum.Brush);
            brushingPhase.CleanTheDirts();

            //float roundedFinalEffective = Mathf.RoundToInt(FinalEffective);
            //int starsGained = Mathf.RoundToInt(math.remap(0f, 100f, 0f, 3f, roundedFinalEffective));
            CustomGameLoopManager.Instance.SetStarGainedToRatingUI(starsGained);
            if (starsGained == 3)
            {
                femaleCharacter.PlayTwinkleFX();
            }
            //ratingUI.Rating(starsGained, () =>
            //{
            //    StartCoroutine("CR_EndLevel");
            //});
            //Debug.Log(starsGained);
        }
        customLevelController.EndLevel();
    }


    //public void CalculateEffectiveness()
    //{
    //    List<IngredientItem.Effect> checkedEffects = new List<IngredientItem.Effect>();
    //    foreach (var i in toothpasteEffects)
    //    {
    //        if (requiredEffect.Contains(i) && !checkedEffects.Contains(i))
    //        {
    //            checkedEffects.Add(i);
    //        }
    //    }
    //    if (checkedEffects.Count == requiredEffect.Count)
    //    {
    //        finalEffective += 100f / 3f;
    //    }
    //}

    public void CalculateEffectiveness()
    {
        List<IngredientItem> checkedIngredient = new List<IngredientItem>();
        foreach (var i in droppedIngredients)
        {
            if (requiredIngredients.Contains(i) && !checkedIngredient.Contains(i))
            {
                checkedIngredient.Add(i);
            }
        }
        if (checkedIngredient.Count == requiredIngredients.Count)
        {
            FinalEffective += 70f;
            starsGained += 1;
        }
    }

    public void CheckForCorrectIngredient()
    {
        List<IngredientItem> checkedIngredient = new List<IngredientItem>();
        foreach (var i in droppedIngredients)
        {
            if (requiredIngredients.Contains(i) && !checkedIngredient.Contains(i))
            {
                checkedIngredient.Add(i);
            }
        }
        if (checkedIngredient.Count == 0)
        {
            FinalEffective = 0f;
            starsGained = 0;
        }
    }

    private void ProcessCurrentPhase()
    {
        var i = (int)gamePhaseName;
        gamePhases[i].PhaseProcessing();
    }


}
