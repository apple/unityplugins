using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apple.GameKit.Leaderboards;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class GameKitSample : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private RawImage _playerPhotoImage;

        [SerializeField]
        private Text _playerDisplayName;

        [SerializeField]
        private Button _showAchievementsBtn;

        [SerializeField]
        private Button _showTurnBasedMatchesBtn;

        [SerializeField]
        private Button _takeTurnButton;

        [SerializeField]
        private Button _endMatchWinnerButton;

        [SerializeField]
        private Button _reportLeaderboardScore;

        [SerializeField]
        private Button _toggleAccessPoint;

        [SerializeField]
        private Button _triggerAccessPoint;

        [SerializeField]
        private Button _realtimeMatchmakeUI;
#pragma warning restore 0649

        private GKLocalPlayer _localPlayer;
        private GKTurnBasedMatch _activeMatch;

        private async Task Start()
        {
            try
            {
                _localPlayer = await GKLocalPlayer.Authenticate();
                UnityEngine.Debug.Log($"GameKit Authentication: isAuthenticated => {_localPlayer.IsAuthenticated}, displayName: {_localPlayer.DisplayName}");

                GKAccessPoint.Shared.Location = GKAccessPoint.GKAccessPointLocation.TopLeading;
                GKAccessPoint.Shared.ShowHighlights = false;
                GKAccessPoint.Shared.IsActive = true;

                _playerDisplayName.text = _localPlayer.DisplayName;

                _showAchievementsBtn.onClick.AddListener(OnShowAchievements);
                _showTurnBasedMatchesBtn.onClick.AddListener(OnShowTurnBasedMatches);
                _takeTurnButton.onClick.AddListener(OnTakeTurn);
                _endMatchWinnerButton.onClick.AddListener(OnEndMatchWinner);
                _reportLeaderboardScore.onClick.AddListener(OnReportLeaderboardScore);
                _toggleAccessPoint.onClick.AddListener(OnToggleAccessPoint);
                _triggerAccessPoint.onClick.AddListener(OnTriggerAccessPoint);
                _realtimeMatchmakeUI.onClick.AddListener(OnRealtimeMatchmake);

                GKTurnBasedMatch.TurnEventReceived += OnMatchTurnEnded;
                GKTurnBasedMatch.MatchEnded += OnMatchEnded;
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(exception);
            }
        }

        private async void OnRealtimeMatchmake()
        {
            var request = GKMatchRequest.Init();
            request.MinPlayers = 2;
            request.MaxPlayers = 2;

            // Wait for match to start...
            var match = await GKMatchmakerViewController.Request(request);
            match.Delegate.DidFailWithError += OnRealtimeMatchDidFailWithError;
            match.Delegate.DataReceived += OnRealtimeMatchDataReceived;

            // Send some data...
            match.Send(new byte[1] { 0 }, GKMatch.GKSendDataMode.Reliable);
        }
        
        private void OnRealtimeMatchDataReceived(byte[] data, GKPlayer fromPlayer)
        {
            Debug.Log($"Realtime match data received");
        }

        private void OnRealtimeMatchDidFailWithError(GameKitException exception)
        {
            Debug.LogError(exception);
        }

        private void OnTriggerAccessPoint()
        { 
            if(GKAccessPoint.Shared.IsVisible)
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

                foreach(var achievement in descriptions)
                {
                    Debug.Log(achievement.Identifier);
                }

                // Get an achievement the player has completed...
                var achievements = await GKAchievement.LoadAchievements();

                if (achievements.Count() > 0)
                {
                    var achievement = achievements.FirstOrDefault();

                    if (!string.IsNullOrEmpty(achievement.Identifier))
                    {
                        Debug.LogError(achievement.Identifier);
                    }
                }
            }
            catch(Exception exception)
            {
                Debug.LogError(exception);
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

                foreach(var participant in _activeMatch.Participants)
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

            foreach(var participant in match.Participants)
            {
                if(participant.Player.GamePlayerId == _localPlayer.GamePlayerId)
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
            catch(GameKitException e)
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
            
            await leaderboard.SubmitScore( 100, 0, GKLocalPlayer.Local);

            var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
            await gameCenter.Present();

            var scores = await leaderboard.LoadEntries(GKLeaderboard.PlayerScope.Global, GKLeaderboard.TimeScope.AllTime, 0, 100);
            
            Debug.LogError($"my score: {scores.LocalPlayerEntry.Score}");

            foreach(var score in scores.Entries)
            {
                Debug.LogError($"score: {score.Score} by {score.Player.DisplayName}");
            }
        }

        private Task<GKMatch> OnFindRealtimeMatch()
        {
            var request = GKMatchRequest.Init();
            request.MinPlayers = 2;
            request.MaxPlayers = 4;

            return GKMatchmakerViewController.Request(request);
        }
    }
}