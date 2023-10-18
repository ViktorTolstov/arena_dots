namespace ArenaGames
{
    [System.Serializable]
    public abstract class Token
    {
        public string token;
        public long expiresIn;
    }
    
    [System.Serializable]
    public class AccessToken : Token
    {
        
    }

    [System.Serializable]
    public class RefreshToken : Token
    {
        
    }

    [System.Serializable]
    public class RefreshTokenStruct
    {
        public AccessToken accessToken;
    }

    [System.Serializable]
    public class LoginStruct
    {
        public AccessToken accessToken;
        public RefreshToken refreshToken;
    }

    [System.Serializable]
    public class RegistrationData
    {
        public string email;
        public string username;
        public string password;
        public string code;
    }

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }
}