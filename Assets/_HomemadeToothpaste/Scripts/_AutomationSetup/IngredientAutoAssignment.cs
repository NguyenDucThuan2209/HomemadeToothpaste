using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientAutoAssignment : MonoBehaviour
{
    public enum ColliderType { Box, Sphere, Mesh, Capsule }
    [SerializeField]
    ColliderType colliderType;
    [SerializeField]
    bool childMeshColliderConvex;
    [SerializeField]
    bool childSmashable;
    [SerializeField]
    float childSmallestScale = 0.5f;
    [SerializeField]
    float childMoveUpAmount = 0.023f;
    [SerializeField]
    List<Mesh> childFragments;

    [ContextMenu("Ingredient Auto Assignment")]

    void Assignment()
    {
        if (transform.GetComponent<Rigidbody>())
        {
            Destroy(transform.GetComponent<Rigidbody>());
        }
        if (transform.GetComponent<Collider>())
        {
            Destroy(transform.GetComponent<Collider>());
        }
        if (transform.GetComponent<Destruction>())
        {
            Destroy(transform.GetComponent<Destruction>());
        }
        gameObject.AddComponent<Rigidbody>();
        switch (colliderType)
        {
            case ColliderType.Box:
                gameObject.AddComponent<BoxCollider>();
                break;
            case ColliderType.Sphere:
                gameObject.AddComponent<SphereCollider>();
                break;
            case ColliderType.Mesh:
                gameObject.AddComponent<MeshCollider>();
                break;
            case ColliderType.Capsule:
                gameObject.AddComponent<CapsuleCollider>();
                break;
        }

        Destruction parentDestuctionScript = gameObject.AddComponent<Destruction>();
        parentDestuctionScript.together = true;
        parentDestuctionScript.breakOnCollision = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Rigidbody>())
            {
                Destroy(transform.GetChild(i).GetComponent<Rigidbody>());
            }
            if (transform.GetChild(i).GetComponent<MeshCollider>())
            {
                Destroy(transform.GetChild(i).GetComponent<MeshCollider>());
            }
            if (transform.GetChild(i).GetComponent<Ingredient>())
            {
                Destroy(transform.GetChild(i).GetComponent<Ingredient>());
            }

            transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            MeshCollider childMeshCol = transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
            childMeshCol.convex = childMeshColliderConvex;
            Ingredient childIngredient = transform.GetChild(i).gameObject.AddComponent<Ingredient>();
            childIngredient.smashable = childSmashable;
            childIngredient.smallestScale = childSmallestScale;
            childIngredient.fragments = childFragments;
            childIngredient.moveUpAmount = childMoveUpAmount;
        }
    }
}
