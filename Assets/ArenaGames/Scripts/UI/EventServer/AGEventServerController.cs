using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArenaGames.EventServer
{
    public class AGEventServerController
    {
        public enum EventType
        {
            Login,
            Logout,
            Online,
            EndGame,
            Connect,
            Disconnect,
            CollapseApp,
            ExpandApp,
            
            // TODO: for tests only
            DestroyDot
        }
        
        private const string LoginEvent = "login";
        private const string LogoutEvent = "logout";
        private const string OnlineEvent = "online";
        private const string EndGameEvent = "end-game";
        private const string ConnectEvent = "connect";
        private const string DisconnectEvent = "disconnect";
        private const string CollapseAppEvent = "collapse-app";
        private const string ExpandAppEvent = "expand-app";

        private Dictionary<EventType, string> _eventRefs = new Dictionary<EventType, string>()
        {
            {EventType.Login, LoginEvent},
            {EventType.Logout, LogoutEvent},
            {EventType.Online, OnlineEvent},
            {EventType.EndGame, EndGameEvent},
            {EventType.Connect, ConnectEvent},
            {EventType.Disconnect, DisconnectEvent},
            {EventType.CollapseApp, CollapseAppEvent},
            {EventType.ExpandApp, ExpandAppEvent},
            {EventType.DestroyDot, "destroy-dot"},
        };

        private ArenaGamesController _controller;
        private List<string> _scheduledEvents = new List<string>();
        private bool _isOnline;
        private bool _isPaused;

        private bool IsActive => _isOnline && !_isPaused;

        public void Setup(ArenaGamesController controller)
        {
            _controller = controller;
            
            StartSendProcess();
            AddOnlineEvent();
        }
        
        public void ScheduleEvent(EventType eventType, bool isForce = false)
        {
            if (!_eventRefs.ContainsKey(eventType))
            {
                Debug.LogError("AGEventServerController:: unknown event " + eventType);
                return;
            }

            var targetEvent = _eventRefs[eventType];

            if (isForce)
            {
                SendEvent(targetEvent);
            } 
            else
            {
                _scheduledEvents.Add(targetEvent);
            }
        }

        private async void StartSendProcess()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(4f));

                if (_scheduledEvents.Count == 0 || !IsActive) continue;
                
                // TODO: make work with few events
                var targetEvent = _scheduledEvents[0];
                _scheduledEvents.RemoveAt(0);
                SendEvent(targetEvent);
            }
        }

        private async void AddOnlineEvent()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
                if (!IsActive) continue;
                ScheduleEvent(EventType.Online);
            }
        }

        private void SendEvent(string targetEvent)
        {
            _controller.NetworkController.SendEventServerEvent(targetEvent);
        }

        public void SetOnline(bool isOnline)
        {
            _isOnline = isOnline;
        }
        
        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}