using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace ArenaGames
{
    public class UIElement_Leaderboard : UIElementBase
    {
        [SerializeField] private LeaderBoardTabBtn _leaderBoardTabBtnPrefab;
        [SerializeField] private Transform _tabsHolder;
        
        public Transform m_LeaderboardsParent;
        public Transform m_LocalLeaderboardParent;
        public GameObject m_LeaderboardEntry;
        private List<GameObject> m_AddedGameObjects = new List<GameObject>();
        private List<LeaderBoardTabBtn> _currentTabs = new List<LeaderBoardTabBtn>();

        private string _currentLeaderboardAlias;

        public void OnEnable()
        {
            UpdateLeaderboard(_currentLeaderboardAlias);
        }

        public void Setup()
        {
            var leaderBoards = ArenaGamesController.Instance.GameData.activeLeaderboards;
            if (leaderBoards.Count == 0)
            {
                Debug.LogError("UIElement_Leaderboard:: there are no any active leaderboards for game");
            }
            
            if (leaderBoards.Count > 1)
            {
                foreach (var lb in leaderBoards)
                {
                    var newTab = Instantiate(_leaderBoardTabBtnPrefab, _tabsHolder);
                    newTab.Setup(lb.alias, lb.name, UpdateLeaderboard);
                    newTab.SetActive(leaderBoards[0].name == lb.name);
                    _currentTabs.Add(newTab);
                }
            }

            UpdateLeaderboard(leaderBoards[0].alias);
        }

        private void UpdateLeaderboard(string lbAlias)
        {
            _currentLeaderboardAlias = lbAlias;
            ArenaGamesController.Instance.NetworkControllerOld.GetLeaderboard(lbAlias, OnLeaderboardsReceived);
            
            foreach (var tab in _currentTabs)
            {
                tab.SetActive(lbAlias == tab.LbAlias);
            }
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