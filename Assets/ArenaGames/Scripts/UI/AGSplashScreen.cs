using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gpm.WebView;
using System.Collections.Generic;

namespace ArenaGames
{
    public class AGSplashScreen : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _loginButton;

        private ArenaGamesController _controller;
        
        private void Start()
        {
            _animator.Play("SplashScreenFade");
            _title.text = AGCore.Settings.GameName;
        }

        private void OnEnable()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
        }

        public void Setup(ArenaGamesController controller)
        {
            _controller = controller;
        }

        private void OnDisable()
        {
            _loginButton.onClick.RemoveListener(OnLoginClicked);
        }

        private void OnLoginClicked()
        {
            if (AGCore.Settings.UseWebViewForSignIn)
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            _controller.NetworkControllerOld.OpenSignInWebView();
            return;
#endif
            }
            
            _controller.ShowAuthScreen();
        }
        
        
    }
}