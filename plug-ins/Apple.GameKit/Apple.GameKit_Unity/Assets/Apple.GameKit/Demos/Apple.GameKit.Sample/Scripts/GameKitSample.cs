using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apple.Core;
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
#pragma warning disable 0649
        [SerializeField] private RawImage _playerPhotoImage;

        [SerializeField] private Text _playerDisplayName;
        [SerializeField] private Text _isMultiplayerGamingRestrictedText;
        [SerializeField] private Text _isPersonalizedCommunicationRestrictedText;
        [SerializeField] private Text _isUnderageText;

        [SerializeField] private GameObject _panelArea;

        [SerializeField] private GameObject _mainButtonLayout;
        [SerializeField] private RealtimeMatchRequestPanel _realtimeMatchRequestPanel;
        [SerializeField] private RealtimeMatchStatusPanel _realtimeMatchStatusPanel;

        [SerializeField] private Button _authenticateBtn;
        [SerializeField] private Text _authenticateBtnText;

        [SerializeField] private Button _showAchievementsBtn;
        [SerializeField] private Button _showTurnBasedMatchesBtn;
        [SerializeField] private Button _takeTurnButton;
        [SerializeField] private Button _endMatchWinnerButton;
        [SerializeField] private Button _reportLeaderboardScore;
        [SerializeField] private Button _toggleAccessPoint;
        [SerializeField] private Button _triggerAccessPoint;
        [SerializeField] private Button _realtimeMatchmakingButton;
#pragma warning restore 0649

        private GKLocalPlayer _localPlayer;
        private GKTurnBasedMatch _activeMatch;

        private void Start()
        {
            try
            {
                // Send Unity log messages to NSLog.
                _ = new AppleLogger();

                GKAccessPoint.Shared.Location = GKAccessPoint.GKAccessPointLocation.TopLeading;
                GKAccessPoint.Shared.ShowHighlights = false;
                GKAccessPoint.Shared.IsActive = true;

                _authenticateBtn.onClick.AddListener(OnAuthenticate);
                _showAchievementsBtn.onClick.AddListener(OnShowAchievements);
                _showTurnBasedMatchesBtn.onClick.AddListener(OnShowTurnBasedMatches);
                _takeTurnButton.onClick.AddListener(OnTakeTurn);
                _endMatchWinnerButton.onClick.AddListener(OnEndMatchWinner);
                _reportLeaderboardScore.onClick.AddListener(OnReportLeaderboardScore);
                _toggleAccessPoint.onClick.AddListener(OnToggleAccessPoint);
                _triggerAccessPoint.onClick.AddListener(OnTriggerAccessPoint);
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
            }
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

        private void OnTriggerAccessPoint()
        {
            if (GKAccessPoint.Shared.IsVisible)
                GKAccessPoint.Shared.Trigger();
        }

        private void OnToggleAccessPoint()
        {
            GKAccessPoint.Shared.IsActive = !GKAccessPoint.Shared.IsActive;
        }

        private async void OnShowAchievements()
        {
            try
            {
                // Wait for player to close the dialog...
                var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Achievements);
                await gameCenter.Present();

                // Log all achievements in game...
                var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();

                if (descriptions.Count > 0)
                {
                    var isRarityPropertyAvailable = 
                        Availability.Available(RuntimeOperatingSystem.macOS, 14, 0) ||
                        Availability.Available(RuntimeOperatingSystem.iOS, 17, 2) ||
                        Availability.Available(RuntimeOperatingSystem.tvOS, 17, 2);

                    var builder = new StringBuilder();
                    builder.AppendLine("Achievement descriptions:");

                    foreach (var achievement in descriptions)
                    {
                        builder.Append($"  {achievement.Identifier}: {achievement.Title}");

                        if (isRarityPropertyAvailable)
                        {
                            builder.AppendLine($" (Rarity: {achievement.RarityPercent * 100.0:F2}%)");
                        }
                        else
                        {
                            builder.AppendLine();
                        }
                    }
                    Debug.Log(builder.ToString());

                    // Get the player's achievements.
                    var achievements = await GKAchievement.LoadAchievements();

                    if (achievements.Count() > 0)
                    {
                        builder.Clear();
                        builder.AppendLine("Player achievements:");

                        foreach (var achievement in achievements)
                        {
                            if (!string.IsNullOrEmpty(achievement.Identifier))
                            {
                                var description = descriptions
                                    .Where(d => d.Identifier.Equals(achievement.Identifier))
                                    .FirstOrDefault();

                                var title = description?.Title ?? string.Empty;

                                builder.AppendLine($"  {achievement.Identifier}: {title} ({achievement.PercentComplete * 100.0:F0}% complete)");
                            }
                        }

                        Debug.Log(builder.ToString());
                    }
                }
                else
                {
                    Debug.Log("This game has no achievements.");
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private async void OnShowTurnBasedMatches()
        {
            try
            {
                var request = GKMatchRequest.Init();
                request.MinPlayers = 2;
                request.MaxPlayers = 2;

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
            var leaderboard = leaderboards.First(l => l.BaseLeaderboardId == "topscores");

            await leaderboard.SubmitScore(100, 0, GKLocalPlayer.Local);

            var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
            await gameCenter.Present();

            var scores = await leaderboard.LoadEntries(GKLeaderboard.PlayerScope.Global, GKLeaderboard.TimeScope.AllTime, 0, 100);

            Debug.LogError($"my score: {scores.LocalPlayerEntry.Score}");

            foreach (var score in scores.Entries)
            {
                Debug.LogError($"score: {score.Score} by {score.Player.DisplayName}");
            }
        }
    }
}
