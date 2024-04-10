using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;

namespace _Shopping {
    public class RequiredIngredientsCell : MonoBehaviour {
        Image m_Image;
        Material m_Material;
        IngredientItem myInfo;
        [SerializeField] CanvasGroup greenTick;
        [HideInInspector] public bool hasGrey;
        private void Awake () {
            m_Image = GetComponent<Image> ();
            var individualMaterial = Instantiate (m_Image.material);
            m_Material = individualMaterial;
            m_Image.material = individualMaterial;
            m_Material.SetFloat ("_GreyScale", 1);
        }
        public void Init (IngredientItem itemInfo) {
            m_Image.sprite = itemInfo.ingredientSprite;
            myInfo = itemInfo;
            greenTick.alpha = 0;
        }
        public void SwitchToGreyScale (float duration) {
            hasGrey = true;
            StartCoroutine (CommonCoroutine.LerpFactor (duration, (t) => {
                greenTick.alpha = t;
            }));
        }
        public bool IsSameKindOfIngredient (IngredientItem itemInfo) {
            return itemInfo == myInfo;
        }
    }
}