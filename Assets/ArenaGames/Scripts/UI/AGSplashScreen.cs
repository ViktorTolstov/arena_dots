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
        [SerializeField] private TextMeshProUGUI _buttonText;

        private ArenaGamesController _controller;
        private bool _needLogin;
        
        private void Start()
        {
            _animator.Play("SplashScreenFade");
            _title.text = AGCore.Settings.GameName;
        }

        private void OnEnable()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
        }

        public void Setup(ArenaGamesController controller, bool needLogin)
        {
            _controller = controller;
            _needLogin = needLogin;
            
            _buttonText.text = needLogin ? "Login" : "Play";
            _loginButton.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _loginButton.onClick.RemoveListener(OnLoginClicked);
        }

        private void OnLoginClicked()
        {
            if (!_needLogin)
            {
                ArenaGamesController.Instance.OnSuccessfulLogin();
                return;
            }
            
            if (AGCore.Settings.UseWebViewForSignIn)
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            _controller.NetworkController.OpenSignInWebView();
            return;
#endif
            }
            
            _controller.ShowAuthScreen();
        }
        
        
    }
}