using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;

namespace _Shopping
{
    public class ShopItemController : MonoBehaviour
    {
        Rigidbody m_Rigidbody;
        [SerializeField] PPrefFloat speed;
        [SerializeField] IngredientItem itemInfo;
        [SerializeField] AudioSource audioSource;
        [SerializeField] ReferentialSoundClip clip;
        [SerializeField] PPrefBool isDisableInput;
        GameObject ingredientGameObject;
        Collider ItemCollider;
        Collider ingredientCollider;
        bool hasTapped;

        public void SetItemInfo(IngredientItem item)
        {
            itemInfo = item;
        }

        private void Awake()
        {

            //ItemCollider = GetComponent<Collider>();
            hasTapped = false;
            ingredientGameObject = Instantiate(itemInfo.ingredientShopPrefab);
            ingredientGameObject.transform.position = transform.position;
            ingredientGameObject.transform.parent = transform;
            m_Rigidbody = GetComponent<Rigidbody>();
            //ingredientGameObject.transform.localPosition = Vector3.zero;
            //ItemCollider = ingredientGameObject.GetComponent<Collider>();
        }
        private void OnMouseDown()
        {
            if (hasTapped || isDisableInput)
            {
                return;
            }
            StartCoroutine(MoveToBasket());
        }
        IEnumerator MoveToBasket()
        {
            hasTapped = true;
            var basket = FindObjectOfType<BasketController>();
            var startPos = transform.position;
            var endPos = basket.droppedPoint.transform.position;
            var midPOs = startPos + ((endPos - startPos) / 2) + Vector3.up * 1.5f;
            var lastVelocity = Vector3.zero;
            var currentFakePos = transform.position;
            PlaySound(clip);
            while ((transform.position - basket.droppedPoint.transform.position).magnitude > 0.01f)
            {
                currentFakePos = Vector3.MoveTowards(currentFakePos, basket.droppedPoint.transform.position, speed * Time.deltaTime);
                var interpolateValue = (currentFakePos - startPos).magnitude / (endPos - startPos).magnitude;
                var m1 = Vector3.Lerp(startPos, midPOs, interpolateValue);
                var m2 = Vector3.Lerp(midPOs, endPos, interpolateValue);
                var m3 = Vector3.Lerp(m1, m2, interpolateValue);
                lastVelocity = (m3 - transform.position) / Time.deltaTime;
                transform.position = m3;
                yield return null;
            }
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            m_Rigidbody.AddForce(lastVelocity, ForceMode.VelocityChange);
            basket.AddItem(itemInfo);
            //ItemCollider.isTrigger = true;
            //ingredientCollider.isTrigger = false;
        }
        void PlaySound(ReferentialSoundClip clip)
        {
            audioSource.clip = clip.Clip;
            audioSource.Play();
        }
    }
}