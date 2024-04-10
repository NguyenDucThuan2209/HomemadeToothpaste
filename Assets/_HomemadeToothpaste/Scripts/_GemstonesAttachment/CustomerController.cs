using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;

public class CustomerController : MonoBehaviour {
    [SerializeField] SkinnedMeshRenderer bodyRenderer;
    [SerializeField] GameObject upperTeeth, lowerTeeth;
    [SerializeField] GameObject upperTeethCloseTransform, lowerTeethCloseTransform;
    [SerializeField] float mouthCloseBlendShapeValue = 68;
    [SerializeField] float closeMouthDuration = 0.3f;
    [SerializeField] ParticleSystem twinkleFX;
    public void CloseMouth () {
        var upperStartPos = upperTeeth.transform.localPosition;
        var upperTargetPos = upperTeethCloseTransform.transform.localPosition;
        var lowerStartPos = lowerTeeth.transform.localPosition;
        var lowerTargetPos = lowerTeethCloseTransform.transform.localPosition;
        var upperStartRot = upperTeeth.transform.localRotation;
        var upperTargetRot = upperTeethCloseTransform.transform.localRotation;
        var lowerStartRot = lowerTeeth.transform.localRotation;
        var lowerTargetRot = lowerTeethCloseTransform.transform.localRotation;
        StartCoroutine (CommonCoroutine.LerpFactor (closeMouthDuration, (t) => {
            bodyRenderer.SetBlendShapeWeight (2, Mathf.Lerp (100, mouthCloseBlendShapeValue, t));
            upperTeeth.transform.localPosition = Vector3.Lerp (upperStartPos, upperTargetPos, t);
            lowerTeeth.transform.localPosition = Vector3.Lerp (lowerStartPos, lowerTargetPos, t);
            upperTeeth.transform.localRotation = Quaternion.Lerp (upperStartRot, upperTargetRot, t);
            lowerTeeth.transform.localRotation = Quaternion.Lerp (lowerStartRot, lowerTargetRot, t);
        }));
    }
    public void PlayTwinkleFX () {
        twinkleFX.Play ();
    }
}