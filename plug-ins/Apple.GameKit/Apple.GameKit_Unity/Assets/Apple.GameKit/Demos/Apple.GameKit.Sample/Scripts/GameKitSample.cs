using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[assembly:Preserve]

namespace Apple.GameKit.Sample
{
    public class GameKitSample : MonoBehaviour
    {
        [SerializeField] private RawImage _playerPhotoImage = default;

        [SerializeField] private Text _playerDisplayName = default;
        [SerializeField] private Text _isMultiplayerGamingRestrictedText = default;
        [SerializeField] private Text _isPersonalizedCommunicationRestrictedText = default;
        [SerializeField] private Text _isUnderageText = default;

        [SerializeField] private GameObject _panelArea = default;

        [SerializeField] private GameObject _mainButtonLayout = default;
        [SerializeField] private AccessPointPanel _accessPointPanel = default;
        [SerializeField] private FriendsPanel _friendsPanel = default;
        [SerializeField] private AchievementsPanel _achievementsPanel = default;
        [SerializeField] private NearbyPlayersPanel _nearbyPlayersPanel = default;
        [SerializeField] private RealtimeMatchRequestPanel _realtimeMatchRequestPanel = default;
        [SerializeField] private RealtimeMatchStatusPanel _realtimeMatchStatusPanel = default;

        [SerializeField] private Button _authenticateBtn = default;
        [SerializeField] private Text _authenticateBtnText = default;

        [SerializeField] private Button _accessPointButton = default;
        [SerializeField] private Button _friendsButton = default;
        [SerializeField] private Button _showAchievementsBtn = default;
        [SerializeField] private Button _nearbyPlayersButton = default;
        [SerializeField] private Button _showTurnBasedMatchesBtn = default;
        [SerializeField] private Button _takeTurnButton = default;
        [SerializeField] private Button _endMatchWinnerButton = default;
        [SerializeField] private Button _reportLeaderboardScore = default;
        [SerializeField] private Button _realtimeMatchmakingButton = default;

        private GKLocalPlayer _localPlayer;
        private GKTurnBasedMatch _activeMatch;

        private void Start()
        {
            try
            {
                // Send Unity log messages to NSLog.
                _ = new AppleLogger();

                _authenticateBtn.onClick.AddListener(OnAuthenticate);
                _accessPointButton.onClick.AddListener(OnShowAccessPointPanel);
                _friendsButton.onClick.AddListener(OnShowFriendsPanel);
                _showAchievementsBtn.onClick.AddListener(OnShowAchievements);
                _nearbyPlayersButton.onClick.AddListener(OnShowNearbyPlayersPanel);
                _showTurnBasedMatchesBtn.onClick.AddListener(OnShowTurnBasedMatches);
                _takeTurnButton.onClick.AddListener(OnTakeTurn);
                _endMatchWinnerButton.onClick.AddListener(OnEndMatchWinner);
                _reportLeaderboardScore.onClick.AddListener(OnReportLeaderboardScore);
                _realtimeMatchmakingButton.onClick.AddListener(OnRealtimeMatchmaking);

                foreach (var btn in _mainButtonLayout.GetComponentsInChildren<Button>())
                {
                    if (btn != _authenticateBtn)
                    {
                        btn.interactable = false;
                    }
                    else
                    {
                        btn.interactable = true;
                    }
                }

                GKTurnBasedMatch.TurnEventReceived += OnMatchTurnEnded;
                GKTurnBasedMatch.MatchEnded += OnMatchEnded;

                GKInvite.InviteAccepted += OnInviteAccepted;

                // Hide all of the interchangeable panels to start.
                for (int i = 0; i < _panelArea.transform.childCount; i++)
                {
                    _panelArea.transform.GetChild(i).gameObject.SetActive(false);
                }

                // Make the main button layout be the one visible panel.
                PushPanel(_mainButtonLayout);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private async void OnAuthenticate()
        {
            try
            {
                _authenticateBtnText.text = "Authenticating...";
                _authenticateBtn.interactable = false;

                var authenticateTask = GKLocalPlayer.Authenticate();
                _localPlayer = await authenticateTask;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (_localPlayer?.IsAuthenticated ?? false)
            {
                foreach (var btn in _mainButtonLayout.GetComponentsInChildren<Button>())
                {
                    if (btn != _authenticateBtn)
                    {
                        btn.interactable = true;
                    }
                }

                _authenticateBtnText.text = "Authenticated";
            }
            else
            {
                _authenticateBtnText.text = "Authentication Failed";
                _authenticateBtn.interactable = true;
            }


            if (_localPlayer != null)
            {
                Debug.Log($"GameKit Authentication: isAuthenticated => {_localPlayer.IsAuthenticated}, displayName: {_localPlayer.DisplayName}");

                _playerDisplayName.text = _localPlayer.DisplayName;

                try
                {
                    _playerPhotoImage.texture = await _localPlayer.LoadPhoto(GKPlayer.PhotoSize.Normal);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                _isMultiplayerGamingRestrictedText.text = $"IsMultiplayerGamingRestricted: {_localPlayer.IsMultiplayerGamingRestricted}";
                _isPersonalizedCommunicationRestrictedText.text = $"IsPersonalizedCommunicationRestricted: {_localPlayer.IsPersonalizedCommunicationRestricted}";
                _isUnderageText.text = $"IsUnderage: {_localPlayer.IsUnderage}";

                await TestFetchItems();
            }
        }

        private async Task TestFetchItems()
        {
            var items = await GKLocalPlayer.Local.FetchItems();
            Debug.Log(
                "GKLocalPlayer.FetchItems:\n" + 
                $"  PublicKeyUrl={items.PublicKeyUrl}\n" + 
                $"  Signature={Convert.ToBase64String(items.GetSignature())} ({items.Signature.Length} bytes)\n" + 
                $"  Salt={Convert.ToBase64String(items.GetSalt())} ({items.Salt.Length} bytes)\n" +
                $"  Timestamp={items.Timestamp}\n");
        }

        private static Stack<GameObject> _panelStack;
        private static Stack<GameObject> PanelStack => _panelStack ??= new Stack<GameObject>();

        public static void PushPanel(GameObject panel)
        {
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Peek() : null;
            if (oldPanel != null)
            {
                oldPanel.SetActive(false);
            }

            PanelStack.Push(panel);
            panel.SetActive(true);
        }

        public static void PopPanel()
        {
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Pop() : null;
            if (oldPanel != null)
            {
                oldPanel.SetActive(false);
            }

            if (PanelStack.Count > 0)
            {
                PanelStack.Peek().SetActive(true);
            }
        }

        public static void ReplaceActivePanel(GameObject panel)
        {
            // Important: Deactivate last because it might end the script if the
            // caller is deactivating itself.
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Pop() : null;

            PanelStack.Push(panel);
            panel.SetActive(true);

            if (oldPanel != null)
            {
                oldPanel.SetActive(false);
            }
        }

        private void OnRealtimeMatchmaking()
        {
            PushPanel(_realtimeMatchStatusPanel.Match != null ? _realtimeMatchStatusPanel.gameObject : _realtimeMatchRequestPanel.gameObject);
        }

        public async void OnInviteAccepted(GKPlayer invitedPlayer, GKInvite invite)
        {
            var match = await GKMatchmakerViewController.Request(invite);
            _realtimeMatchStatusPanel.Populate(match);
            PushPanel(_realtimeMatchStatusPanel.gameObject);
        }

        private void OnShowAccessPointPanel()
        {
            PushPanel(_accessPointPanel.gameObject);
        }

        private void OnShowFriendsPanel()
        {
            PushPanel(_friendsPanel.gameObject);
        }

        private void OnShowAchievements()
        {
            PushPanel(_achievementsPanel.gameObject);
        }

        private void OnShowNearbyPlayersPanel()
        {
            PushPanel(_nearbyPlayersPanel.gameObject);
        }

        private async void OnShowTurnBasedMatches()
        {
            try
            {
                var request = GKMatchRequest.Init();
                request.MinPlayers = 2;
                request.MaxPlayers = Math.Max(request.MinPlayers, GKMatchRequest.MaxPlayersAllowedForMatch(GKMatchRequest.GKMatchType.TurnBased));

                _activeMatch = await GKTurnBasedMatchmakerViewController.Request(request);
                _playerDisplayName.text = $"Match: {_activeMatch.MatchId}, Status: {_activeMatch.MatchStatus}, IsMyTurn: {_activeMatch.IsActivePlayer}";

                foreach (var participant in _activeMatch.Participants)
                {
                    _playerDisplayName.text += participant.Player.GamePlayerId;
                }
            }
            catch (GameKitException e)
            {
                Debug.LogError(e);
            }
        }

        private void OnMatchEnded(GKPlayer player, GKTurnBasedMatch match)
        {
            // Ignore if not the active match...
            if (match.MatchId != _activeMatch.MatchId)
                return;

            // Update local data representation...
            _activeMatch = match;

            // Find the match outcome for the local player...
            var localOutcome = GKTurnBasedMatch.Outcome.None;

            foreach (var participant in match.Participants)
            {
                if (participant.Player.GamePlayerId == _localPlayer.GamePlayerId)
                {
                    localOutcome = participant.MatchOutcome;
                }
            }

            _playerDisplayName.text = $"Match Ended: {match.MatchId}, Status: {match.MatchStatus}, LocalOutcome: {localOutcome}, IsMyTurn: {match.IsActivePlayer}";
        }

        private void OnMatchTurnEnded(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive)
        {
            // Ignore if not the active match...
            if (match.MatchId != _activeMatch.MatchId)
                return;

            // Update local data representation...
            _activeMatch = match;

            _playerDisplayName.text = $"Match Turn Ended: {match.MatchId}, Status: {match.MatchStatus}, IsMyTurn: {didBecomeActive}";

            foreach (var participant in _activeMatch.Participants)
            {
                _playerDisplayName.text += participant.Player.DisplayName;
            }
        }

        private async void OnTakeTurn()
        {
            // Ensure it's our turn...
            if (!_activeMatch.IsActivePlayer)
                return;

            try
            {
                // Update data for turn...
                // TODO: automate the setting of the DataLength...
                var data = Encoding.UTF8.GetBytes("Hello World");
                await _activeMatch.EndTurn(GetNextParticipants(_activeMatch), GKTurnBasedMatch.TurnTimeoutNone, data);
            }
            catch (GameKitException e)
            {
                Debug.LogError(e);
            }
        }

        public GKTurnBasedParticipant[] GetNextParticipants(GKTurnBasedMatch match)
        {
            return match.Participants.Where(p => p != match.CurrentParticipant).ToArray();
        }

        private async void OnEndMatchWinner()
        {
            // Ensure we are in a real match...
            if (_activeMatch.MatchStatus != GKTurnBasedMatch.Status.Open
                || !_activeMatch.IsActivePlayer)
                return;

            try
            {
                // Set outcomes...
                foreach (var participant in _activeMatch.Participants)
                {
                    participant.MatchOutcome = participant.Player.GamePlayerId == _localPlayer.GamePlayerId ? GKTurnBasedMatch.Outcome.Won : GKTurnBasedMatch.Outcome.Lost;
                }

                // End match...
                await _activeMatch.EndMatchInTurn(_activeMatch.MatchData);
            }
            catch (GameKitException e)
            {
                Debug.LogError(e);
            }
        }

        private async void OnReportLeaderboardScore()
        {
            var leaderboards = await GKLeaderboard.LoadLeaderboards();
            var leaderboard = leaderboards?.FirstOrDefault();

            if (leaderboard != null)
            {
                await leaderboard.SubmitScore(100, 0, GKLocalPlayer.Local);
            }

            var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
            await gameCenter.Present();

            if (leaderboard != null)
            {
                var players = new NSMutableArray<GKPlayer>();

                // Demonstrate LoadEntries().
                var scores1 = await leaderboard.LoadEntries(GKLeaderboard.PlayerScope.Global, GKLeaderboard.TimeScope.AllTime, 0, 100);
                Debug.Log($"GKLeaderboard.LoadEntries: my score: {scores1.LocalPlayerEntry.Score}");
                foreach (var score in scores1.Entries.OrderByDescending(e => e.Score).ToArray())
                {
                    Debug.Log($"GKLeaderboard.LoadEntries: score: {score.Score} by {score.Player.DisplayName}");
                    players.Add(score.Player);
                }

                // Demonstrate LoadEntriesForPlayers().
                var scores2 = await leaderboard.LoadEntriesForPlayers(players, GKLeaderboard.TimeScope.AllTime);
                Debug.Log($"GKLeaderboard.LoadEntriesForPlayers: my score: {scores2.LocalPlayerEntry.Score}");
                foreach (var score in scores2.Entries.OrderByDescending(e => e.Score).ToArray())
                {
                    Debug.Log($"GKLeaderboard.LoadEntriesForPlayers: score: {score.Score} by {score.Player.DisplayName}");
                    players.Add(score.Player);
                }
            }
        }
    }
}
