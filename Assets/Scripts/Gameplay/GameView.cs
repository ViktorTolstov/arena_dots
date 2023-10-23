using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fori.Helpers;
using ForiDots.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ForiDots
{
    /// <summary>
    /// Game field visual - dots view container
    /// </summary>
    public class GameView : MonoBehaviour
    {
        [SerializeField] private float _gameTime = 60;
        [SerializeField] private DotView _dotsView;
        [SerializeField] private LineView _lineView;
        [SerializeField] private List<Color> _dotsColor;
        [SerializeField] private ScoreHolder _scoreHolder;
        [SerializeField] private TimerHolder _timerHolder;
        [SerializeField] private Button _returnButton;

        private List<DotView> _dotsViews = new ();
        private GameController _gameController;
        private GameplaySceneContext _context;
        private Sequence _destroySequence = null;

        public void Setup(GameController gameController, GameplaySceneContext context)
        {
            _gameController = gameController;
            _context = context;

            var fieldData = gameController.Field;
            foreach (var dotData in fieldData)
            {
                var newDot = Instantiate(_dotsView, transform).GetComponent<DotView>();
                newDot.Setup(this);
                newDot.SetDotData(dotData.Position, GetColorByTypeId(dotData.TypeId));
                _dotsViews.Add(newDot);
            }

            _scoreHolder.gameObject.SetActive(true);
            _timerHolder.gameObject.SetActive(true);
            _lineView.gameObject.SetActive(true);
            
            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(ReturnToStartState);

            _gameController.ResetScore();

            _scoreHolder.UpdateScoreValue(_gameController.CurrentScore);
            StartCoroutine(GameTimer(_gameTime));
        }
        
        private void DestroyDots(List<Vector2Int> dotsToDestroy, Action updateAction)
        {
            var fieldData = _gameController.Field;
            _destroySequence = DOTween.Sequence();
            
            foreach (var dotData in fieldData)
            {
                var dotView = _dotsViews.Find(x => x.Position == dotData.Position);
                if (dotsToDestroy.Contains(dotView.Position))
                {
                    dotView.Hide(_destroySequence);
                }
            }

            _destroySequence.OnComplete(() =>
            {
                updateAction?.Invoke();
                _destroySequence = null;
            });
        }

        private void UpdateDots()
        {
            var fieldData = _gameController.Field;
            foreach (var dotData in fieldData)
            {
                var dotView = _dotsViews.Find(x => x.Position == dotData.Position);
                dotView.SetDotData(dotData.Position, GetColorByTypeId(dotData.TypeId));
            }
        }

        private void DeselectAll()
        {
            foreach (var dotView in _dotsViews)
            {
                dotView.Deselect();
            }
        }

        public void OnDotOver(Vector2Int position)
        {
            if (!Input.GetMouseButton(0)) return;
            var needUpdate = _gameController.TryChooseDot(position);
            if (needUpdate)
            {
                SelectChosenDots();
            }
            UpdateLinePositions();
        }
        
        public void OnDotUp()
        {
            var dotsToDestroy = _gameController.OnCursorUp();
            DeselectAll();
            
            DestroyDots(dotsToDestroy, UpdateDots);
            
            UpdateLinePositions();
            _scoreHolder.UpdateScoreValue(_gameController.CurrentScore);
        }

        private void SelectChosenDots()
        {
            var chosenDots = _gameController.ChosenDots;
            if (chosenDots.ContainsDuplicate())
            {
                var targetType = chosenDots[0].TypeId;
                foreach (var dotData in _gameController.Field)
                {
                    if (dotData.TypeId != targetType) continue;
                    
                    var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                    dot.Select();
                }
            }
            else
            {
                foreach (var dotData in chosenDots)
                {
                    var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                    dot.Select();
                }
            }
        }

        private void UpdateLinePositions()
        {
            var positions = new List<Vector3>();
            var typeId = _gameController.ChosenDots.Count > 0 ? _gameController.ChosenDots[^1].TypeId : -1;
            foreach (var dotData in _gameController.ChosenDots)
            {
                var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                positions.Add(dot.transform.position);
            }
            
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            positions.Add(mouseWorldPos);

            _lineView.UpdateLine(positions, GetColorByTypeId(typeId));
        }

        private Color GetColorByTypeId(int typeId)
        {
            return typeId == -1 ? Color.black : _dotsColor[typeId];
        }

        private void GameOver(bool silent = false)
        {
            foreach (var dotView in _dotsViews)
            {
                Destroy(dotView.gameObject);
            }
            
            _dotsViews.Clear();
            _scoreHolder.gameObject.SetActive(false);
            _timerHolder.gameObject.SetActive(false);
            _lineView.gameObject.SetActive(false);

            if (silent)
            {
                _context.GameStateMachine.FinishGame(0);
            } else
            {
                _context.GameStateMachine.StopGame(_gameController.CurrentScore);
            }
            
            StopAllCoroutines();
        }

        private IEnumerator GameTimer(float gameTime)
        {
            for (float i = 0; i < gameTime; i += Time.deltaTime)
            {
                _timerHolder.UpdateTimer(gameTime - i);
                yield return null;
            }
            
            GameOver();
        }

        private void ReturnToStartState()
        {
            GameOver(true);
        }
    }
}