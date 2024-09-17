using System;
using System.Collections.Generic;
using System.Text;
using Apple.Core;
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
        [SerializeField] private MatchPlayerButton _playerStatusButtonPrefab = default;
        [SerializeField] private GameObject _playerListContent = default;
        [SerializeField] private Text _matchStatusTitleText = default;
        [SerializeField] private GameObject _matchButtonArea = default;
        [SerializeField] private GameObject _addPlayersButtonArea = default;
        [SerializeField] private GameObject _inviteButtonArea = default;
        [SerializeField] private GameObject _startGameButtonArea = default;

        [SerializeField] private MessageLog _messageLog = default;

        private readonly bool IsViewControllerAvailableForPlayer = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithPlayer));
        private readonly bool IsRuleBasedMatchmakingAvailable = Availability.IsPropertyAvailable<GKMatchRequest>(nameof(GKMatchRequest.QueueName));

        public void Start()
        {
            _messageLog.LinesToKeep = 5;
        }

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
            var playerProperties = IsRuleBasedMatchmakingAvailable ? (match?.PlayerProperties ?? matchedPlayers?.PlayerProperties ?? request?.RecipientProperties) : null;
            var localPlayerProperties = IsRuleBasedMatchmakingAvailable ? (match?.Properties ?? matchedPlayers?.Properties ?? request?.Properties) : null;

            // Add the local player first.
            AddOrUpdatePlayerButton(GKLocalPlayer.Local, initialConnectionState, localPlayerProperties);

            // Add remote players last.
            if (players != null)
            {
                foreach (var player in players)
                {
                    GKMatchProperties matchProperties = null;
                    playerProperties?.TryGetValue(player, out matchProperties);

                    // All players in the list are in the connected state.
                    AddOrUpdatePlayerButton(player, initialConnectionState, matchProperties);
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
            _addPlayersButtonArea.SetActive(_matchButtonArea.activeSelf && MatchRequest != null);
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
            _playerStatusButtons.Clear();

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

        private Dictionary<GKPlayer, MatchPlayerButton> _playerStatusButtons = new Dictionary<GKPlayer, MatchPlayerButton>();

        private MatchPlayerButton AddOrUpdatePlayerButton(GKPlayer player, GKPlayerConnectionState connectionState, GKMatchProperties matchProperties = null)
        {
            if (_playerStatusButtons.TryGetValue(player, out var button))
            {
                // Existing player: just update the existing button.
                button.ConnectionState = connectionState;
            }
            else
            {
                // New player: create new button.
                button = Instantiate(_playerStatusButtonPrefab, _playerListContent.transform, worldPositionStays: false);
                button.Player = player;
                button.ConnectionState = connectionState;

                if (IsRuleBasedMatchmakingAvailable)
                {
                    button.MatchProperties = matchProperties;
                }

                if (IsViewControllerAvailableForPlayer)
                {
                    button.ButtonClick += async (sender, args) =>
                    {
                        var viewController = GKGameCenterViewController.InitWithPlayer(player);
                        await viewController.Present();
                    };
                }

                _playerStatusButtons[player] = button;
            }

            return button;
        }

        private void OnDidFailWithError(GameKitException exception)
        {
            // todo do something with this
            Debug.LogError(exception);
        }

        private void OnPlayerConnectionChanged(GKPlayer player, GKPlayerConnectionState connectionState)
        {
            GKMatchProperties matchProperties = null;
            if (IsRuleBasedMatchmakingAvailable && connectionState == GKPlayerConnectionState.Connected)
            {
                // get player properties for this new player
                Match?.PlayerProperties?.TryGetValue(player, out matchProperties);
            }

            AddOrUpdatePlayerButton(player, connectionState, matchProperties);
        }

        private void OnDataReceived(byte[] data, GKPlayer fromPlayer)
        {
            _messageLog.AppendMessageToLog($"{DateTime.Now} {data.Length} bytes from {fromPlayer.DisplayName}");
        }

        private void OnDataReceivedForPlayer(byte[] data, GKPlayer forPlayer, GKPlayer fromPlayer)
        {
            _messageLog.AppendMessageToLog($"{DateTime.Now} {data.Length} bytes for {forPlayer.DisplayName} from {fromPlayer.DisplayName}");
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
            GameKitSample.Instance.PopPanel();
        }

        public void Invite()
        {
            if (MatchRequest != null && MatchedPlayers != null)
            {
                IsRequestingMatch = true;
                GameKitSample.Instance.PopPanel();
            }
        }

        public void FinishMatchmaking()
        {
            if (Match != null)
            {
                GKMatchmaker.Shared?.FinishMatchmaking(Match);
                _startGameButtonArea.SetActive(false);
                _matchButtonArea.SetActive(true);
                _addPlayersButtonArea.SetActive(MatchRequest != null);
            }
        }

        public async void AddPlayers()
        {
            if (MatchRequest != null && Match != null)
            {
                // Create a new match request, copying properties from the original one.
                var matchRequest = GKMatchRequest.Init();
                matchRequest.MaxPlayers = MatchRequest.MaxPlayers;
                matchRequest.MinPlayers = MatchRequest.MinPlayers;
                matchRequest.PlayerGroup = MatchRequest.PlayerGroup;
                matchRequest.PlayerAttributes = MatchRequest.PlayerAttributes;
                matchRequest.InviteMessage = MatchRequest.InviteMessage;
                if (IsRuleBasedMatchmakingAvailable)
                {
                    matchRequest.QueueName = MatchRequest.QueueName;
                    matchRequest.Properties = MatchRequest.Properties;
                    matchRequest.RecipientProperties = MatchRequest.RecipientProperties;
                }
                matchRequest.DefaultNumberOfPlayers = MatchRequest.DefaultNumberOfPlayers;

                // When adding new players, we don't want to reuse the previous recipient list, if any.
                // Doing so would result in the original players receiving another invitation.
                matchRequest.Recipients = null;

                await GKMatchmakerViewController.AddPlayersToMatch(matchRequest, Match);
            }
        }
    }
}
