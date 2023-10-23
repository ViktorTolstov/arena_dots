using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ArenaGames.Network
{
    public class AGNetworkController
    {
        private ArenaGamesController _controller;
        public void Setup(ArenaGamesController controller)
        {
            _controller = controller;
        }
        
        public async UniTask UpdateGameData()
        {
            var request = UnityWebRequest.Get(AGHelperURIs.GAME_INFO_API);

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to get data. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var gameData = ResponseStruct.TryParse<ResponseStruct.GameInfoStruct>(request.downloadHandler.text);
                    ArenaGamesController.Instance.SetGameData(gameData);
                    break;
            }
        }
        
        public async UniTask SendEventServerEvent(string eventName)
        {
            Debug.Log("SendEventServerEvent:: " + eventName);

            var form = new WWWForm();

            form.AddField("gameId", AGCore.Settings.GameName);
            form.AddField("userId", ArenaGamesController.Instance.User.PlayerInfo.username);
            form.AddField("eventType", eventName);
            
            var request = UnityWebRequest.Post(AGHelperURIs.EVENT_SERVER_URI, form);

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to send events. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("SendResult: " + request.downloadHandler.text);
                    break;
            }
        }
        
        public async UniTask ResendCode(string email)
        {
            Debug.Log("ResendCode");

            var form = new WWWForm();

            form.AddField("email", email);
            
            var request = UnityWebRequest.Post(AGHelperURIs.RESEND_CONFIRM_CODE, form);

            request.SetRequestHeader("accept", "application/json");

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to send events. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("SendResult: " + request.downloadHandler.text);
                    break;
            }
        }

        public async UniTask<long> GetResetData(string email)
        {
            var form = new WWWForm();

            form.AddField("email", email);
            
            var request = UnityWebRequest.Post(AGHelperURIs.GET_NEXT_TRY_FOR_RESEND_CODE, form);
            request.method = "GET";

            request.SetRequestHeader("accept", "application/json");

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            long resultTime = 0;
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to sed events. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var codeData = ResponseStruct.TryParse<ResponseStruct.CodeAttemptResponseStruct>(request.downloadHandler.text);
                    resultTime = long.Parse(codeData.nextAttemptTime) / 1000;
                    break;
            }

            return resultTime;
        }
        
        public async UniTask UpdateScore(int value)
        {
            var form = new WWWForm();

            form.AddField("value", value);
            
            var request = UnityWebRequest.Post(AGHelperURIs.UPDATE_SCORE, form);
            request.method = "PATCH";

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to update score. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Update score success: " + request.downloadHandler.text);
                    break;
            }
        }
        
        public async UniTask<bool> IsGameAllowed()
        {
            var request = UnityWebRequest.Get(AGHelperURIs.IS_GAME_ALLOWED);
            request.method = "Post";

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to update score. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var successData = ResponseStruct.TryParse<ResponseStruct.StatusStruct>(request.downloadHandler.text);
                    return successData.ok;
            }

            return false;
        }
        
        public async UniTask PayGame()
        {
            var request = UnityWebRequest.Get(AGHelperURIs.PAY_GAME);
            request.method = "Post";

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("access-token", ArenaGamesController.Instance.User.AccessInfo.accessToken.token);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to pay game. Error message: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Success pay: " + request.downloadHandler.text);
                    break;
            }
        }
        
        public async UniTask<bool> RefreshAuthData(string refreshToken, long refreshExpires)
        {
            Debug.Log("RefreshAuthData");
            
            var request = UnityWebRequest.Get(AGHelperURIs.REFRESH_TOKEN_URI);
            request.method = "Post";

            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("refresh-token", refreshToken);

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Failed to auth: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Success auth with refresh token");
                    var accessTokenData = ResponseStruct.TryParse<ResponseStruct.RefreshAuthStruct>(request.downloadHandler.text).accessToken;
            
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
                    return true;
            }
            
            return false;
        }
    }
}