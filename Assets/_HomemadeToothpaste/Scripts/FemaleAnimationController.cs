using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FemaleAnimationController : MonoBehaviour
{
    public enum Gender { Female, Male }

    public Gender gender;
    public
    Animator anim;
    public
    SkinnedMeshRenderer characterRenderer;
    public
    Transform fakeMouth;
    public
    Transform lieDownMouthPosition;
    public
    Transform sitMouthPosition;
    public
    Transform openMouthPosition;
    [Header("Blinking Animation")]
    public
    AnimationCurve blinkingCurve;
    public
    float blinkingDuration;
    [Header("Mouth animation")]
    public
    float openOrCloseMouthSpeed;
    public
    float garglingAnimationSpeed;
    public
    float garglingTime;
    public
    Transform sitPosition;
    public
    MultiAimConstraint headIK;
    public
    Transform headIKTarget;
    public
    Transform leftHandIKTarget;
    public
    Transform sitHeadTarget;
    public
    Transform sitSpineTarget;
    public
    Transform sitLeftHandTarget;
    public
    Transform vomitHeadTarget;
    public
    Transform vomitSpineTarget;
    public
    Transform vomitLeftHandTarget;
    public
    TwoBoneIKConstraint leftHandIk;
    public
    TwoBoneIKConstraint rightHandIK;
    public
    MultiAimConstraint spineIK;
    public
    Transform spineIKTarget;
    public
    GameObject glass;
    public
    ParticleSystem vomitParticle;
    public Mouth mouth;

    bool allowHeadIK;
    private float nextBlinkTime;
    private ParticleSystem twinkleVFX;

    // Start is called before the first frame update
    void Start()
    {
        mouth = GameObject.Find("teeth_character Variant").GetComponent<Mouth>();
        mouth.emitParticle = true;
        twinkleVFX = GameObject.FindWithTag("TwinkleVFX").GetComponent<ParticleSystem>();
        vomitParticle = GameObject.FindWithTag("VomitVFX").GetComponent<ParticleSystem>();
        if (fakeMouth != null)
        {
            fakeMouth.transform.localPosition = lieDownMouthPosition.localPosition;
            fakeMouth.transform.localRotation = lieDownMouthPosition.localRotation;
        }

        nextBlinkTime = Time.time;
        MouthMotion(true);
        //Gargling();
    }

    // Update is called once per frame
    void Update()
    {
        Blinking();
        headIK.weight = allowHeadIK ? 1 : 0;
    }

    public void Drink()
    {
        anim.SetLayerWeight(1, 1f);
        anim.speed = 1f;
        leftHandIk.weight = 0;
        rightHandIK.weight = 0;
        spineIK.weight = 1f;
        headIKTarget.transform.position = sitHeadTarget.position;
        anim.SetBool("Drink", true);
    }

    void Blinking()
    {
        if (Time.time > nextBlinkTime)
        {
            StartCoroutine("CR_Blinking");
        }
    }

    public void PlayTwinkleFX()
    {
        twinkleVFX.Play();
    }

    public void SetAllowHeadIK(int boolean)
    {
        allowHeadIK = (boolean != 0);
    }

    public void SetLayerWeight(float weight)
    {
        anim.SetLayerWeight(1, weight);
    }

    public void GlassDisplay(int display)
    {
        glass.SetActive(display != 0);
    }

    public void Vomit(bool boolean, System.Action callback = null)
    {
        StartCoroutine(CR_Vomit(boolean, callback));
    }

    IEnumerator CR_Vomit(bool boolean, System.Action callback)
    {
        if (boolean)
        {
            mouth.DisableAllParticles();
            if (gender == Gender.Female)
                SFXManager.Instance.playFemaleVomitSFX();
            else
                SFXManager.Instance.playMaleVomitSFX();
            spineIK.weight = 1f;
            SetLayerWeight(1f);
            SetAnimatorSpeed(1f);
        }
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            leftHandIKTarget.transform.position =
            Vector3.Lerp(
                boolean ? sitLeftHandTarget.transform.position : vomitLeftHandTarget.transform.position,
                boolean ? vomitLeftHandTarget.transform.position : sitLeftHandTarget.transform.position,
                t);
            headIKTarget.transform.position =
            Vector3.Lerp(
                boolean ? sitHeadTarget.transform.position : vomitHeadTarget.transform.position,
                boolean ? vomitHeadTarget.transform.position : sitHeadTarget.transform.position,
                t);
            spineIKTarget.transform.position =
            Vector3.Lerp(
                boolean ? sitSpineTarget.transform.position : vomitSpineTarget.transform.position,
                boolean ? vomitSpineTarget.transform.position : sitSpineTarget.transform.position,
                t);
            yield return null;
        }
        if (boolean)
        {
            vomitParticle.Play();
        }
        else
        {
            mouth.EnableStinkyParticles();
            vomitParticle.Stop();
        }
        anim.SetBool("Lose", boolean);
        callback?.Invoke();
    }

    public void AngryEmotion()
    {

        StartCoroutine("CR_AngryEmotion");
    }

    IEnumerator CR_AngryEmotion()
    {
        //anim.SetBool("Lose", true);
        anim.speed = 0f;
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            t = Mathf.Clamp01(t);
            characterRenderer.SetBlendShapeWeight(6, 100 - (t * 100f));
            characterRenderer.SetBlendShapeWeight(1, t * 100f);
            yield return null;
        }
        if (gender == Gender.Female)
            SFXManager.Instance.playFemaleScreamSFX();
        else
            SFXManager.Instance.playMaleScreamSFX();
    }

    public void OpenMouthTeethUpdatePosition()
    {
        if (fakeMouth != null)
        {
            fakeMouth.transform.localPosition = openMouthPosition.localPosition;
            fakeMouth.transform.localRotation = openMouthPosition.localRotation;
        }

    }


    public void SetWeightHeadIK(float weight)
    {

    }

    public void HappyEmotion()
    {
        StartCoroutine("CR_HappyEmotion");
    }

    public void SetAnimatorSpeed(float speed)
    {
        anim.speed = speed;
    }

    IEnumerator CR_HappyEmotion()
    {
        //anim.SetBool("Win", true);
        anim.speed = 0f;
        var t = 0f;
        while (t < 1f)
        {
            t += 2f * Time.deltaTime;
            t = Mathf.Clamp01(t);
            characterRenderer.SetBlendShapeWeight(0, t * 100f);
            yield return null;
        }

    }

    public void Sit()
    {
        if (fakeMouth != null)
        {
            fakeMouth.transform.localPosition = sitMouthPosition.localPosition;
            fakeMouth.transform.localRotation = sitMouthPosition.localRotation;
        }

        anim.SetBool("Sit", true);
        anim.speed = 0f;
        transform.position = sitPosition.position;
        transform.rotation = sitPosition.rotation;
        allowHeadIK = true;
    }

    IEnumerator CR_Blinking()
    {
        nextBlinkTime = Time.time + blinkingDuration;
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            t = Mathf.Clamp01(t);
            characterRenderer.SetBlendShapeWeight(5, blinkingCurve.Evaluate(t) * 100f);
            yield return null;
        }
    }

    public void MouthMotion(bool open)
    {
        StartCoroutine(CR_MouthMotion(open));
    }

    IEnumerator CR_MouthMotion(bool open)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += openOrCloseMouthSpeed * Time.deltaTime;
            t = Mathf.Clamp01(t);
            if (!open)
                characterRenderer.SetBlendShapeWeight(6, t * 100f);
            else
                characterRenderer.SetBlendShapeWeight(6, 100 - (t * 100f));
            characterRenderer.SetBlendShapeWeight(2, open ? t * 100f : 100 - (t * 100f));
            yield return null;
        }
    }


    public void Gargling()
    {
        StartCoroutine("CR_Gargling");
    }

    IEnumerator CR_Gargling()
    {
        BrushingPhase brushingPhase = (BrushingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.Brush);
        brushingPhase.DisableAllFoam();
        for (int i = 0; i < garglingTime; i++)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += garglingAnimationSpeed * Time.deltaTime;
                t = Mathf.Clamp01(t);
                characterRenderer.SetBlendShapeWeight(3, 100f * t);
                characterRenderer.SetBlendShapeWeight(4, 100f - (t * 100f));
                yield return null;
            }
        }
        var t2 = 0f;
        while (t2 < 1f)
        {
            t2 += garglingAnimationSpeed * Time.deltaTime;
            t2 = Mathf.Clamp01(t2);
            characterRenderer.SetBlendShapeWeight(3, 100f - (t2 * 100f));
            characterRenderer.SetBlendShapeWeight(4, 100f - (t2 * 100f));
            yield return null;
        }
        characterRenderer.SetBlendShapeWeight(3, 0f); //reset gargling blendshape;
        characterRenderer.SetBlendShapeWeight(4, 0f); //reset gargling blendshape;
    }
}
