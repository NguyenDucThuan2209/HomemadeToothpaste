using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CopyCharacter : MonoBehaviour {
    [SerializeField] GameObject source;
    [SerializeField] Vector3 offsetPosition;
    [SerializeField] List<string> copyingObjectNames;
    [ContextMenu ("Copy")]
    void Copy () {
        CopyTransform (source.transform, transform);
        transform.position += offsetPosition;
        CopyAnimator ();
        CopyRigBuilder ();
        foreach (var name in copyingObjectNames) {
            CopyGameObject (name);
        }
        CopyFemaleAnimationController ();
    }
    void CopyAnimator () {
        var m_Animator = GetComponent<Animator> ();
        var sourceAnimator = source.GetComponent<Animator> ();
        m_Animator.applyRootMotion = sourceAnimator.applyRootMotion;
        m_Animator.runtimeAnimatorController = sourceAnimator.runtimeAnimatorController;
    }
    void CopyRigBuilder () {
        var sourceRigBuilder = source.GetComponent<RigBuilder> ();
        var m_RigBuilder = GetComponent<RigBuilder> ();
        if (m_RigBuilder != null) {
            DestroyImmediate (m_RigBuilder);
        }
        m_RigBuilder = gameObject.AddComponent<RigBuilder> ();

        var m_Rig = GetComponentInChildren<Rig> ();
        if (m_Rig != null) {
            DestroyImmediate (m_Rig.gameObject);
        }
        m_Rig = Instantiate (sourceRigBuilder.layers[0].rig);
        m_Rig.name = sourceRigBuilder.layers[0].rig.name;
        m_Rig.transform.SetParent (transform);
        CopyTransform (m_Rig.transform, sourceRigBuilder.layers[0].rig.transform);
        var rigLayer = new RigLayer (m_Rig);
        m_RigBuilder.layers.Add (rigLayer);
        var sourceTwoBones = sourceRigBuilder.layers[0].rig.gameObject.GetComponentsInChildren<TwoBoneIKConstraint> ();
        var targetTwoBones = m_Rig.gameObject.GetComponentsInChildren<TwoBoneIKConstraint> ();
        for (var i = 0; i < sourceTwoBones.Length; i++) {
            CopyTwoBoneIK (sourceTwoBones[i], targetTwoBones[i]);
        }
        var sourceTwoMultiAimConstraints = sourceRigBuilder.layers[0].rig.gameObject.GetComponentsInChildren<MultiAimConstraint> ();
        var targetTwoMultiAimConstraints = m_Rig.gameObject.GetComponentsInChildren<MultiAimConstraint> ();
        for (var i = 0; i < sourceTwoMultiAimConstraints.Length; i++) {
            CopyMultiAim (sourceTwoMultiAimConstraints[i], targetTwoMultiAimConstraints[i]);
        }
    }
    public Transform FindDeepChild (Transform aParent, string aName) {
        Queue<Transform> queue = new Queue<Transform> ();
        queue.Enqueue (aParent);
        while (queue.Count > 0) {
            var c = queue.Dequeue ();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue (t);
        }
        return null;
    }
    void CopyTwoBoneIK (TwoBoneIKConstraint source, TwoBoneIKConstraint target) {
        target.data.root = FindDeepChild (transform, source.data.root.name);
        target.data.mid = FindDeepChild (transform, source.data.mid.name);
        target.data.tip = FindDeepChild (transform, source.data.tip.name);
        target.data.target = source.data.target;
    }
    void CopyMultiAim (MultiAimConstraint source, MultiAimConstraint target) {
        target.data.constrainedObject = FindDeepChild (transform, source.data.constrainedObject.name);
        target.data.sourceObjects = source.data.sourceObjects;
    }
    void CopyGameObject (string name) {
        var sourceObject = FindDeepChild (source.transform, name);
        if (sourceObject == null) {
            return;
        }
        var parentTargetObject = FindDeepChild (transform, sourceObject.transform.parent.name);
        var targetObject = FindDeepChild (transform, name);
        if (targetObject != null) {
            DestroyImmediate (targetObject.gameObject);
        }
        targetObject = Instantiate (sourceObject);
        targetObject.name = sourceObject.name;
        targetObject.SetParent (parentTargetObject);
        CopyTransform (sourceObject, targetObject);
    }
    void CopyTransform (Transform source, Transform target) {
        target.transform.position = source.transform.position;
        target.transform.rotation = source.transform.rotation;
        target.transform.localScale = source.transform.localScale;
        target.SetSiblingIndex (source.GetSiblingIndex ());
    }
    void CopyFemaleAnimationController () {
        var targetFemaleAnimationController = GetComponent<FemaleAnimationController> ();
        var sourceFemaleAnimationController = source.GetComponent<FemaleAnimationController> ();
        if (targetFemaleAnimationController != null) {
            DestroyImmediate (targetFemaleAnimationController);
        }
        targetFemaleAnimationController = gameObject.AddComponent<FemaleAnimationController> ();
        targetFemaleAnimationController.anim = GetComponent<Animator> ();
        targetFemaleAnimationController.characterRenderer = FindDeepChild (transform, sourceFemaleAnimationController.characterRenderer.name).GetComponent<SkinnedMeshRenderer> ();
        targetFemaleAnimationController.fakeMouth = FindDeepChild (transform, sourceFemaleAnimationController.fakeMouth.name);
        targetFemaleAnimationController.lieDownMouthPosition = FindDeepChild (transform, sourceFemaleAnimationController.lieDownMouthPosition.name);
        targetFemaleAnimationController.sitMouthPosition = FindDeepChild (transform, sourceFemaleAnimationController.sitMouthPosition.name);
        targetFemaleAnimationController.openMouthPosition = FindDeepChild (transform, sourceFemaleAnimationController.openMouthPosition.name);
        targetFemaleAnimationController.glass = FindDeepChild (transform, sourceFemaleAnimationController.glass.name).gameObject;
        targetFemaleAnimationController.blinkingCurve = sourceFemaleAnimationController.blinkingCurve;
        targetFemaleAnimationController.blinkingDuration = sourceFemaleAnimationController.blinkingDuration;
        targetFemaleAnimationController.openOrCloseMouthSpeed = sourceFemaleAnimationController.openOrCloseMouthSpeed;
        targetFemaleAnimationController.garglingAnimationSpeed = sourceFemaleAnimationController.garglingAnimationSpeed;
        targetFemaleAnimationController.garglingTime = sourceFemaleAnimationController.garglingTime;
        targetFemaleAnimationController.sitPosition = sourceFemaleAnimationController.sitPosition;
        targetFemaleAnimationController.headIK = FindDeepChild (transform, sourceFemaleAnimationController.headIK.name).GetComponent<MultiAimConstraint> ();
        targetFemaleAnimationController.headIKTarget = sourceFemaleAnimationController.headIKTarget;
        targetFemaleAnimationController.leftHandIKTarget = sourceFemaleAnimationController.leftHandIKTarget;
        targetFemaleAnimationController.sitHeadTarget = sourceFemaleAnimationController.sitHeadTarget;
        targetFemaleAnimationController.sitSpineTarget = sourceFemaleAnimationController.sitSpineTarget;
        targetFemaleAnimationController.sitLeftHandTarget = sourceFemaleAnimationController.sitLeftHandTarget;
        targetFemaleAnimationController.vomitHeadTarget = sourceFemaleAnimationController.vomitHeadTarget;
        targetFemaleAnimationController.vomitSpineTarget = sourceFemaleAnimationController.vomitSpineTarget;
        targetFemaleAnimationController.vomitLeftHandTarget = sourceFemaleAnimationController.vomitLeftHandTarget;
        targetFemaleAnimationController.leftHandIk = FindDeepChild (transform, sourceFemaleAnimationController.leftHandIk.name).GetComponent<TwoBoneIKConstraint> ();
        targetFemaleAnimationController.rightHandIK = FindDeepChild (transform, sourceFemaleAnimationController.rightHandIK.name).GetComponent<TwoBoneIKConstraint> ();
        targetFemaleAnimationController.spineIK = FindDeepChild (transform, sourceFemaleAnimationController.spineIK.name).GetComponent<MultiAimConstraint> ();
        targetFemaleAnimationController.mouth = FindDeepChild (transform, sourceFemaleAnimationController.mouth.name).GetComponent<Mouth> ();
        targetFemaleAnimationController.spineIKTarget = sourceFemaleAnimationController.spineIKTarget;
        targetFemaleAnimationController.vomitParticle = sourceFemaleAnimationController.vomitParticle;
    }
}