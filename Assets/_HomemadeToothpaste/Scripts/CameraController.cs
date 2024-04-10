using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    Coroutine MovingCRRunner;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        MovingCRRunner = null;
    }

    public void MoveToTarget(Transform targetPoint, System.Action callback = null)
    {
        if (MovingCRRunner == null)
        {
            MovingCRRunner = StartCoroutine(CR_MoveToTarget(targetPoint, callback));
        }
    }

    IEnumerator CR_MoveToTarget(Transform targetPoint, System.Action callback)
    {
        var t = 0f;
        Vector3 orgPosition = transform.position;
        Quaternion orgRotation = transform.rotation;
        while (t < 1f)
        {
            t += Time.deltaTime;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(orgPosition, targetPoint.position, t);
            transform.rotation = Quaternion.Lerp(orgRotation, targetPoint.rotation, t);
            yield return null;
        }
        callback?.Invoke();
        MovingCRRunner = null;
    }
}
