using ArenaGames.EventServer;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class ConnectionErrorPopup: UIElementBase
    {
        [SerializeField] private Button _btnRestart;

        private ArenaGamesController _controller;

        private void OnEnable()
        {
            _btnRestart.onClick.AddListener(OpenArenaBrowser);
        }

        private void OpenArenaBrowser()
        {
            ArenaGamesController.Instance.EventServerController.ScheduleEvent(AGEventServerController.EventType.Logout);
            ArenaGamesController.Instance.EventServerController.SetOnline(false);
            ArenaGamesController.Instance.PlayerData.ClearRefreshToken();

            ArenaGamesController.Instance.StartProcess();
            Destroy(m_InGameUIController.gameObject);
            ArenaGamesController.Instance.ShowConnectionError(false);
        }
        
        private void OnDisable()
        {
            _btnRestart.onClick.RemoveAllListeners();
        }
    }
}