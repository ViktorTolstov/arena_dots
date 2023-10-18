using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_GameOverPanel : UIElementBase
    {
        [SerializeField] private Button _returnButton;
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void SetEndScore(int value) =>
            _scoreText.text = value.ToString();

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
            m_InGameUIController.GetBottomPanel().SetActive(true);
            Debug.Log("Bot panel opened");
            m_InGameUIController.GetUIElement<UIElement_StartGamePanel>().Open();
            Close();
        }
    }
}

