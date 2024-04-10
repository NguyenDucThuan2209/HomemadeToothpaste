using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public enum SubType { Broke, Flatted }

    [SerializeField]
    IngredientItem.type Type;
    public SubType subType;
    public int splitAmount = 2;
    public bool smashable;
    [HideInInspector]
    public Collider col;
    [SerializeField]
    public float smallestScale;
    [SerializeField]
    public List<Mesh> fragments;
    [SerializeField]
    bool destroyOnCollision;
    public Rigidbody rb;
    public float moveUpAmount;
    StampingPhase stampingPhase;

    public float percentIncreament;
    // Start is called before the first frame update
    void Start()
    {
        if (Type == IngredientItem.type.Shatterable)
        {
            stampingPhase = (StampingPhase)GamePhaseManager.Instance.GetPhase(GamePhaseManager.GamePhaseEnum.StampOrBlend);
        }
        rb = GetComponent<Rigidbody>();
        //allowToSmash = true;
        col = GetComponent<Collider>();

    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (smashable && Type == IngredientItem.type.Shatterable)
        {
            if (collision.gameObject.CompareTag("Pestle"))
            {
                if (subType == SubType.Broke)
                {
                    if (transform.localScale.x > smallestScale)
                    {
                        for (int i = 0; i < splitAmount; i++)
                        {
                            int randomInd = Random.Range(0, fragments.Count - 1);
                            GameObject inst = Instantiate(gameObject);
                            if (fragments.Count > 0)
                            {
                                Mesh randomMesh = fragments[Random.Range(0, fragments.Count - 1)];
                                inst.GetComponent<MeshFilter>().mesh = randomMesh;
                            }
                            inst.transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
                            inst.transform.localScale -= Vector3.one * 0.5f;
                            Destroy(inst.GetComponent<MeshCollider>());
                            Destroy(inst.GetComponent<SphereCollider>());
                            inst.AddComponent<SphereCollider>();
                            inst.GetComponent<Rigidbody>().isKinematic = false;
                            Ingredient instIngredient = inst.GetComponent<Ingredient>();
                            instIngredient.smashable = false;
                            //instIngredient.splitAmount *= 2;
                            instIngredient.SetSmashableDelay(0.5f);
                            stampingPhase.StampingProgress += percentIncreament / (1f / smallestScale);

                            // 1 ingr => 15, 2 ingr => 10 , 3 ingr = 5
                        }
                        Destroy(gameObject);
                        stampingPhase.RemoveFromFragmentList(this);
                    }
                    else
                    {
                        stampingPhase.StampingProgress += percentIncreament / (1f / smallestScale);
                        stampingPhase.Powderization(true, moveUpAmount);
                        Destroy(gameObject); //cannot be split smaller, the powder will appeared
                        stampingPhase.RemoveFromFragmentList(this);
                    }

                }
                else
                {
                    if (transform.localScale.y > smallestScale)
                    {
                        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1f, 0.1f, 1f), 0.1f);
                        SetSmashableDelay(1f);
                        stampingPhase.StampingProgress += percentIncreament / (1f / smallestScale);
                    }
                    else
                    {
                        stampingPhase.Powderization(true, moveUpAmount);
                        stampingPhase.RemoveFromFragmentList(this);
                        Destroy(gameObject); //cannot be split smaller, the powder will appeared
                    }

                }

            }
        }
        if (destroyOnCollision && collision.gameObject.CompareTag("Pestle"))
        {
            Destroy(gameObject);
        }
    }

    public void SetSmashableDelay(float delay)
    {
        StartCoroutine(CR_SetSmashableDelay(delay));
    }

    IEnumerator CR_SetSmashableDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        smashable = true;
    }
    float addForceTimeStamp = 0;
    public void BounceWhenCollideWithBlade(Vector3 bladePosition, float affectRange, float forcePower)
    {
        if ((transform.position - bladePosition).magnitude <= affectRange && Time.time - addForceTimeStamp > 0.1f)
        {
            addForceTimeStamp = Time.time;
            rb.AddForce(Vector3.up * forcePower, ForceMode.VelocityChange);
        }
    }
}