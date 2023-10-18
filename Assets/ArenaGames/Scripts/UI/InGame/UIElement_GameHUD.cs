using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_GameHUD : UIElementBase
    {
        [SerializeField] private Button _returnButton;

        public event Action EndGame;

        private void OnEnable()
        {
            _returnButton.onClick.AddListener(ReturnToStart);
        }

        private void OnDisable()
        {
            _returnButton.onClick.RemoveAllListeners();
        }

        private void ReturnToStart()
        {
            EndGame?.Invoke();
            m_InGameUIController.GetBottomPanel().SetActive(true);
            Debug.Log("Bot panel opened");
            m_InGameUIController.GetUIElement<UIElement_StartGamePanel>().Open();
            Close();
        }
    }
}

