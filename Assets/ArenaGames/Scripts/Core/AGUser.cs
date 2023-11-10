using System;
using Cysharp.Threading.Tasks;

namespace ArenaGames
{
    public class AGUser
    {
        internal LoginStruct AccessInfo;
        internal PlayerInfoStruct PlayerInfo;
        internal CurrenciesData CurrencyInfo;

        private ArenaGamesController _controller;

        public void Setup(ArenaGamesController controller)
        {
            _controller = controller;
        }

        public void SetSignInData(LoginStruct accessInfo, bool withLoginEvent = true)
        {
            AccessInfo = accessInfo;
            ApplyAccessInfo(null, withLoginEvent);
        }

        public async void SignInUser(LoginData loginData, Action onSuccess, Action<string> onError)
        {
            await ArenaGamesController.Instance.NetworkController.SignInUser(loginData, () =>
            {
                ApplyAccessInfo(onSuccess);
            }, onError);
        }

        private async void ApplyAccessInfo(Action callback = null, bool withLoginEvent = true)
        {
            if (withLoginEvent)
            {
                ArenaGamesController.Instance.HideSplashScreen();
            }

            ArenaGamesController.Instance.PlayerData.SaveRefreshToken(AccessInfo.refreshToken);
            ArenaGamesController.Instance.NetworkController.RefreshUserCurrency();
            
            RefreshAuthToken();
            await _controller.NetworkController.GetUserData();

            if (withLoginEvent)
            {
                ArenaGamesController.Instance.OnSuccessfulLogin();
            }
            
            callback?.Invoke();
        }

        private async void RefreshAuthToken()
        {
            // TODO: make refresh on token expire and not every 30 seconds
            await UniTask.Delay(TimeSpan.FromSeconds(30f));

            await _controller.NetworkController.RefreshAuthData(
                AccessInfo.refreshToken.token,
                AccessInfo.refreshToken.expiresIn,
                RefreshAuthToken
            );
        }
    }
}