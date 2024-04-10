using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;

namespace _GemstonesAttachment
{
    public class GemstoneAttacment : MonoBehaviour
    {
        AttachmentTool tooth;
        List<AttachableSlot> attachableSlots;
        CustomerController customerController;
        [SerializeField] Text completedText;
        [SerializeField] CustomLevelController levelController;
        [SerializeField] ProgressBar progressBar;
        int count;
        private void Awake()
        {
            tooth = GetComponentInChildren<AttachmentTool>();
            customerController = GetComponentInChildren<CustomerController>();
            attachableSlots = new List<AttachableSlot>(GetComponentsInChildren<AttachableSlot>());
            tooth.OnGemAttached += OnGemAttached;
            //completedText.gameObject.SetActive(false);
        }
        void OnGemAttached()
        {
            count++;
            progressBar.SetProgress((float)count / attachableSlots.Count);
            if (count >= attachableSlots.Count)
            {
                EndGame();
            }
        }
        void EndGame()
        {
            customerController.CloseMouth();
            customerController.PlayTwinkleFX();
            tooth.DisableTooth();

            //completedText.gameObject.SetActive (true);
            //completedText.text = "COMPLETED: " + Mathf.Round ((float) count / attachableSlots.Count * 100) + "%";
            levelController.LevelState = CustomLevelController.State.Win;
            CustomGameLoopManager.Instance.SetStarGainedToRatingUI(3);
            StartCoroutine(CommonCoroutine.Delay(1f, false, () =>
            {
                //completedText.gameObject.SetActive (false);
                progressBar.Display(false, () => levelController.EndLevel());
            }));
        }
    }
}