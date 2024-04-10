using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendshapeCopy : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer sourceRenderer;

    SkinnedMeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        renderer.SetBlendShapeWeight(0, sourceRenderer.GetBlendShapeWeight(0));
        renderer.SetBlendShapeWeight(1, sourceRenderer.GetBlendShapeWeight(1));
        renderer.SetBlendShapeWeight(2, sourceRenderer.GetBlendShapeWeight(2));
        renderer.SetBlendShapeWeight(3, sourceRenderer.GetBlendShapeWeight(3));
        renderer.SetBlendShapeWeight(4, sourceRenderer.GetBlendShapeWeight(4));
        renderer.SetBlendShapeWeight(5, sourceRenderer.GetBlendShapeWeight(5));
        renderer.SetBlendShapeWeight(6, sourceRenderer.GetBlendShapeWeight(6));
    }
}
