using System;
using System.Linq;
using System.Text;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class TurnBasedMatchesPanel : MonoBehaviour
    {
        [SerializeField] private Button _showTurnBasedMatchesButton = default;
        [SerializeField] private Button _takeTurnButton = default;
        [SerializeField] private Button _endMatchWinnerButton = default;
        [SerializeField] private Button _clearLogutton = default;

        [SerializeField] private MessageLog _messageLog = default;

        private GKTurnBasedMatch _activeMatch;

        private void Start()
        {
            try
            {
                _showTurnBasedMatchesButton.onClick.AddListener(OnShowTurnBasedMatches);
                _takeTurnButton.onClick.AddListener(OnTakeTurn);
                _endMatchWinnerButton.onClick.AddListener(OnEndMatchWinner);
                _clearLogutton.onClick.AddListener(OnClearLog);

                GKTurnBasedMatch.TurnEventReceived += OnMatchTurnEnded;
                GKTurnBasedMatch.MatchEnded += OnMatchEnded;
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
                _messageLog.AppendMessageToLog(ex.ToString());
            }
        }

        private async void OnShowTurnBasedMatches()
        {
            try
            {
                var request = GKMatchRequest.Init();
                request.MinPlayers = 2;
                request.MaxPlayers = Math.Max(request.MinPlayers, GKMatchRequest.MaxPlayersAllowedForMatch(GKMatchRequest.GKMatchType.TurnBased));

                _activeMatch = await GKTurnBasedMatchmakerViewController.Request(request);

                if (_activeMatch != null)
                {
                    var message = $"Match: {_activeMatch.MatchId}, Status: {_activeMatch.MatchStatus}, IsMyTurn: {_activeMatch.IsActivePlayer}";

                    foreach (var participant in _activeMatch.Participants)
                    {
                        message += participant.Player.GamePlayerId;
                    }

                    _messageLog.AppendMessageToLog(message);                    
                }
                else
                {
                    _messageLog.AppendMessageToLog("No active match.");
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
                _messageLog.AppendMessageToLog(ex.ToString());
            }
        }

        private void OnMatchEnded(GKPlayer player, GKTurnBasedMatch match)
        {
            if (_activeMatch == null)
            {
                _messageLog.AppendMessageToLog("No active match.");
                return;
            }

            // Ignore if not the active match...
            if (match.MatchId != _activeMatch.MatchId)
            {
                _messageLog.AppendMessageToLog("No active match.");
                return;
            }

            // Update local data representation...
            _activeMatch = match;

            // Find the match outcome for the local player...
            var localOutcome = GKTurnBasedMatch.Outcome.None;

            foreach (var participant in match.Participants)
            {
                if (participant.Player.GamePlayerId == GKLocalPlayer.Local.GamePlayerId)
                {
                    localOutcome = participant.MatchOutcome;
                }
            }

            var message = $"Match Ended: {match.MatchId}, Status: {match.MatchStatus}, LocalOutcome: {localOutcome}, IsMyTurn: {match.IsActivePlayer}";
            _messageLog.AppendMessageToLog(message);
        }

        private void OnMatchTurnEnded(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive)
        {
            if (_activeMatch == null)
            {
                _messageLog.AppendMessageToLog("No active match.");
                return;
            }

            // Ignore if not the active match...
            if (match.MatchId != _activeMatch.MatchId)
            {
                return;
            }

            // Update local data representation...
            _activeMatch = match;

            var message = $"Match Turn Ended: {match.MatchId}, Status: {match.MatchStatus}, IsMyTurn: {didBecomeActive}";

            foreach (var participant in _activeMatch.Participants)
            {
                message += participant.Player.DisplayName;
            }

            _messageLog.AppendMessageToLog(message);
        }

        private async void OnTakeTurn()
        {
            if (_activeMatch == null)
            {
                _messageLog.AppendMessageToLog("No active match.");
                return;
            }

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
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
                _messageLog.AppendMessageToLog(ex.ToString());
            }
        }

        public GKTurnBasedParticipant[] GetNextParticipants(GKTurnBasedMatch match)
        {
            return match.Participants.Where(p => p != match.CurrentParticipant).ToArray();
        }

        private async void OnEndMatchWinner()
        {
            if (_activeMatch == null)
            {
                _messageLog.AppendMessageToLog("No active match.");
                return;
            }

            // Ensure we are in a real match...
            if (_activeMatch.MatchStatus != GKTurnBasedMatch.Status.Open ||
                !_activeMatch.IsActivePlayer)
                return;

            try
            {
                // Set outcomes...
                foreach (var participant in _activeMatch.Participants)
                {
                    participant.MatchOutcome = participant.Player.GamePlayerId == GKLocalPlayer.Local.GamePlayerId ? GKTurnBasedMatch.Outcome.Won : GKTurnBasedMatch.Outcome.Lost;
                }

                // End match...
                await _activeMatch.EndMatchInTurn(_activeMatch.MatchData);
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
                _messageLog.AppendMessageToLog(ex.ToString());
            }
        }

        private void OnClearLog()
        {
            _messageLog.ClearLog();
        }
    }
}
