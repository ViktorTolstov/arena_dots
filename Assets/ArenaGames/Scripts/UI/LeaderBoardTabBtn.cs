using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class LeaderBoardTabBtn : Button
    {
        [SerializeField] private TextMeshProUGUI _text;

        private string _lbAlias;

        public void Setup(string lbAlias)
        {
            _lbAlias = lbAlias;
            onClick.AddListener(OnTabClick);
        }

        private void OnTabClick()
        {
            Debug.LogError("Open " + _lbAlias);
        }
    }
}