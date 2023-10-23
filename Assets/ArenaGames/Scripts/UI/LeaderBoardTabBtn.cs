using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class LeaderBoardTabBtn : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Button _btn;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _inactiveColor;

        public string LbAlias => _lbAlias;
        
        private string _lbAlias;
        private Action<string> _updateLbAction;

        public void Setup(string lbAlias, string lbName, Action<string> updateLbAction)
        {
            _updateLbAction = updateLbAction;
            _name.text = lbName;
            _lbAlias = lbAlias;
            _btn.onClick.AddListener(OnTabClick);
        }

        public void SetActive(bool isActive)
        {
            _btn.interactable = !isActive;
            _name.color = isActive ? _activeColor : _inactiveColor;
        }

        private void OnTabClick()
        {
            _updateLbAction?.Invoke(_lbAlias);
        }

        private void OnDestroy()
        {
            _btn.onClick.RemoveAllListeners();
        }
    }
}