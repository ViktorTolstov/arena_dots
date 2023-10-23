using System.Collections.Generic;
using ArenaGames.EventServer;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_AccountPanel : UIElementBase
    {
        public Transform m_AchievementsParent;
        public GameObject m_AchievementsEntry;

        public UIElementBase m_SocialPlatforms;

        public Button m_BtnSocialPlatforms;
        public Button m_BtnFAQ;
        public Button m_BtnCustomSupport;
        public Button m_BtnDeleteAccount;
        public Button m_LogOutBtn;

        private List<GameObject> m_AddedMissions = new List<GameObject>();

        private void OnEnable()
        {
            m_BtnSocialPlatforms.onClick.AddListener(OnSocialBtnClicked);
            m_BtnFAQ.onClick.AddListener(() => OpenURL("https://arenavs.com/about"));
            m_BtnCustomSupport.onClick.AddListener(() => OpenURL("https://arenavs.com/about"));
            m_BtnDeleteAccount.onClick.AddListener(() => OpenURL("https://arenavs.com/deleteaccount"));
            m_LogOutBtn.onClick.AddListener(OnLogoutClicked);

            /*ArenaGamesController.Instance.NetworkControllerOld.GetAchievements("100", "0", SetupAchievements);
            ArenaGamesController.Instance.NetworkControllerOld.ProgressAchievement("AM_Loyal", 1);*/
        }

        private void OnDisable()
        {
            m_BtnSocialPlatforms.onClick.RemoveListener(OnSocialBtnClicked);
            m_BtnFAQ.onClick.RemoveAllListeners();
            m_BtnCustomSupport.onClick.RemoveAllListeners();
            m_BtnDeleteAccount.onClick.RemoveAllListeners();
            m_LogOutBtn.onClick.RemoveListener(OnLogoutClicked);
        }

        private void SetupAchievements(AchievementsStruct _Achievements)
        {
            foreach (GameObject _Obj in m_AddedMissions)
            {
                Destroy(_Obj);
            }

            m_AddedMissions = new List<GameObject>();

            foreach (AchievementEntryStruct _Achievement in _Achievements.docs)
            {
                GameObject _Obj = Instantiate(m_AchievementsEntry);
                m_AddedMissions.Add(_Obj);

                _Obj.GetComponent<AchievementsEntry>().Setup(_Achievement.name, _Achievement.description, _Achievement.completed);
                _Obj.transform.SetParent(m_AchievementsParent);
                _Obj.transform.localScale = Vector3.one;
            }
        }

        private void OnLogoutClicked()
        {
            PlayerPrefs.SetString("Username", "");
            PlayerPrefs.SetString("Password", "");

            ArenaGamesController.Instance.EventServerController.ScheduleEvent(AGEventServerController.EventType.Logout);
            ArenaGamesController.Instance.EventServerController.SetOnline(false);
            ArenaGamesController.Instance.PlayerData.ClearRefreshToken();

            ArenaGamesController.Instance.StartProcess();
            Destroy(m_InGameUIController.gameObject);
        }

        private void OnSocialBtnClicked()
        {
            m_SocialPlatforms.Open();
        }

        private void OpenURL(string _URL)
        {
            Application.OpenURL(_URL);
        }
    }
}