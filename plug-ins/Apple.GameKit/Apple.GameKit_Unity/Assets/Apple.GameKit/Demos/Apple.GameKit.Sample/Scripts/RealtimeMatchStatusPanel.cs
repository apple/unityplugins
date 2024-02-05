using System;
using System.Collections.Generic;
using System.Text;
using Apple.Core.Runtime;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using static Apple.GameKit.Multiplayer.GKMatchDelegate;

namespace Apple.GameKit.Sample
{
    using GKMatchProperties = NSDictionary<NSString, NSObject>;

    public class RealtimeMatchStatusPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _playerStatusPanelPrefab = default;
        [SerializeField] private GameObject _playerListContent = default;
        [SerializeField] private Text _messageLogText = default;
        [SerializeField] private Text _matchStatusTitleText = default;
        [SerializeField] private GameObject _matchButtonArea = default;
        [SerializeField] private GameObject _inviteButtonArea = default;
        [SerializeField] private GameObject _startGameButtonArea = default;

        public void Populate(GKMatch match) => Populate(
            GKPlayerConnectionState.Connected,
            match: match);

        public void Populate(GKMatchRequest request, GKMatch match, bool showStartGameButton) => Populate(
            GKPlayerConnectionState.Connected,
            request: request,
            match: match,
            showStartGameButton: showStartGameButton);

        public void Populate(GKMatchRequest request, GKMatchedPlayers matchedPlayers) => Populate(
            GKPlayerConnectionState.Unknown,
            request: request,
            matchedPlayers: matchedPlayers);

        public void Populate(GKMatchRequest request, NSArray<GKPlayer> players) => Populate(
            GKPlayerConnectionState.Unknown,
            request: request,
            players: players);

        private void Populate(
            GKPlayerConnectionState initialConnectionState,
            GKMatchRequest request = null,
            GKMatch match = null,
            bool showStartGameButton = false,
            GKMatchedPlayers matchedPlayers = null,
            NSArray<GKPlayer> players = null)
        {
            Clear();

            MatchRequest = request;
            Match = match;
            MatchedPlayers = matchedPlayers;

            players ??= match?.Players ?? matchedPlayers?.Players;
            var playerProperties = match?.PlayerProperties ?? matchedPlayers?.PlayerProperties ?? request?.RecipientProperties;
            var localPlayerProperties = match?.Properties ?? matchedPlayers?.Properties ?? request?.Properties;

            // Add the local player first.
            AddOrUpdatePlayerPanel(GKLocalPlayer.Local, initialConnectionState, localPlayerProperties);

            // Add remote players last.
            if (players != null)
            {
                foreach (var player in players)
                {
                    GKMatchProperties matchProperties = null;
                    playerProperties?.TryGetValue(player, out matchProperties);

                    // All players in the list are in the connected state.
                    AddOrUpdatePlayerPanel(player, initialConnectionState, matchProperties);
                }
            }

            if (match != null)
            {
                match.Delegate.DidFailWithError += OnDidFailWithError;
                match.Delegate.DataReceived += OnDataReceived;
                match.Delegate.DataReceivedForPlayer += OnDataReceivedForPlayer;
                match.Delegate.PlayerConnectionChanged += OnPlayerConnectionChanged;
            }

            _matchStatusTitleText.gameObject.SetActive(Match != null);
            _startGameButtonArea.SetActive(Match != null && showStartGameButton);
            _matchButtonArea.SetActive(Match != null && !showStartGameButton);
            _inviteButtonArea.SetActive(MatchRequest != null && MatchedPlayers?.Players?.Count > 0);
        }

        public void Clear()
        {
            var match = Match;
            if (match != null)
            {
                match.Delegate.DidFailWithError -= OnDidFailWithError;
                match.Delegate.DataReceived -= OnDataReceived;
                match.Delegate.DataReceivedForPlayer -= OnDataReceivedForPlayer;
                match.Delegate.PlayerConnectionChanged -= OnPlayerConnectionChanged;
            }

            // clear the previous list of players
            foreach (Transform transform in _playerListContent.transform)
            {
                Destroy(transform.gameObject);
            }            
            _playerListContent.transform.DetachChildren();
            _playerStatusPanels.Clear();

            _matchStatusTitleText.gameObject.SetActive(false);
            _matchButtonArea.SetActive(false);
            _inviteButtonArea.SetActive(false);

            Match = null;
            MatchRequest = null;
            MatchedPlayers = null;
            IsRequestingMatch = false;
        }

        public GKMatch Match { get; private set; }
        public GKMatchRequest MatchRequest { get; private set; }
        public GKMatchedPlayers MatchedPlayers { get; private set; }
        public bool IsRequestingMatch { get; private set; }

        private Dictionary<GKPlayer, PlayerStatusPanel> _playerStatusPanels = new Dictionary<GKPlayer, PlayerStatusPanel>();

        private PlayerStatusPanel AddOrUpdatePlayerPanel(GKPlayer player, GKPlayerConnectionState connectionState, GKMatchProperties matchProperties = null)
        {
            if (_playerStatusPanels.TryGetValue(player, out var panel))
            {
                // Existing player: just update the existing panel.
                panel.ConnectionState = connectionState;
            }
            else
            {
                // New player: create new panel.
                var panelObject = Instantiate(_playerStatusPanelPrefab, _playerListContent.transform, worldPositionStays: false);

                panel = panelObject.GetComponent<PlayerStatusPanel>();
                panel.Player = player;
                panel.ConnectionState = connectionState;

                panel.MatchProperties = matchProperties;

                _playerStatusPanels[player] = panel;
            }

            return panel;
        }

        private void OnDidFailWithError(GameKitException exception)
        {
            // todo do something with this
            Debug.LogError(exception);
        }

        private void OnPlayerConnectionChanged(GKPlayer player, GKPlayerConnectionState connectionState)
        {
            GKMatchProperties matchProperties = null;
            if (connectionState == GKPlayerConnectionState.Connected)
            {
                // get player properties for this new player
                Match?.PlayerProperties?.TryGetValue(player, out matchProperties);
            }

            AddOrUpdatePlayerPanel(player, connectionState, matchProperties);
        }

        private void OnDataReceived(byte[] data, GKPlayer fromPlayer)
        {
            AppendMessageToLog($"{DateTime.Now} {data.Length} bytes from {fromPlayer.DisplayName}");
        }

        private void OnDataReceivedForPlayer(byte[] data, GKPlayer forPlayer, GKPlayer fromPlayer)
        {
            AppendMessageToLog($"{DateTime.Now} {data.Length} bytes for {forPlayer.DisplayName} from {fromPlayer.DisplayName}");
        }

        private void AppendMessageToLog(string message)
        {
            string messageLog = _messageLogText.text;
            if (string.IsNullOrEmpty(messageLog))
            {
                messageLog = message;
            }
            else
            {
                // keep the last N messages
                const int LinesToKeep = 5;

                int numLines, index;
                for (numLines = 1, index = messageLog.Length; 
                    numLines < LinesToKeep && index > 0; 
                    numLines++, index = messageLog.LastIndexOf('\n', index - 1))
                {
                }

                if (index > 0)
                {
                    messageLog = messageLog.Substring(index + 1);
                }

                messageLog += '\n' + message;
            }

            _messageLogText.text = messageLog;
        }

        private int _broadcastNumber = 0;
        public void Broadcast()
        {
            // Send some data...
            Match?.Send(Encoding.UTF8.GetBytes($"Broadcast {_broadcastNumber++}"), GKMatch.GKSendDataMode.Reliable);
        }

        public void Disconnect()
        {
            Match?.Disconnect();
            Match = null;
            GameKitSample.PopPanel();
        }

        public void Invite()
        {
            if (MatchRequest != null && MatchedPlayers != null)
            {
                IsRequestingMatch = true;
                GameKitSample.PopPanel();
            }
        }

        public void FinishMatchmaking()
        {
            if (Match != null)
            {
                GKMatchmaker.Shared?.FinishMatchmaking(Match);
                _startGameButtonArea.SetActive(false);
                _matchButtonArea.SetActive(true);
            }
        }
    }
}
