using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class AGInGameUIController : MonoBehaviour
    {
        [SerializeField] private UIElement_Leaderboard _leaderboardPanel;
        [SerializeField] private UIElement_AccountPanel _accountPanel;
        [SerializeField] private UIElement_StartGamePanel _startGamePanel;
        [SerializeField] private UIElement_TopPanel _topPanel;
        [SerializeField] private UIElement_GameOverPanel _gameOverPanel;
        [SerializeField] private UIElement_GameHUD _gameHUD;
        [SerializeField] private GameObject _bottomPanel;
        [SerializeField] private TabButton _leaderboardButton;
        [SerializeField] private TabButton _accountButton;
        [SerializeField] private TabButton _playButton;

        private TabButton _lastClickedButton;
        private UIElementBase _lastOpenedWindow;

        public GameObject GetBottomPanel() =>
            _bottomPanel;
        public UIElementBase GetUIElement<T>() where T : UIElementBase
        {
            switch (typeof(T))
            {
                case var cls when cls == typeof(UIElement_Leaderboard):
                    return _leaderboardPanel;
                case var cls when cls == typeof(UIElement_AccountPanel):
                    return _accountPanel;
                case var cls when cls == typeof(UIElement_StartGamePanel):
                    return _startGamePanel;
                case var cls when cls == typeof(UIElement_TopPanel):
                    return _topPanel;
                case var cls when cls == typeof(UIElement_GameOverPanel):
                    return _gameOverPanel;
                case var cls when cls == typeof(UIElement_GameHUD):
                    return _gameHUD;
                default:
                    return null;
            }
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(() => OnTapButtonClick(_startGamePanel, _playButton, false));
            _leaderboardButton.onClick.AddListener(() => OnTapButtonClick(_leaderboardPanel, _leaderboardButton));
            _accountButton.onClick.AddListener(() => OnTapButtonClick(_accountPanel, _accountButton));
            _startGamePanel.gameStarted += () =>
            {
                _gameHUD.Open();
                _bottomPanel.gameObject.SetActive(false);
            };
            Debug.Log("Bot panel closed");
            
            _playButton.onClick?.Invoke();
        }

        private void OnDisable()
        {
            _leaderboardButton.onClick.RemoveAllListeners();
            _accountButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
            _startGamePanel.gameStarted -= () =>
            {
                _gameHUD.Open();
                _bottomPanel.gameObject.SetActive(true);
            };
        }

        private void OnTapButtonClick(UIElementBase panel, TabButton button, bool showTopPanel = true)
        {
            if (_lastClickedButton == button) return;
            
            ResetPanels();
            
            panel?.Open();
            button.UpdatePress(true);
            _topPanel.gameObject.SetActive(showTopPanel);
            
            _lastClickedButton = button;
            _lastOpenedWindow = panel;
        }

        private void ResetPanels()
        {
            if (_lastClickedButton != null)
                _lastClickedButton.UpdatePress(false);
            
            if (_lastOpenedWindow != null)
                _lastOpenedWindow.Close();
        }
    }
}