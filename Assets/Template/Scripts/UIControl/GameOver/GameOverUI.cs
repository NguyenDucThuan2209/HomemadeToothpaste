using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LatteGames;

namespace LatteGames.Template
{
    public class GameOverUI : MonoBehaviour
    {
        public event Action Replay = delegate { };
        public event Action Next = delegate { };

        [SerializeField] private Button replayButton = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private Text title = null;
        private void Awake()
        {
            replayButton.onClick.AddListener(() => Replay());
            nextButton.onClick.AddListener(() => Next());
        }

        private void Start()
        {
            title.gameObject.SetActive(false);
        }

        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        public void SetButtonGroup(bool enableReplay, bool enableNext)
        {
            replayButton.gameObject.SetActive(enableReplay);
            nextButton.gameObject.SetActive(enableNext);

        }
    }
}