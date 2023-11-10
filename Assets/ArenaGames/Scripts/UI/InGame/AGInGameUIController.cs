using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class AGInGameUIController : MonoBehaviour
    {
        [SerializeField] private UIElement_Leaderboard _leaderboardPanel;
        [SerializeField] private UIElement_AccountPanel _accountPanel;
        [SerializeField] private UIElement_TopPanel _topPanel;
        [SerializeField] private UIElement_PayPanel _payPanel;
        [SerializeField] private ConnectionErrorPopup _connectionErrorPanel;
        [SerializeField] private GameObject _bottomPanel;
        [SerializeField] private TabButton _leaderboardButton;
        [SerializeField] private TabButton _accountButton;
        [SerializeField] private TabButton _playButton;

        private TabButton _lastClickedButton;
        private UIElementBase _lastOpenedWindow;

        public GameObject BottomPanel => _bottomPanel;
        public UIElement_Leaderboard LeaderboardPanel => _leaderboardPanel;

        private void OnEnable()
        {                
            _playButton.onClick.AddListener(() => OnTapButtonClick(null, _playButton, false));
            _leaderboardButton.onClick.AddListener(() => OnTapButtonClick(_leaderboardPanel, _leaderboardButton));
            _accountButton.onClick.AddListener(() => OnTapButtonClick(_accountPanel, _accountButton));

            _playButton.onClick?.Invoke();
        }

        private void OnDisable()
        {
            _leaderboardButton.onClick.RemoveAllListeners();
            _accountButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
        }

        private void OnTapButtonClick(UIElementBase panel, TabButton button, bool showTopPanel = true)
        {
            if (_lastClickedButton == button) return;
            
            ResetPanels();
            
            panel?.Open();
            button.UpdatePress(true);
            
            if (showTopPanel)
            {
                _topPanel.UpdateData();
            }
            
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

        public void ShowPayPanel(bool isVisible)
        {                
            _payPanel.gameObject.SetActive(isVisible);
        }
        
        public void ShowConnectionError(bool isVisible)
        {                
            _connectionErrorPanel.gameObject.SetActive(isVisible);
        }
    }
}