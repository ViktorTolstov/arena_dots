using System;
using System.Collections.Generic;
using ArenaGames.EventServer;
using ArenaGames.Scripts.Core;
using Cysharp.Threading.Tasks;
using Gpm.WebView;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ArenaGames.Network
{
    public class AGNetworkController
    {
        private enum RequestType
        {
            Get,
            Post
        }
        
        private enum RequestName
        {
            UpdateGameData,
            SendEventServerEvent,
            ResendCode,
            GetResetData,
            UpdateScore,
            IsGameAllowed,
            PayGame,
            RefreshAuthData,
            RefreshUserCurrency,
            ProgressAchievement,
            GetLeaderboard,
            RegisterUser, 
            ConfirmEmail, 
            GetUserData,
            SignInUser,
        }
        
        private ArenaGamesController _controller;
        private bool _isOnline = true;
        private int _connectionErrorCounter;
        private Dictionary<RequestName, string> _requestsUris;

        public bool IsOnline => _isOnline;

        private const string PatchKey = "PATCH";
        private const string PostKey = "POST";
        public const int EntriesLimit = 10;
        
        public void Setup(ArenaGamesController controller)
        {
            _controller = controller;
            _requestsUris = new()
            {
                {RequestName.UpdateGameData, AGHelperURIs.GAME_INFO_API},
                {RequestName.SendEventServerEvent, AGHelperURIs.EVENT_SERVER_URI},
                {RequestName.ResendCode, AGHelperURIs.RESEND_CONFIRM_CODE},
                {RequestName.GetResetData, AGHelperURIs.GET_NEXT_TRY_FOR_RESEND_CODE},
                {RequestName.UpdateScore, AGHelperURIs.UPDATE_SCORE},
                {RequestName.IsGameAllowed, AGHelperURIs.IS_GAME_ALLOWED},
                {RequestName.PayGame, AGHelperURIs.PAY_GAME},
                {RequestName.RefreshAuthData, AGHelperURIs.REFRESH_TOKEN_URI},
                {RequestName.RefreshUserCurrency, AGHelperURIs.CURRENCY_URI},
                {RequestName.RegisterUser, AGHelperURIs.REGISTRATION_URI},
                {RequestName.ConfirmEmail, AGHelperURIs.CONFIRM_EMAIL_URI},
                {RequestName.GetUserData, AGHelperURIs.CLIENT_PROFILE_URI},
                {RequestName.SignInUser, AGHelperURIs.AUTH_URI},
            };
        }

        public void SetOnline()
        {
            _isOnline = true;
        }
        
        private async UniTask SendRequest(
            RequestName requestName, 
            RequestType requestType = RequestType.Get,
            string requestMethod = "",
            WWWForm form = null, 
            string jsonData = "",
            Action<string> onError = null, 
            Action<UnityWebRequest> onSuccess = null,
            bool isUseAuthToken = true,
            bool isUseRefreshToken = false,
            string refreshToken = "",
            string uriOverride = ""
            )
        {
            AGLogger.Log($"AGNetworkController:: {requestName}");
            
            var requestUri = _requestsUris.ContainsKey(requestName) ? _requestsUris[requestName] : uriOverride;
            if (requestUri == "")
            {
                AGLogger.LogError($"AGNetworkController:: unknown request {requestName}");
                return;
            }

            UnityWebRequest request;
            switch (requestType)
            {
                case RequestType.Post:
                    request = form == null ? 
                        new UnityWebRequest(requestUri, "POST") : 
                        UnityWebRequest.Post(requestUri, form);
                    break;
                default:
                    request = UnityWebRequest.Get(requestUri);
                    break;
            }

            if (form == null)
            {
                var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                request.SetRequestHeader("Accept", "application/json");
            }
            
            request.SetRequestHeader("gameAlias", AGCore.Settings.GameAlias);
            
            if (isUseAuthToken)
            {
                request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);
            }
            
            if (isUseRefreshToken)
            {
                request.SetRequestHeader("refresh-token", refreshToken);
            }

            if (requestMethod != "")
            {
                request.method = requestMethod;
            }

            try
            {
                var operation = request.SendWebRequest();
                await operation.ToUniTask();
            }
            catch (Exception exception)
            {
                FinalizeRequest(request, requestName, onError, onSuccess, async () =>
                {
                    await SendRequest(
                        requestName,
                        requestType,
                        requestMethod,
                        form,
                        jsonData,
                        onError,
                        onSuccess,
                        isUseAuthToken,
                        isUseRefreshToken,
                        refreshToken,
                        uriOverride
                    );
                });
            }
            finally
            {
                FinalizeRequest(request, requestName, onError, onSuccess);
            }
        }

        private void FinalizeRequest(UnityWebRequest request, RequestName requestName, Action<string> onError, Action<UnityWebRequest> onSuccess, Action onConnectionError = null)
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    AGLogger.Log("AGNetworkController:: ConnectionError");

                    if (_connectionErrorCounter > AGCore.Settings.MaxConnectionErrorsCount)
                    {
                        if (!_isOnline) return;
                        
                        _isOnline = false;
                        ArenaGamesController.Instance.EventServerController.ScheduleEvent(AGEventServerController.EventType.Disconnect);
                        ArenaGamesController.Instance.ShowConnectionError(true);
                        ArenaGamesController.Instance.TryDestroyItems();
                        return;
                    }

                    onConnectionError?.Invoke();
                    _connectionErrorCounter++;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    AGLogger.Log($"AGNetworkController:: Failed {requestName}. Error message: {request.error}");
                    var errorData = ResponseStruct.TryParse<ResponseStruct.ErrorResponseStruct>(request.downloadHandler.text);
                    var message = errorData == null ?
                        "Unexpected error, please try again" :
                        errorData.messageParams != null && errorData.messageParams.Count > 0 ? errorData.messageParams[0] : errorData.message;
                    onError?.Invoke(message);
                    break;
                case UnityWebRequest.Result.Success:
                    if (!_isOnline)
                    {
                        ArenaGamesController.Instance.EventServerController.ScheduleEvent(AGEventServerController.EventType.Connect);
                    }
                    
                    _isOnline = true;
                    _connectionErrorCounter = 0;
                    AGLogger.Log($"AGNetworkController:: Success {requestName}. Info: " + request.downloadHandler.text);
                    onSuccess?.Invoke(request);
                    break;
            }
        }
        
        public async UniTask UpdateGameData()
        {
            await SendRequest(
                requestName: RequestName.UpdateGameData,
                onSuccess: webRequest =>
                {
                    var gameData = ResponseStruct.TryParse<ResponseStruct.GameInfoStruct>(webRequest.downloadHandler.text);
                    ArenaGamesController.Instance.SetGameData(gameData);
                }
            );
        }
        
        public async UniTask SendEventServerEvent(List<string> events)
        {
            var dataEvents = new List<ResponseStruct.EventStruct>();
            foreach (var targetEvent in events)
            {
                var dataStruct = new ResponseStruct.EventStruct()
                {
                    gameId = AGCore.Settings.GameName,
                    userId = ArenaGamesController.Instance.User.PlayerInfo.username,
                    eventType = targetEvent,
                    timestamp = AGTimeController.TimestampMS,
                };
                
                dataEvents.Add(dataStruct);
            }

            var eventsStruct = new ResponseStruct.EventsStruct()
            {
                events = dataEvents
            };
            
            var jsonData = JsonConvert.SerializeObject(eventsStruct);

            await SendRequest(
                requestName: RequestName.SendEventServerEvent,
                requestType: RequestType.Post,
                jsonData: jsonData
            );
        }
        
        public async UniTask ResendCode(string email)
        {
            var form = new WWWForm();
            form.AddField("email", email);
            
            await SendRequest(
                requestName: RequestName.ResendCode,
                requestType: RequestType.Post,
                form: form,
                isUseAuthToken: false
            );
        }

        public async UniTask<long> GetResetData(string email)
        {
            var form = new WWWForm();
            form.AddField("email", email);
            
            long resultTime = 0;
            await SendRequest(
                requestName: RequestName.GetResetData,
                requestType: RequestType.Post,
                form: form,
                onSuccess: webRequest =>
                {
                    var codeData = ResponseStruct.TryParse<ResponseStruct.CodeAttemptResponseStruct>(webRequest.downloadHandler.text);
                    resultTime = long.Parse(codeData.nextAttemptTime) / 1000;
                },
                isUseAuthToken: false
            );
            
            return resultTime;
        }
        
        public async UniTask UpdateScore(int value)
        {
            var form = new WWWForm();
            form.AddField("value", value);

            await SendRequest(
                requestName: RequestName.UpdateScore,
                requestType: RequestType.Post,
                requestMethod: PatchKey,
                form: form
            );
        }
        
        public async UniTask<bool> IsGameAllowed()
        {
            var result = false;
            await SendRequest(
                requestName: RequestName.IsGameAllowed,
                requestType: RequestType.Get,
                requestMethod: PostKey,
                onSuccess: webRequest =>
                {
                    var successData = ResponseStruct.TryParse<ResponseStruct.StatusStruct>(webRequest.downloadHandler.text);
                    result = successData.ok;
                }
            );

            return result;
        }
        
        public async UniTask<bool> PayGame()
        {            
            var result = false;
            await SendRequest(
                requestName: RequestName.PayGame,
                requestType: RequestType.Get,
                requestMethod: PostKey,
                onSuccess: webRequest =>
                {
                    var successData = ResponseStruct.TryParse<ResponseStruct.StatusStruct>(webRequest.downloadHandler.text);
                    result = successData.ok;
                }
            );
            
            return result;
        }
        
        public async UniTask<bool> RefreshAuthData(string refreshToken, long refreshExpires, Action onSuccessCallback = null)
        {
            var result = false;
            await SendRequest(
                requestName: RequestName.RefreshAuthData,
                requestType: RequestType.Get,
                requestMethod: PostKey,
                onSuccess: webRequest =>
                {
                    var accessTokenData = ResponseStruct.TryParse<ResponseStruct.RefreshAuthStruct>(webRequest.downloadHandler.text).accessToken;
            
                    var loginData = new LoginStruct()
                    {
                        accessToken = new AccessToken()
                        {
                            token = accessTokenData.token,
                            expiresIn = accessTokenData.expiresIn
                        },
                        refreshToken = new RefreshToken()
                        {
                            token = refreshToken,
                            expiresIn = refreshExpires * 1000
                        }
                    };
                    _controller.User.SetSignInData(loginData, false);
                    result = true;
                    
                    onSuccessCallback?.Invoke();
                },
                isUseAuthToken: false,
                isUseRefreshToken: true,
                refreshToken: refreshToken
            );

            return result;
        }
        
        public async UniTask RefreshUserCurrency()
        {
            await SendRequest(
                requestName: RequestName.RefreshUserCurrency,
                requestType: RequestType.Get,
                onSuccess: webRequest =>
                {
                    ArenaGamesController.Instance.User.CurrencyInfo ??= new CurrenciesData();
                    ArenaGamesController.Instance.User.CurrencyInfo.CurrencyInfo = JsonConvert.DeserializeObject<List<CurrencyInfoStruct>>(webRequest.downloadHandler.text);
                }
            );
        }
        
        public async UniTask ProgressAchievement(string achievement, int num)
        {
            var form = new WWWForm();
            form.AddField("operation", "INCREMENT");
            form.AddField("value", num);
            
            await SendRequest(
                requestName: RequestName.ProgressAchievement,
                requestType: RequestType.Post,
                requestMethod: PatchKey,
                form: form,
                uriOverride: AGHelperURIs.ACHIEVEMENTS_PATCH_URI + achievement + "/progress"
            );
        }
        
        public async UniTask GetLeaderboard(string leaderboardId, int offset, Action<ResponseStruct.LeaderboardsStruct> onSuccessCallback)
        {
            await SendRequest(
                requestName: RequestName.GetLeaderboard,
                requestType: RequestType.Get,
                onSuccess: webRequest =>
                {
                    var leaderboardStruct = ResponseStruct.TryParse<ResponseStruct.LeaderboardsStruct>(webRequest.downloadHandler.text);
                    onSuccessCallback?.Invoke(leaderboardStruct);
                },
                uriOverride: $"{AGHelperURIs.LEADERBOARD_URI}{leaderboardId}?aroundPlayerLimit=1&limit={EntriesLimit}&offset={offset}"
            );
        }
        
        public async UniTask GetAchievements(int limit, int offset, Action<ResponseStruct.AchievementsStruct> onSuccessCallback)
        {
            await SendRequest(
                requestName: RequestName.GetLeaderboard,
                requestType: RequestType.Get,
                onSuccess: webRequest =>
                {
                    var leaderboardStruct = ResponseStruct.TryParse<ResponseStruct.AchievementsStruct>(webRequest.downloadHandler.text);
                    onSuccessCallback?.Invoke(leaderboardStruct);
                },
                uriOverride: AGHelperURIs.GET_ACHIEVEMENTS_URI + "?limit=" + limit + "&offset=" + offset
            );
        }
        
        public async UniTask RegisterUser(RegistrationData data, Action onSuccessCallback, Action<string> onErrorCallback)
        {
            var form = new WWWForm();
            form.AddField("email", data.email);
            form.AddField("username", data.username);
            form.AddField("password", data.password);
            
            await SendRequest(
                requestName: RequestName.RegisterUser,
                requestType: RequestType.Post,
                form: form,
                onSuccess: _ => onSuccessCallback?.Invoke(),
                onError: message => onErrorCallback?.Invoke(message),
                isUseAuthToken: false
            );
        }
        
        public async UniTask ConfirmEmail(RegistrationData data, Action onSuccessCallback, Action<string> onErrorCallback)
        {
            var form = new WWWForm();
            form.AddField("email", data.email);
            form.AddField("code", data.code);
            
            await SendRequest(
                requestName: RequestName.ConfirmEmail,
                requestType: RequestType.Post,
                form: form,
                onSuccess: _ => onSuccessCallback?.Invoke(),
                onError: message => onErrorCallback?.Invoke(message),
                isUseAuthToken: false
            );
        }
        
        public async UniTask GetUserData()
        {
            await SendRequest(
                requestName: RequestName.GetUserData,
                requestType: RequestType.Get,
                onSuccess: webRequest =>
                {
                    _controller.User.PlayerInfo = JsonUtility.FromJson<PlayerInfoStruct>(webRequest.downloadHandler.text);
                    ArenaGamesController.Instance.SetNickName(_controller.User.PlayerInfo.username);
                }
            );
        }
        
        public async UniTask SignInUser(LoginData data, Action onSuccessCallback, Action<string> onErrorCallback)
        {
            var form = new WWWForm();
            form.AddField("login", data.username);
            form.AddField("password", data.password);
            
            await SendRequest(
                requestName: RequestName.SignInUser,
                requestType: RequestType.Post,
                form: form,
                onSuccess: webRequest =>
                {
                    _controller.User.AccessInfo = JsonUtility.FromJson<LoginStruct>(webRequest.downloadHandler.text);
                    onSuccessCallback?.Invoke();
                },
                onError: message => onErrorCallback?.Invoke(message),
                isUseAuthToken: false
            );
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
    }
}