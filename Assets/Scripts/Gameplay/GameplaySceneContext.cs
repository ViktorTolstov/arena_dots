using ArenaGames;
using ForiDots.UI;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Custom realization of Context
    /// </summary>
    public class GameplaySceneContext : MonoBehaviour
    {
        [SerializeField] private GameView _gameView;
        [SerializeField] private GameStateMachine _gameStateMachine;
        
        private GameController _gameController;
        private GameModel _gameModel;
        private bool _isStarted;
        private UIElement_StartGamePanel _startGamePanel;

        public GameStateMachine GameStateMachine => _gameStateMachine;
        
        private void Start()
        {
            ArenaGamesController.Instance.OnSuccessfulLoginEvent += TryStartGame;
        }

        private void OnDisable()
        {
            ArenaGamesController.Instance.OnSuccessfulLoginEvent -= TryStartGame;
        }

        private void TryStartGame()
        {
            if (_isStarted) return;

            _isStarted = true;
            _gameModel = new GameModel();

            _gameController = new GameController();
            _gameController.Setup(_gameModel);

            _gameStateMachine.OnStartGame += StartGame;
            _gameStateMachine.OnGameFinish += StopGame;
        }

        private async void StartGame()
        {                
            var isGameAllowed = await ArenaGamesController.Instance.TryStartGame();

            if (isGameAllowed)
            {
                _gameView.Setup(_gameController, this);
            }
            else
            {
                _gameStateMachine.SetStartState();
            }
        }

        private void StopGame(int score)
        {
            ArenaGamesController.Instance.StopGame(score);
        }
    }
}

