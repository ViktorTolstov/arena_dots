using UnityEngine;

namespace ArenaGames.Scripts.Data
{
    public class AGData
    {
        private const string RefreshTokenKey = "RefreshToken";
        private const string RefreshTokenExpireKey = "RefreshTokenExpire";

        public bool TryGetRefreshToken(out string token, out int expiresIn)
        {
            token = "";
            var nowTs = AGTimeController.Timestamp;
            expiresIn = PlayerPrefs.GetInt(RefreshTokenExpireKey);
            if (nowTs > expiresIn) return false;
            
            var isExist = PlayerPrefs.HasKey(RefreshTokenKey);
            if (isExist)
            {
                token = PlayerPrefs.GetString(RefreshTokenKey);
            }
            
            return isExist;
        }

        public void SaveRefreshToken(RefreshToken refreshToken)
        {
            var expiresInTs = (int) (refreshToken.expiresIn / 1000);
            PlayerPrefs.SetString(RefreshTokenKey, refreshToken.token);
            PlayerPrefs.SetInt(RefreshTokenExpireKey, expiresInTs);
            PlayerPrefs.Save();
        }

        public void ClearRefreshToken()
        {
            PlayerPrefs.SetString(RefreshTokenKey, "");
            PlayerPrefs.SetInt(RefreshTokenExpireKey, 0);
            PlayerPrefs.Save();
        }
    }
}