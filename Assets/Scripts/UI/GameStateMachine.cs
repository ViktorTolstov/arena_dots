using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForiDots.UI
{
    public class GameStateMachine : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _returnButton;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private GameObject _scoreHolder;
        [SerializeField] private GameObject _root;

        public event Action OnStartGame;
        public event Action<int> OnGameFinish;

        private void OnEnable()
        {
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            OnStartGame?.Invoke();
            
            _root.SetActive(false);
        }

        public void StopGame(int score)
        {
            _score.text = score.ToString();
            _returnButton.onClick.AddListener(() => FinishGame(score));
            
            _returnButton.gameObject.SetActive(true);
            _startButton.gameObject.SetActive(false);
            _scoreHolder.SetActive(true);
            _root.SetActive(true);
        }

        public void FinishGame(int score)
        {
            OnGameFinish?.Invoke(score);
            SetStartState();
        }

        public void SetStartState()
        {
            _root.SetActive(true);
            _returnButton.gameObject.SetActive(false);
            _scoreHolder.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(true);
            _returnButton.onClick.RemoveAllListeners();
        }
        
        private void OnDisable()
        {
            _startButton.onClick.RemoveAllListeners();
        }
    }
}