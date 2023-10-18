using System;
using System.Collections;
using System.Collections.Generic;

using ArenaGames;
using ArenaGames.Network;
using Gpm.WebView;
using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// TODO: consider to make not monobeh
[RequireComponent(typeof(ArenaGamesController))]
public class AGNetworkControllerOld : MonoBehaviour
{
    private ArenaGamesController _controller;
    
    public void Setup(ArenaGamesController controller)
    {
        _controller = controller;
    }
    
    #region WebView

    private const string SCHEME_NAME = "ARENA_SDK_SCHEME";
    private const string ACCESS_PATTERN = "access=([^&]+)";
    private const string REFRESH_PATTERN = "refresh=([^&]+)";
    private const string ACCESS_EXPIRES_PATTERN = "accessExpiresIn=([^&]+)";
    private const string REFRESH_EXPIRES_PATTERN = "refreshExpiresIn=([^&]+)";
        
#if UNITY_ANDROID || UNITY_IOS
    public void OpenSignInWebView()
    {
        GpmWebView.ShowUrl(
            AGHelperURIs.WEBVIEW_AUTH_URI,
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                backgroundColor = "#FFFFFF",
                isNavigationBarVisible = true,
                navigationBarColor = "#030A17",
                title = "ARENA AUTH",
                isBackButtonVisible = true,
                isForwardButtonVisible = true,
                isCloseButtonVisible = true,
                supportMultipleWindows = true,
                customSchemePostCommand = new GpmWebViewRequest.CustomSchemePostCommand()
#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE
#endif
            },
            OnWebViewCallback,
            new List<string>()
            {
                SCHEME_NAME
            });
    }
#endif
    
    private void OnWebViewCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
    {
        if (callbackType == GpmWebViewCallback.CallbackType.PageStarted && !string.IsNullOrEmpty(data) && data.Contains(SCHEME_NAME))
        {
            var isAccessValid = AGRegexHelper.TryParseKey(data, ACCESS_PATTERN, out var accessToken);
            var isRefreshValid = AGRegexHelper.TryParseKey(data, REFRESH_PATTERN, out var refreshToken);
            var isAccessExpiresValid = AGRegexHelper.TryParseKey(data, ACCESS_EXPIRES_PATTERN, out var accessExpires);
            var isRefreshExpiresValid = AGRegexHelper.TryParseKey(data, REFRESH_EXPIRES_PATTERN, out var refreshExpires);

            if (!isAccessValid || !isRefreshValid || !isAccessExpiresValid || !isRefreshExpiresValid)
            {
                Debug.LogError("AGNetworkController:: not valid token data from webview");
            }
            
            var loginData = new LoginStruct()
            {
                accessToken = new AccessToken()
                {
                    token = accessToken,
                    expiresIn = long.Parse(accessExpires)
                },
                refreshToken = new RefreshToken()
                {
                    token = refreshToken,
                    expiresIn = long.Parse(refreshExpires)
                }
            };
                
            _controller.User.SetSignInData(loginData);
        }
    }

    #endregion
    
    #region Registration
    public void TryRegisterUser(RegistrationData _Data, UnityAction _OnSuccess, UnityAction<string> _OnError)
    {
        StartCoroutine(IE_SendRegisterRequest(_Data, _OnSuccess, _OnError));
    }

    public void TryConfirmUser(RegistrationData _Data, UnityAction _OnSuccess, UnityAction<string> _OnError)
    {
        StartCoroutine(IE_TryConfirmEmail(_Data, _OnSuccess, _OnError));
    }

    private IEnumerator IE_SendRegisterRequest(RegistrationData _Data, UnityAction _OnSuccess, UnityAction<string> _OnError)
    {
        WWWForm _Form = new WWWForm();

        _Form.AddField("email", _Data.email);
        _Form.AddField("username", _Data.username);
        _Form.AddField("password", _Data.password);

        UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.REGISTRATION_URI, _Form);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            switch (_Request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                {
                    Debug.LogError(_Request.downloadHandler.text);
                    var errorData = ResponseStruct.TryParse<ResponseStruct.ErrorResponseStruct>(_Request.downloadHandler.text);
                    var errorText = errorData == null ? 
                        AGErrorMessages.GetErrorMessage(_Request.result == UnityWebRequest.Result.ConnectionError, _Request.responseCode, AGErrorMessages.ERROR_MESSAGES_SIGNIN) :
                        errorData.message;

                    _OnError?.Invoke(errorText);
                    break;
                }
                case UnityWebRequest.Result.Success:
                    _OnSuccess?.Invoke();
                    break;
            }
        }
    }

    private IEnumerator IE_TryConfirmEmail(RegistrationData _Data, UnityAction _OnSuccess, UnityAction<string> _OnError)
    {
        WWWForm _Form = new WWWForm();

        _Form.AddField("email", _Data.email);
        _Form.AddField("code", _Data.code);

        UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.CONFIRM_EMAIL_URI, _Form);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
            {
                _OnError?.Invoke(AGErrorMessages.GetErrorMessage(_Request.result == UnityWebRequest.Result.ConnectionError, _Request.responseCode, AGErrorMessages.ERROR_MESSAGES_SIGNIN));
            }
            else if (_Request.result == UnityWebRequest.Result.Success)
            {
                _OnSuccess?.Invoke();
            }
        }
    }
    #endregion

    public void GetLeaderboard(string _LeaderboardId, UnityAction<LeaderboardsStruct> _Action)
    {
        StartCoroutine(IE_GetLeaderboard(_LeaderboardId, _Action));
    }

    private IEnumerator IE_GetLeaderboard(string _LeaderboardId, UnityAction<LeaderboardsStruct> _Action)
    {
        Debug.Log("Trying to load leaderboard: " + _LeaderboardId);

        UnityWebRequest _Request = UnityWebRequest.Get(AGHelperURIs.LEADERBOARD_URI + _LeaderboardId + "?aroundPlayerLimit=1&limit=25&offset=0");

        _Request.SetRequestHeader("accept", "application/json");
        _Request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            Debug.Log(_Request.responseCode);

            if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Failed to load leaderboard. Error message: " + _Request.downloadHandler.text);
            }
            if (_Request.result == UnityWebRequest.Result.Success)
            {
                LeaderboardsStruct _Leaderboard = new LeaderboardsStruct();
                _Leaderboard.leaderboards = new List<Leaderboard>();
                _Leaderboard.aroundLeaderboards = new List<AroundLeaderboard>();

                _Leaderboard = JsonConvert.DeserializeObject<LeaderboardsStruct>(_Request.downloadHandler.text);

                _Action?.Invoke(_Leaderboard);
            }
        }
    }

    public void UpdateToLeaderboard(int _Value)
    {
        StartCoroutine(IE_PostToLeaderboard(_Value));
    }

    // TODO: refactor this
    private IEnumerator IE_PostToLeaderboard(int _Value)
    {
        WWWForm _Form = new WWWForm();

        _Form.AddField("profileId", ArenaGamesController.Instance.User.PlayerInfo.id);
        _Form.AddField("value", _Value);

        UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.LEADERBOARD_POST + ArenaGamesController.Instance.GameData.activeLeaderboards[0].alias + "/score", _Form);
        _Request.method = "PATCH";

        _Request.SetRequestHeader("accept", "application/json");
        _Request.SetRequestHeader("x-auth-server", AGCore.Settings.ServerToken);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Failed to send leaderboard score: " + _Request.downloadHandler.text);
            }
            else if (_Request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score posted successfully");
            }
        }
    }

    public void GetAchievements(string _Limit, string _Offset, UnityAction<AchievementsStruct> _Action)
    {
        StartCoroutine(IE_GetAchievements(_Limit, _Offset, _Action));
    }

    private IEnumerator IE_GetAchievements(string _Limit, string _Offset, UnityAction<AchievementsStruct> _Action)
    {
        Debug.Log("Trying to load achievements");

        UnityWebRequest _Request = UnityWebRequest.Get(AGHelperURIs.GET_ACHIEVEMENTS_URI + "?limit=" + _Limit + "&offset=" + _Offset);

        _Request.SetRequestHeader("accept", "application/json");
        _Request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);
        _Request.SetRequestHeader("gameAlias", AGCore.Settings.GameAlias);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            Debug.Log(_Request.responseCode);

            if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Failed to load achievements. Error message: " + _Request.downloadHandler.text);
            }
            if (_Request.result == UnityWebRequest.Result.Success)
            {
                AchievementsStruct _NewAchievement = new AchievementsStruct();
                _NewAchievement = JsonConvert.DeserializeObject<AchievementsStruct>(_Request.downloadHandler.text);

                _Action?.Invoke(_NewAchievement);
            }
        }
    }

    public void ProgressAchievement(string _Achievement, int _Num)
    {
        StartCoroutine(IE_ProgressAchievement(_Achievement, _Num));
    }

    private IEnumerator IE_ProgressAchievement(string _Achievement, int _Num)
    {
        WWWForm _Form = new WWWForm();

        _Form.AddField("userId", ArenaGamesController.Instance.User.PlayerInfo.id);
        _Form.AddField("operation", "INCREMENT");
        _Form.AddField("value", _Num);

        UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.ACHIEVEMENTS_POST_URI + _Achievement + "/progress", _Form);
        _Request.method = "PATCH";

        _Request.SetRequestHeader("accept", "application/json");
        _Request.SetRequestHeader("x-auth-server", AGCore.Settings.ServerToken);

        yield return _Request.SendWebRequest();

        if (_Request.isDone)
        {
            if (_Request.result == UnityWebRequest.Result.ConnectionError || _Request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Failed to send achievement score: " + _Request.downloadHandler.text);
            }
            else if (_Request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("achievement posted successfully");
            }
        }
    }
    
    
}