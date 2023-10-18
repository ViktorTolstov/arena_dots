using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_StartGamePanel : UIElementBase
    {
        [SerializeField] private Button m_BtnStart;

        public event Action gameStarted;

        private void OnEnable()
        {
            m_BtnStart.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            gameStarted?.Invoke();
            Close();
        }

        private void OnDisable()
        {
            m_BtnStart.onClick.RemoveListener(StartGame);
        }
    }
}

