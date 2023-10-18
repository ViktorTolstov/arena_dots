using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace ArenaGames
{
    public class UIElement_Leaderboard : UIElementBase
    {
        [SerializeField] private LeaderBoardTabBtn _leaderBoardTabBtnPrefab;
        
        public Transform m_LeaderboardsParent;
        public Transform m_LocalLeaderboardParent;

        public GameObject m_LeaderboardEntry;

        private List<GameObject> m_AddedGameObjects = new List<GameObject>();
        
        private List<LeaderBoardTabBtn> _activeLeaderBoardTabBtns = new List<LeaderBoardTabBtn>();

        private void OnEnable()
        {
            ArenaGamesController.Instance.NetworkControllerOld.GetLeaderboard(AGCore.Settings.LeaderboardId, OnLeaderboardsReceived);
        }

        private void OnLeaderboardsReceived(LeaderboardsStruct _Leaderboard)
        {
            foreach (var gameObject in m_AddedGameObjects)
            {
                Destroy(gameObject);
            }

            m_AddedGameObjects = new List<GameObject>();

            bool _HasMineComeUp = false;

            for (int i = 0; i < _Leaderboard.leaderboards.Count; i++)
            {
                GameObject _Obj = Instantiate(m_LeaderboardEntry);
                _Obj.GetComponent<LeaderboardEntry>().SetupEntry(_Leaderboard.leaderboards[i].position.ToString(), _Leaderboard.leaderboards[i].username, _Leaderboard.leaderboards[i].score.ToString(), "-", _Leaderboard.leaderboards[i].profileId == ArenaGamesController.Instance.User.PlayerInfo.id);
                _Obj.transform.SetParent(m_LeaderboardsParent.transform);
                _Obj.transform.localScale = Vector3.one;

                m_AddedGameObjects.Add(_Obj);

                if (_Leaderboard.leaderboards[i].profileId == ArenaGamesController.Instance.User.PlayerInfo.id)
                {
                    _HasMineComeUp = true;
                }
            }

            if (!_HasMineComeUp)
            {
                if (_Leaderboard.aroundLeaderboards.Count > 0)
                {
                    GameObject _Obj = Instantiate(m_LeaderboardEntry);
                    _Obj.GetComponent<LeaderboardEntry>().SetupEntry(_Leaderboard.aroundLeaderboards[0].position.ToString(), _Leaderboard.aroundLeaderboards[0].username, _Leaderboard.aroundLeaderboards[0].score.ToString(), "-", true);
                    _Obj.transform.SetParent(m_LocalLeaderboardParent.transform);
                    _Obj.transform.localScale = Vector3.one;

                    m_AddedGameObjects.Add(_Obj);
                }
            }
        }
    }
}