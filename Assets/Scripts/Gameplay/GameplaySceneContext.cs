using ArenaGames;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Custom realization of Context
    /// </summary>
    public class GameplaySceneContext : MonoBehaviour
    {
        [SerializeField] private GameView _gameView;
        
        private GameController _gameController;
        private GameModel _gameModel;

        private bool _isStarted;
        
        private void Start()
        {
            ArenaGamesController.Instance.OnSuccessfulLoginEvent += StartGame;
        }

        private void OnDisable()
        {
            ArenaGamesController.Instance.OnSuccessfulLoginEvent -= StartGame;
        }

        private void StartGame()
        {
            if (_isStarted) return;
            
            _isStarted = true;
            _gameModel = new GameModel();

            _gameController = new GameController();
            _gameController.Setup(_gameModel);

            var inGameUIController = ArenaGamesController.Instance.GetInGameUIController();
            var startPanel = (UIElement_StartGamePanel)inGameUIController.GetUIElement<UIElement_StartGamePanel>();
            var gameOverPanel = (UIElement_GameOverPanel)inGameUIController.GetUIElement<UIElement_GameOverPanel>();
            var gameHUD = (UIElement_GameHUD)inGameUIController.GetUIElement<UIElement_GameHUD>();
            startPanel.gameStarted += () => { _gameView.Setup(_gameController, gameOverPanel, gameHUD); };
        }
    }
}

