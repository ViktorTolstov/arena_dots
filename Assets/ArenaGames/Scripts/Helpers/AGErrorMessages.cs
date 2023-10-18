using System.Collections.Generic;

public static class AGErrorMessages
{
    public static Dictionary<long, string> ERROR_MESSAGES_SIGNIN;
    public static Dictionary<long, string> ERROR_MESSAGES_REGISTRATION;
    public static Dictionary<long, string> ERROR_MESSAGES_CONFIRM_EMAIL;

    public static string CONNECTION_ERROR = "Connection error. Please check your network";

    public static void Initialize()
    {
        ERROR_MESSAGES_SIGNIN = new Dictionary<long, string>()
        {
            {-1, "Unexpected error, please try again" },
            { 400, "Invalid parameters passed."},
            { 401, "Wrong username or password"}
        };

        ERROR_MESSAGES_REGISTRATION = new Dictionary<long, string>()
        {
            {-1, "Unexpected error, please try again" },
            {400, "Invalid parameters passed."},
            {401, "Wrong username or password"},
            {409, "Email or Password is already used."}
        };

        ERROR_MESSAGES_CONFIRM_EMAIL = new Dictionary<long, string>()
        {
            {-1, "Unexpected error, please try again" },
            {400, "Invalid parameters passed."},
            {401, "Wrong username or password"},
            {404, "This profile doesn't exists" },
            {409, "The code has already been used"},
        };
    }

    public static string GetErrorMessage(bool _IsConnectionError, long _ErrorCode = 0, Dictionary<long, string> _Dictionary = null)
    {
        if (_IsConnectionError)
            return CONNECTION_ERROR;
        else
        {
            if (_Dictionary.ContainsKey(_ErrorCode))
                return _Dictionary[_ErrorCode];
            else
                return _Dictionary[-1];
        }
    }
}
