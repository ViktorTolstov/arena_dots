using ArenaGames;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class AGHTTPRequestHandler : MonoBehaviour
{
    public static void SendRequest(RegistrationData _Data, UnityAction _OnSuccess, UnityAction<string> _OnError)
    {
        WWWForm _Form = new WWWForm();

        _Form.AddField("email", _Data.email);
        _Form.AddField("username", _Data.username);
        _Form.AddField("password", _Data.password);

        UnityWebRequest _Request = UnityWebRequest.Post(AGHelperURIs.REGISTRATION_URI, _Form);

        _Request.SendWebRequest();

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
}
