using System;
using ArenaGames;
using ArenaGames.EventServer;
using ArenaGames.Network;
using Gpm.WebView;
using UnityEngine;

namespace ArenaGames
{
    public class ArenaGamesController : MonoBehaviour
    {
        // TODO: remove singleton
        // TODO: move all instantiates and destroys with hide/show windows
        public static ArenaGamesController Instance;

        [SerializeField] private AGSplashScreen _splashScreenPrefab;
        [SerializeField] private AGAuthUIController _authPanelPrefab;
        [SerializeField] private AGInGameUIController _inGameControlPrefab;

        private AGInGameUIController _inGameControl;
        private AGNetworkControllerOld _networkControllerOld;
        private AGNetworkController _networkController;
        private AGUser _user;
        private AGSplashScreen _splashScreen;
        private AGEventServerController _eventServerController;
        private ResponseStruct.GameInfoStruct _gameData;

        
        public event Action OnSuccessfulLoginEvent;
        public event Action<string> OnNickNameUpdateEvent;
        
        // TODO: make private
        public AGEventServerController EventServerController => _eventServerController;
        public AGNetworkController NetworkController => _networkController;
        public AGNetworkControllerOld NetworkControllerOld => _networkControllerOld;
        public AGUser User => _user;
        public ResponseStruct.GameInfoStruct GameData => _gameData;
        public AGInGameUIController GetInGameUIController() => _inGameControl;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _networkControllerOld = GetComponent<AGNetworkControllerOld>();
            _networkControllerOld.Setup(this);
            
            _user = GetComponent<AGUser>();

            _eventServerController = new AGEventServerController();
            _eventServerController.Setup(this);

            _networkController = new AGNetworkController();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (!AGCore.IsInitialized)
                AGCore.Initialize();

            StartProcess();
        }

        public void StartProcess()
        {
            _splashScreen = Instantiate(_splashScreenPrefab).GetComponent<AGSplashScreen>();
            _splashScreen.Setup(this);
        }

        public void ShowAuthScreen()
        {
            Instantiate(_authPanelPrefab);
        }

        private void ShowInGamePanel(bool isShow = true)
        {
            if (_inGameControl != null)
            {
                _inGameControl.gameObject.SetActive(isShow);
            }
            else
            {
                if (isShow)
                {
                    _inGameControl = Instantiate(_inGameControlPrefab);
                }
            }
        }

        public void SetGameData(ResponseStruct.GameInfoStruct gameData)
        {
            _gameData = gameData;
            foreach (var l in gameData.activeLeaderboards)
            {
                Debug.LogError(l.alias);
            }
        }

        /*private void OnLevelWasLoaded(int level)
        {
            if (level == 0)
                if (AGUser.Current.m_AccessInfo == null)
                    StartSignInProcess();
        }*/

        public void HideSplashScreen()
        {
            if (GpmWebView.IsActive())
            {
                GpmWebView.Close();
            }
            
            ShowInGamePanel();
        }
        
        #region Events
        
        public void OnSuccessfulLogin()
        {
            Destroy(_splashScreen.gameObject);
            
            _eventServerController.ScheduleEvent(AGEventServerController.EventType.Login);
            _eventServerController.SetOnline(true);
            
            _networkController.UpdateGameData();
            
            OnSuccessfulLoginEvent?.Invoke();
        }

        public void SetNickName(string nickName)
        {
            OnNickNameUpdateEvent?.Invoke(nickName);
        }
        #endregion
        
        #region Public Methods
        public void UpdateScore(int value)
        {
            NetworkControllerOld.UpdateToLeaderboard(value);
        }
        #endregion
        
        private void OnApplicationPause(bool isPaused)
        {
            Debug.Log("App paused " + isPaused);
            _eventServerController.SetPaused(isPaused);
            _eventServerController.ScheduleEvent(isPaused ? 
                AGEventServerController.EventType.CollapseApp : 
                AGEventServerController.EventType.ExpandApp);
        }
    }

}