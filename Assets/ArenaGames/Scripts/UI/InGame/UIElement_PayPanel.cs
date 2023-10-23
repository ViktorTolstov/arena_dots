using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_PayPanel: UIElementBase
    {
        [SerializeField] private Button _btnPay;

        private ArenaGamesController _controller;

        private void OnEnable()
        {
            _btnPay.onClick.AddListener(OpenArenaBrowser);
        }

        private void OpenArenaBrowser()
        {
            ArenaGamesController.Instance.InGameControl.ShowPayPanel(false);
            Application.OpenURL("https://arenavs.com/");
        }
        
        private void OnDisable()
        {
            _btnPay.onClick.RemoveAllListeners();
        }
    }
}