using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ArenaGames.Network
{
    public class AGNetworkController
    {
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
    }
}