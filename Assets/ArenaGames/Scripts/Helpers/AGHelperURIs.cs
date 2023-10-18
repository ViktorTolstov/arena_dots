using UnityEngine;

namespace ArenaGames
{
    public static class AGHelperURIs
    {
        private const string API_ADDRESS = "[API_ADDRESS]";
        private const string CLIENT_ADDRESS = "[CLIENT_ADDRESS]";
        private const string GAME_ALIAS = "[GAME_ALIAS]";
        
        private static string STAGE_ADDRESS = "https://stage.arenagames.api.ldtc.space";
        private static string PROD_ADDRESS = "https://api.arenavs.com";
        
        private static string STAGE_ADDRESS_CLIENT = "https://stage.arenagames.test.ldtc.space";
        private static string PROD_ADDRESS_CLIENT = "https://arenavs.com";
        
        public static string WEBVIEW_AUTH_URI = $"{CLIENT_ADDRESS}/auth/sign-in";
        
        public static string AUTH_URI = $"{API_ADDRESS}/api/v2/gamedev/client/auth/sign-in";
        public static string REFRESH_TOKEN_URI = $"{API_ADDRESS}/api/v2/gamedev/client/auth/refresh-token";
        public static string CLIENT_PROFILE_URI = $"{API_ADDRESS}/api/v2/gamedev/client/my-profile";

        public static string GAME_INFO_API = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}";
        public static string REGISTRATION_URI = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/user-registration";
        public static string CONFIRM_EMAIL_URI = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/user-registration/confirmation-email";
        public static string CURRENCY_URI = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/wallet";
        public static string LEADERBOARD_URI = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/leaderboard/";
        public static string LEADERBOARD_POST = $"{API_ADDRESS}/api/v2/gamedev/server/{GAME_ALIAS}/leaderboard/";
        public static string GET_ACHIEVEMENTS_URI = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/achievement";
        public static string ACHIEVEMENTS_POST_URI = $"{API_ADDRESS}/api/v2/gamedev/server/{GAME_ALIAS}/achievement/";
        public static string RESEND_CONFIRM_CODE = $"{API_ADDRESS}/api/v2/gamedev/client/{GAME_ALIAS}/user-registration/next-attempt-send-code/";
        
        public static string EVENT_SERVER_URI = "https://es1.arenavs.com/game-events";
        
        public static void UpdateUris(bool isProd, string gameAlias)
        {
            var clientAddress = isProd ? PROD_ADDRESS_CLIENT : STAGE_ADDRESS_CLIENT;
            var apiAddress = isProd ? PROD_ADDRESS : STAGE_ADDRESS;
            
            WEBVIEW_AUTH_URI = WEBVIEW_AUTH_URI
                .Replace(CLIENT_ADDRESS, clientAddress);

            AUTH_URI = AUTH_URI
                .Replace(API_ADDRESS, apiAddress);
            REFRESH_TOKEN_URI = REFRESH_TOKEN_URI
                .Replace(API_ADDRESS, apiAddress);
            CLIENT_PROFILE_URI = CLIENT_PROFILE_URI
                .Replace(API_ADDRESS, apiAddress);
            
            GAME_INFO_API = GAME_INFO_API
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            REGISTRATION_URI = REGISTRATION_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            CONFIRM_EMAIL_URI = CONFIRM_EMAIL_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            CURRENCY_URI = CURRENCY_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            LEADERBOARD_URI = LEADERBOARD_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            LEADERBOARD_POST = LEADERBOARD_POST
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            GET_ACHIEVEMENTS_URI = GET_ACHIEVEMENTS_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            ACHIEVEMENTS_POST_URI = ACHIEVEMENTS_POST_URI
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
            RESEND_CONFIRM_CODE = RESEND_CONFIRM_CODE
                .Replace(API_ADDRESS, apiAddress)
                .Replace(GAME_ALIAS, gameAlias);
        }
    }
}