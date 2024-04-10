using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityAnimationController : MonoBehaviour {
    [SerializeField] Animator infinityContainerAnimator;
    [SerializeField] TrailRenderer trailRenderer;
    public static InfinityAnimationController Instance;
    private void Awake () {
        if (Instance == null) {
            Instance = this;
        } else {
            DestroyImmediate (gameObject);
        }
    }
    public void Show () {
        trailRenderer.Clear ();
        infinityContainerAnimator.gameObject.SetActive (true);
        infinityContainerAnimator.enabled = true;
    }
    public void Hide () {
        infinityContainerAnimator.gameObject.SetActive (false);
        infinityContainerAnimator.enabled = false;
    }
}