using System;
using ArenaGames.EventServer;
using ArenaGames.Network;
using ArenaGames.Scripts.Data;
using Cysharp.Threading.Tasks;
using Gpm.WebView;
using UnityEngine;

namespace ArenaGames
{
    public class ArenaGamesController : MonoBehaviour
    {
        // TODO: remove singleton
        // TODO: move all instantiates and destroys with hide/show windows
        public static ArenaGamesController Instance;

        [SerializeField] private AGSplashScreen _splashScreenPrefabLandscape;
        [SerializeField] private AGAuthUIController _authPanelPrefabLandscape;
        [SerializeField] private AGInGameUIController _inGameControlPrefabLandscape;

        [SerializeField] private AGSplashScreen _splashScreenPrefabPortrait;
        [SerializeField] private AGAuthUIController _authPanelPrefabPortrait;
        [SerializeField] private AGInGameUIController _inGameControlPrefabPortrait;

        private const float FlipLayoutFactor = 1.34f;
        private bool IsVertical = Screen.width < Screen.height * FlipLayoutFactor;

        private AGInGameUIController _inGameControl;
        private AGAuthUIController _authPanel;
        private AGNetworkController _networkController;
        private AGUser _user;
        private AGSplashScreen _splashScreen;
        private AGEventServerController _eventServerController;
        private ResponseStruct.GameInfoStruct _gameData;
        private AGData _playerData;
        
        public event Action OnSuccessfulLoginEvent;
        public event Action<string> OnNickNameUpdateEvent;
        
        // TODO: make private
        public AGEventServerController EventServerController => _eventServerController;
        public AGNetworkController NetworkController => _networkController;
        public AGUser User => _user;
        public ResponseStruct.GameInfoStruct GameData => _gameData;
        public AGInGameUIController InGameControl => _inGameControl;
        public AGData PlayerData => _playerData;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject);

            if (!AGCore.IsInitialized)
                AGCore.Initialize();

            _user = new AGUser();
            _user.Setup(this);

            _eventServerController = new AGEventServerController();
            _eventServerController.Setup(this);

            _networkController = new AGNetworkController();
            _networkController.Setup(this);

            _playerData = new AGData();
            
            StartProcess();
        }

        public async void StartProcess()
        {
            var targetSplashScreenPrefab = IsVertical ? _splashScreenPrefabPortrait : _splashScreenPrefabLandscape;
            _splashScreen = Instantiate(targetSplashScreenPrefab).GetComponent<AGSplashScreen>();
            
            var isRefreshExist = _playerData.TryGetRefreshToken(out var token, out var expiresIn);
            
            var isNeedLogin = !isRefreshExist || !await NetworkController.RefreshAuthData(token, expiresIn);
            
            _splashScreen.Setup(this, isNeedLogin);
            _networkController.SetOnline();
        }

        public void ShowAuthScreen()
        {
            var targetAuthPanelPrefab = IsVertical ? _authPanelPrefabPortrait : _authPanelPrefabLandscape;
            _authPanel = Instantiate(targetAuthPanelPrefab).GetComponent<AGAuthUIController>();
        }

        private void ShowInGamePanel(bool isShow = true)
        {
            if (_inGameControl != null)
            {
                _inGameControl.gameObject.SetActive(isShow);
            }
            else
            {
                if (!isShow) return;

                CreateInGameControl();
            }
        }

        private void CreateInGameControl()
        {
            var targetGameControlPrefab = IsVertical ? _inGameControlPrefabPortrait : _inGameControlPrefabLandscape;
            _inGameControl = Instantiate(targetGameControlPrefab);
        }

        public void SetGameData(ResponseStruct.GameInfoStruct gameData)
        {
            if (_inGameControl == null)
            {
                CreateInGameControl();
            }
            
            _gameData = gameData;
            _inGameControl.LeaderboardPanel.Setup();
        }

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
        public void StopGame(int score)
        {
            _inGameControl.gameObject.SetActive(true);

            if (score == 0) return;
            
            NetworkController.UpdateScore(score);
        }

        public async UniTask<bool> TryStartGame()
        {
            var isGameAllowed = await NetworkController.IsGameAllowed();
            var isGamePayed = false;
            
            if (isGameAllowed)
            {
                isGamePayed = await NetworkController.PayGame();
            }

            if (isGamePayed)
            {
                _inGameControl.gameObject.SetActive(false);
            }
            else
            {
                _inGameControl.ShowPayPanel(true);
            }

            return isGameAllowed;
        }
        
        public async UniTask ProgressAchievement(string achievementName, int count)
        {
            await NetworkController.ProgressAchievement(achievementName, count);
        }
        #endregion
        
        private void OnApplicationPause(bool isPaused)
        {
            _eventServerController.SetPaused(isPaused);
            _eventServerController.ScheduleEvent(isPaused ? 
                AGEventServerController.EventType.CollapseApp : 
                AGEventServerController.EventType.ExpandApp);
        }

        public void ShowConnectionError(bool isVisible)
        {
            if (_inGameControl == null)
            {
                CreateInGameControl();
            }
            
            InGameControl.ShowConnectionError(isVisible);
        }

        public void TryDestroyItems()
        {
            if (_splashScreen != null)
            {
                Destroy(_splashScreen.gameObject);
            }
            
            if (_authPanel != null)
            {
                Destroy(_authPanel.gameObject);
            }
        }
    }
}