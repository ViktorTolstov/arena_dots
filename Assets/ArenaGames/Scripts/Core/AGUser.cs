using System.Collections;
using System.Collections.Generic;
using Gpm.WebView;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ArenaGames
{
    // TODO: move all coroutines to async tasks
    public class AGUser : MonoBehaviour
    {
        internal LoginStruct AccessInfo;
        internal PlayerInfoStruct PlayerInfo;
        internal CurrenciesData CurrencyInfo;

        #region Auth

        public void SetSignInData(LoginStruct accessInfo, bool withLoginEvent = true)
        {
            AccessInfo = accessInfo;
            StartCoroutine(ApplyAccessInfo(null, withLoginEvent));
        }
        
        public void SignInUser(LoginData loginData, UnityAction onSuccess, UnityAction<string> onError)
        {
            StartCoroutine(IE_SignInUser(loginData, onSuccess, onError));
        }

        IEnumerator IE_SignInUser(LoginData loginData, UnityAction onSuccess, UnityAction<string> onError)
        {
            WWWForm _Form = new WWWForm();
            _Form.AddField("password", loginData.password);
            _Form.AddField("login", loginData.username);

            UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.AUTH_URI, _Form);

            yield return _Request.SendWebRequest();

            if (_Request.isDone)
            {
                switch (_Request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.ProtocolError:
                        onError?.Invoke(AGErrorMessages.GetErrorMessage(_Request.result == UnityWebRequest.Result.ConnectionError, _Request.responseCode, AGErrorMessages.ERROR_MESSAGES_SIGNIN));
                        break;
                    case UnityWebRequest.Result.Success:
                        AccessInfo = JsonUtility.FromJson<LoginStruct>(_Request.downloadHandler.text);

                        yield return ApplyAccessInfo(onSuccess);
                        
                        break;
                }
            }
        }

        private IEnumerator ApplyAccessInfo(UnityAction callback = null, bool withLoginEvent = true)
        {
            ArenaGamesController.Instance.HideSplashScreen();
            
            ArenaGamesController.Instance.PlayerData.SaveRefreshToken(AccessInfo.refreshToken);
            
            StartCoroutine(nameof(IE_RefreshAuthToken));
            yield return StartCoroutine(nameof(IE_GetUserData));
            yield return StartCoroutine(nameof(IE_GetUserCurrency));

            if (withLoginEvent)
            {
                ArenaGamesController.Instance.OnSuccessfulLogin();
            }
            
            callback?.Invoke();
        }

        private IEnumerator IE_RefreshAuthToken()
        {
            // TODO: make refresh on token expire and not every 30 seconds
            yield return new WaitForSeconds(30f);

            while (true)
            {
                WWWForm _Form = new WWWForm();
                UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.REFRESH_TOKEN_URI, _Form);
                _Request.SetRequestHeader("refresh-token", AccessInfo.refreshToken.token);

                yield return _Request.SendWebRequest();

                if (_Request.isDone)
                {
                    if (_Request.result == UnityWebRequest.Result.Success)
                    {
                        AccessInfo.accessToken = JsonUtility.FromJson<RefreshTokenStruct>(_Request.downloadHandler.text).accessToken;

                        yield return new WaitForSeconds(30f);
                    }
                    else
                    {
                        Debug.Log("Failed to update access token");
                    }
                }
            }
        }
        #endregion

        #region Get User Data

        private IEnumerator IE_GetUserData()
        {
            UnityWebRequest _Request = UnityWebRequest.Get(AGHelperURIs.CLIENT_PROFILE_URI);

            _Request.SetRequestHeader("accept", "application/json");
            _Request.SetRequestHeader("access-token", AccessInfo.accessToken.token);

            yield return _Request.SendWebRequest();

            if (_Request.isDone)
            {
                if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
                {
                }
                else if (_Request.result == UnityWebRequest.Result.Success)
                {
                    PlayerInfo = JsonUtility.FromJson<PlayerInfoStruct>(_Request.downloadHandler.text);
                    ArenaGamesController.Instance.SetNickName(PlayerInfo.username);
                }
            }
        }

        private IEnumerator IE_GetUserCurrency()
        {
            UnityWebRequest _Request = UnityWebRequest.Get(AGHelperURIs.CURRENCY_URI);

            _Request.SetRequestHeader("accept", "application/json");
            _Request.SetRequestHeader("access-token", AccessInfo.accessToken.token);

            yield return _Request.SendWebRequest();

            if (_Request.isDone)
            {
                if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(_Request.downloadHandler.text);
                }
                else if (_Request.result == UnityWebRequest.Result.Success)
                {
                    CurrencyInfo = new CurrenciesData();
                    CurrencyInfo.CurrencyInfo = JsonConvert.DeserializeObject<List<CurrencyInfoStruct>>(_Request.downloadHandler.text);
                }
            }
        }
        #endregion
    }
}