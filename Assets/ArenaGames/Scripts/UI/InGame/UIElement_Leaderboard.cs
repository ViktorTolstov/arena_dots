using System.Collections.Generic;
using System.Linq;
using ArenaGames.Network;
using TMPro;

using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_Leaderboard : UIElementBase
    {
        [SerializeField] private LeaderBoardTabBtn _leaderBoardTabBtnPrefab;
        [SerializeField] private Transform _tabsHolder;
        [SerializeField] private Button _previousBtn;
        [SerializeField] private Button _nextBtn;

        public Transform m_LeaderboardsParent;
        public Transform m_LocalLeaderboardParent;
        public GameObject m_LeaderboardEntry;
        private List<GameObject> _leaderboardItems = new List<GameObject>();
        private List<LeaderBoardTabBtn> _currentTabs = new List<LeaderBoardTabBtn>();

        private string _currentLeaderboardAlias;
        private int _currentPage = 0;

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
            
            _nextBtn.onClick.AddListener(OpenNextPage);
            _previousBtn.onClick.AddListener(OpenPreviousPage);
        }

        private void UpdateLeaderboard(string lbAlias)
        {
            _currentPage = 0;
            _currentLeaderboardAlias = lbAlias;
            ArenaGamesController.Instance.NetworkController.GetLeaderboard(lbAlias, _currentPage * AGNetworkController.EntriesLimit, OnLeaderboardsReceived);
            
            foreach (var tab in _currentTabs)
            {
                tab.SetActive(lbAlias == tab.LbAlias);
            }
        }


        private void OnLeaderboardsReceived(ResponseStruct.LeaderboardsStruct leaderboard)
        {
            _previousBtn?.gameObject.SetActive(_currentPage > 0);
            // _nextBtn.gameObject.SetActive(_currentPage > 0);
            
            // TODO: переделать на нормальную фактори
            foreach (var gameObject in _leaderboardItems)
            {
                Destroy(gameObject);
            }

            _leaderboardItems = new List<GameObject>();

            var hasMineComeUp = leaderboard.leaderboards.Any(x => x.profileId == ArenaGamesController.Instance.User.PlayerInfo.id);
            
            if (leaderboard.leaderboards.Count == 0) return;
            
            if (!hasMineComeUp && leaderboard.aroundLeaderboards.Count > 0 && leaderboard.aroundLeaderboards[0].position < leaderboard.leaderboards[0].position)
            {
                CreateItem(leaderboard.aroundLeaderboards[0]);
            }

            foreach (var entry in leaderboard.leaderboards)
            {
                CreateItem(entry);
            }
            
            if (!hasMineComeUp && leaderboard.aroundLeaderboards.Count > 0 && leaderboard.aroundLeaderboards[0].position > leaderboard.leaderboards[^1].position)
            {
                CreateItem(leaderboard.aroundLeaderboards[0]);
            }
        }
        
        private void OpenNextPage()
        {
            _currentPage++;
            ArenaGamesController.Instance.NetworkController.GetLeaderboard(_currentLeaderboardAlias, _currentPage * AGNetworkController.EntriesLimit, OnLeaderboardsReceived);
        }

        private void OpenPreviousPage()
        {
            _currentPage--;
            ArenaGamesController.Instance.NetworkController.GetLeaderboard(_currentLeaderboardAlias, _currentPage * AGNetworkController.EntriesLimit, OnLeaderboardsReceived);
        }

        private void CreateItem(ResponseStruct.Leaderboard leaderboard)
        {
            var obj = Instantiate(m_LeaderboardEntry);
            obj.GetComponent<LeaderboardEntry>().SetupEntry(leaderboard.position.ToString(), leaderboard.username, leaderboard.score.ToString(), "-", leaderboard.profileId == ArenaGamesController.Instance.User.PlayerInfo.id);
            obj.transform.SetParent(m_LeaderboardsParent.transform);
            obj.transform.localScale = Vector3.one;

            _leaderboardItems.Add(obj);
        }
    }
}