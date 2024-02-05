using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object used to implement turn-based matches between sets of players on Game Center.
    /// </summary>
    public class GKTurnBasedMatch : NSObject
    {
        #region Delegates
        // Exchange Handlers...
        public delegate void ExchangeCanceledHandler(GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);
        private delegate void InteropExchangeCanceledHandler(IntPtr player, IntPtr exchange, IntPtr match);
        public delegate void ExchangeCompletedHandler(GKPlayer player, NSArray<GKTurnBasedExchangeReply> replies, GKTurnBasedExchange exchange, GKTurnBasedMatch match);
        private delegate void InteropExchangeCompletedHandler(IntPtr player, IntPtr replies, IntPtr exchage, IntPtr match);
        public delegate void ExchangeReceivedHandler(GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);
        private delegate void InteropExchangeReceivedHandler(IntPtr player, IntPtr exchange, IntPtr match);
        // Match related handlers...
        public delegate void MatchRequestedWithOtherPlayersHandler(GKPlayer player, NSArray<GKPlayer> playersToInvite);
        private delegate void InteropMatchRequestedWithOtherPlayersHandler(IntPtr player, IntPtr playersToInvite);
        public delegate void MatchEndedHandler(GKPlayer player, GKTurnBasedMatch match);
        private delegate void InteropMatchEndedHandler(IntPtr player, IntPtr match);
        public delegate void TurnEventReceivedHandler(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive);
        private delegate void InteropTurnEventReceivedHandler(IntPtr player, IntPtr match, bool didBecomeActive);
        public delegate void PlayerWantsToQuitMatchHandler(GKPlayer player, GKTurnBasedMatch match);
        private delegate void InteropPlayerWantsToQuitMatchHandler(IntPtr player, IntPtr match);
        #endregion
        
        #region Static Events
        /// <summary>
        /// Called when the exchange is cancelled by the sender.
        /// </summary>
        public static event ExchangeCanceledHandler ExchangeCanceled;
        /// <summary>
        /// Called when a player receives an exchange request from another player.
        /// </summary>
        public static event ExchangeReceivedHandler ExchangeReceived;
        /// <summary>
        /// Called when the exchange is completed.
        /// </summary>
        public static event ExchangeCompletedHandler ExchangeCompleted;
        /// <summary>
        /// Initiates a match from Game Center with the requested players.
        /// </summary>
        public static event MatchRequestedWithOtherPlayersHandler MatchRequestedWithOtherPlayers;
        /// <summary>
        /// Called when the match has ended.
        /// </summary>
        public static event MatchEndedHandler MatchEnded;
        /// <summary>
        /// Activates the player's turn.
        /// </summary>
        public static event TurnEventReceivedHandler TurnEventReceived;
        /// <summary>
        /// Indicates that the current player wants to quit the current match.
        /// </summary>
        public static event PlayerWantsToQuitMatchHandler PlayerWantsToQuit;
        #endregion
        
        #region Static Event Registration
        static GKTurnBasedMatch()
        {
            // Exchange events...
            Interop.GKTurnBasedMatch_SetExchangeCanceledCallback(OnExchangeCanceled);
            Interop.GKTurnBasedMatch_SetExchangeReceivedCallback(OnExchangeReceived);
            Interop.GKTurnBasedMatch_SetExchangeCompletedCallback(OnExchangeCompleted);
            
            // Turn based events...
            Interop.GKTurnBasedMatch_SetMatchRequestedWithOtherPlayersCallback(OnMatchRequestedWithOtherPlayers);
            Interop.GKTurnBasedMatch_SetMatchEndedCallback(OnMatchEnded);
            Interop.GKTurnBasedMatch_SetTurnEventReceivedCallback(OnTurnEventReceived);
            Interop.GKTurnBasedMatch_SetPlayerWantsToQuitMatchCallback(OnPlayerWantsToQuit);
        }

        [MonoPInvokeCallback(typeof(InteropExchangeCanceledHandler))]
        private static void OnExchangeCanceled(IntPtr player, IntPtr exchange, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ExchangeCanceled?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKTurnBasedExchange>(exchange), PointerCast<GKTurnBasedMatch>(match)));
        }
        
        [MonoPInvokeCallback(typeof(InteropExchangeReceivedHandler))]
        private static void OnExchangeReceived(IntPtr player, IntPtr exchange, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ExchangeReceived?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKTurnBasedExchange>(exchange), PointerCast<GKTurnBasedMatch>(match)));
        }
        
        [MonoPInvokeCallback(typeof(InteropExchangeCompletedHandler))]
        private static void OnExchangeCompleted(IntPtr player, IntPtr replies, IntPtr exchange, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ExchangeCompleted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<NSArray<GKTurnBasedExchangeReply>>(replies), PointerCast<GKTurnBasedExchange>(exchange), PointerCast<GKTurnBasedMatch>(match)));
        }

        [MonoPInvokeCallback(typeof(InteropMatchRequestedWithOtherPlayersHandler))]
        private static void OnMatchRequestedWithOtherPlayers(IntPtr player, IntPtr otherPlayers)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => MatchRequestedWithOtherPlayers?.Invoke(PointerCast<GKPlayer>(player), PointerCast<NSArray<GKPlayer>>(otherPlayers)));
        }

        [MonoPInvokeCallback(typeof(InteropMatchEndedHandler))]
        private static void OnMatchEnded(IntPtr player, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => MatchEnded?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKTurnBasedMatch>(match)));
        }

        [MonoPInvokeCallback(typeof(InteropTurnEventReceivedHandler))]
        private static void OnTurnEventReceived(IntPtr player, IntPtr match, bool didBecomeActive)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => TurnEventReceived?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKTurnBasedMatch>(match), didBecomeActive));
        }

        [MonoPInvokeCallback(typeof(InteropPlayerWantsToQuitMatchHandler))]
        private static void OnPlayerWantsToQuit(IntPtr player, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => PlayerWantsToQuit?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKTurnBasedMatch>(match)));
        }
        #endregion

        internal GKTurnBasedMatch(IntPtr pointer) : base(pointer) {}
        
        /// <summary>
        /// Information about the players participating in the match.
        /// </summary>
        public NSArray<GKTurnBasedParticipant> Participants
        {
            get
            {
                var pointer = Interop.GKTurnBasedMatch_GetParticipants(Pointer);

                if (pointer != IntPtr.Zero)
                    return new NSArray<GKTurnBasedParticipant>(pointer);
                
                return null;
            }
        }
        
        /// <summary>
        /// The participant whose turn it is to act next.
        /// </summary>
        public GKTurnBasedParticipant CurrentParticipant
        {
            get
            {
                var pointer = Interop.GKTurnBasedMatch_GetCurrentParticipant(Pointer);
                
                if(pointer != IntPtr.Zero)
                    return new GKTurnBasedParticipant(pointer);

                return null;
            }
        }
        
        /// <summary>
        /// Returns whether the CurrentParticipant gamePlayerId matches
        /// the GKLocalPlayer.
        /// </summary>
        public bool IsActivePlayer
        {
            get
            {
                if (CurrentParticipant != null)
                {
                    return CurrentParticipant.Player?.GamePlayerId == GKLocalPlayer.Local.GamePlayerId;
                }

                return false;
            }
        }
        
        /// <summary>
        /// Game-specific data that reflects the details of the match.
        /// </summary>
        public byte[] MatchData
        {
            get
            {
                return Interop.GKTurnBasedMatch_GetMatchData(Pointer).ToBytes();
            }
        }
        
        /// <summary>
        /// Returns the limit the Game Center servers place on the size of the match data.
        /// </summary>
        public int MatchDataMaximumSize => Interop.GKTurnBasedMatch_GetMatchDataMaximumSize(Pointer);
        
        /// <summary>
        /// A message displayed to all players in the match.
        /// </summary>
        public string Message => Interop.GKTurnBasedMatch_GetMessage(Pointer);
        
        /// <summary>
        /// The date that the match was created.
        /// </summary>
        public DateTimeOffset CreationDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedMatch_GetCreationDate(Pointer));
        
        /// <summary>
        /// A string that uniquely identifies the match.
        /// </summary>
        public string MatchId => Interop.GKTurnBasedMatch_GetMatchID(Pointer);
        
        /// <summary>
        /// The current state of the match.
        /// </summary>
        public Status MatchStatus => Interop.GKTurnBasedMatch_GetStatus(Pointer);
        
        #region LoadMatchData

        /// <summary>
        /// Loads the game-specific data associated with a match, including all exchanges.
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> LoadMatchData()
        {
            var tcs = InteropTasks.Create<byte[]>(out var taskId);
            Interop.GKTurnBasedMatch_LoadMatchData(Pointer, taskId, OnLoadMatchData, OnLoadMatchDataError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<InteropData>))]
        private static void OnLoadMatchData(long taskId, InteropData data)
        {
            InteropTasks.TrySetResultAndRemove(taskId, data.ToBytes());
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadMatchDataError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<byte[]>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region SaveCurrentTurn

        /// <summary>
        /// Update the match data without advancing the game to another player.
        /// </summary>
        /// <param name="matchData">A serialized blob of data reflecting the game-specific state for the match. Do not pass nil as an argument.</param>
        /// <returns></returns>
        public Task SaveCurrentTurn(byte[] matchData)
        {
            var handle = GCHandle.Alloc(matchData, GCHandleType.Pinned);
            var data = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = matchData.Length
            };
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_SaveCurrentTurn(Pointer, taskId, data, OnSaveCurrentTurn, OnSaveCurrentTurnError);
            handle.Free();
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSaveCurrentTurn(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }
        
        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSaveCurrentTurnError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region EndTurn

        /// <summary>
        /// Updates the data stored on Game Center for the current match.
        /// </summary>
        /// <param name="nextParticipants">An array of GKTurnBasedParticipant objects reflecting the order in which the players should act next. Each object in the array must be one of the objects stored in the match participants property.</param>
        /// <param name="timeout">The length of time the next player has to complete their turn. The maximum timeout is 90 days.</param>
        /// <param name="matchData">A serialized blob of data reflecting the game-specific state for the match. Do not pass nil as an argument.</param>
        /// <returns></returns>
        public Task EndTurn(GKTurnBasedParticipant[] nextParticipants, TimeSpan timeout, byte[] matchData)
        {
            var handle = GCHandle.Alloc(matchData, GCHandleType.Pinned);
            var data = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = matchData.Length
            };

            // Prepare participants...
            var mutable = new NSMutableArray<GKTurnBasedParticipant>(nextParticipants);
            
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_EndTurn(Pointer, taskId, mutable.Pointer, timeout.TotalSeconds, data, OnEndTurn, OnSaveCurrentTurnError);
            handle.Free();
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnEndTurn(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnEndTurnError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region ParticipantQuitInTurn

        /// <summary>
        /// Resigns the current player from the match without ending the match.
        /// </summary>
        /// <param name="outcome">The end outcome of the current player in the match.</param>
        /// <param name="nextParticipants">An array of GKTurnBasedParticipant objects that contains participant objects reflecting the order in which the players should act next. Each object in the array must be one of the objects stored in the match participants property.</param>
        /// <param name="turnTimeout">The length of time the next player has to complete their turn. The maximum timeout is 90 days.</param>
        /// <param name="matchData">A serialized blob of data reflecting the game-specific state for the match.</param>
        /// <returns></returns>
        public Task ParticipantQuitInTurn(Outcome outcome, GKTurnBasedParticipant[] nextParticipants, TimeSpan turnTimeout, byte[] matchData)
        {
            var handle = GCHandle.Alloc(matchData, GCHandleType.Pinned);
            var data = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = matchData.Length
            };

            // Prepare participants...
            var mutable = new NSMutableArray<GKTurnBasedParticipant>(nextParticipants);
            
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_ParticipantQuitInTurn(Pointer, taskId, outcome, mutable.Pointer, turnTimeout.TotalSeconds, data, OnParticipantQuitInTurn, OnParticipantQuitInTurnError);
            handle.Free();
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnParticipantQuitInTurn(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnParticipantQuitInTurnError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region ParticipantQuitOutOfTurn

        /// <summary>
        /// Resigns the player from the match when that player is not the current player. This action does not end the match
        /// </summary>
        /// <param name="outcome">The end outcome of the current player in the match.</param>
        /// <returns></returns>
        public Task ParticipantQuitOutOfTurn(Outcome outcome)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_ParticipantQuitOutOfTurn(Pointer, taskId, outcome, OnParticipantQuitOutOfTurn, OnParticipantQuitOutOfTurnError);
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnParticipantQuitOutOfTurn(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnParticipantQuitOutOfTurnError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region EndMatchInTurn
        
        /// <summary>
        /// Ends the match.
        /// </summary>
        /// <param name="matchData">A serialized blob of data reflecting the end state for the match. Do not pass nil as an argument.</param>
        /// <returns></returns>
        public Task EndMatchInTurn(byte[] matchData)
        {
            var handle = GCHandle.Alloc(matchData, GCHandleType.Pinned);
            var data = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = matchData.Length
            };

            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_EndMatchInTurn(Pointer, taskId, data, OnEndMatchInTurn, OnEndMatchInTurnError);
            handle.Free();
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnEndMatchInTurn(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnEndMatchInTurnError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Remove

        /// <summary>
        /// Programmatically removes a match from Game Center.
        /// </summary>
        /// <returns></returns>
        public Task Remove()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_Remove(Pointer, taskId, OnRemove, OnRemoveError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnRemove(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnRemoveError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region SaveMergedMatch

        /// <summary>
        /// Saves the merged data for the current turn without ending the turn.
        /// </summary>
        /// <param name="matchData">A serialized blob of data reflecting the current state for the match.</param>
        /// <param name="exchanges">An array of GKTurnBasedExchange objects that contains the resolved exchanges.</param>
        /// <returns></returns>
        public Task SaveMergedMatch(byte[] matchData, params GKTurnBasedExchange[] exchanges)
        {
            var handle = GCHandle.Alloc(matchData, GCHandleType.Pinned);
            var data = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = matchData.Length
            };

            // Prepare exchanges...
            var mutable = new NSMutableArray<GKTurnBasedExchange>(exchanges);
            
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_SaveMergedMatch(Pointer, taskId, data, mutable.Pointer, OnSaveMergedMatch, OnSaveMergedMatchError);
            handle.Free();
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSaveMergedMatch(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSaveMergedMatchError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region SendExchange

        public Task<GKTurnBasedExchange> SendExchange(GKTurnBasedParticipant[] participants, byte[] data, string localizableMessageKey, string[] arguments, TimeSpan timeout)
        {
            // Data...
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var interopData = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = data.Length
            };
            
            // Participants...
            var mutableParticipants = new NSMutableArray<GKTurnBasedParticipant>(participants);

            // Arguments...
            var mutableArguments = new NSMutableArray<NSString>(arguments?.Select(arg => new NSString(arg)));

            var tcs = InteropTasks.Create<GKTurnBasedExchange>(out var taskId);
            Interop.GKTurnBasedMatch_SendExchange(
                Pointer, 
                taskId, participants: mutableParticipants.Pointer,
                data: interopData,
                localizableMessageKey: localizableMessageKey,
                arguments: mutableArguments.Pointer,
                timeout: timeout.TotalSeconds,
                onSuccess: OnSendExchange,
                onError: OnSendExchangeError);

            handle.Free();
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnSendExchange(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKTurnBasedExchange>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSendExchangeError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedExchange>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        /// <summary>
        /// The exchange will timeout after one day if no reply is received.
        /// </summary>
        public static TimeSpan ExchangeTimeoutDefault => TimeSpan.FromSeconds(Interop.GKTurnBasedMatch_GKExchangeTimeoutDefault());
        
        /// <summary>
        /// The exchange will not timeou
        /// </summary>
        public static TimeSpan ExchangeTimeoutNone => TimeSpan.FromSeconds(Interop.GKTurnBasedMatch_GKExchangeTimeoutNone());
        
        /// <summary>
        /// Indicates that the player has one week to take a turn.
        /// </summary>
        public static TimeSpan TurnTimeoutDefault => TimeSpan.FromSeconds(Interop.GKTurnBasedMatch_GetTurnTimeoutDefault());
        
        /// <summary>
        /// Indicates that the playerâ€™s turn never times out.
        /// </summary>
        public static TimeSpan TurnTimeoutNone => TimeSpan.FromSeconds(Interop.GKTurnBasedMatch_GetTurnTimeoutNone());
        
        /// <summary>
        /// Returns the exchanges that are active for the local player.
        /// </summary>
        public NSArray<GKTurnBasedExchange> ActiveExchanges => PointerCast<NSArray<GKTurnBasedExchange>>(Interop.GKTurnBasedMatch_GetActiveExchanges(Pointer));
        
        /// <summary>
        /// The exchanges that have been completed and need to be merged by the local participant.
        /// </summary>
        public NSArray<GKTurnBasedExchange> CompletedExchanges => PointerCast<NSArray<GKTurnBasedExchange>>(Interop.GKTurnBasedMatch_GetCompletedExchanges(Pointer));
        
        /// <summary>
        /// The current exchanges that are in progress for the match.
        /// </summary>
        public NSArray<GKTurnBasedExchange> Exchanges => PointerCast<NSArray<GKTurnBasedExchange>>(Interop.GKTurnBasedMatch_GetExchanges(Pointer));
        
        /// <summary>
        /// The maximum amount of data allowed for an exchange.
        /// </summary>
        public int ExchangeDataMaximumSize => Interop.GKTurnBasedMatch_GetExchangeDataMaximumSize(Pointer);
        
        /// <summary>
        /// Limits the number of exchanges the player can have initiated at once.
        /// </summary>
        public int ExchangeMaxInitiatedExchangesPerPlayer => Interop.GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer(Pointer);
        
        public void SetLocalizableMessageWithKey(string key, string[] arguments)
        {
            // Arguments...
            var mutableArguments = new NSMutableArray<NSString>(arguments?.Select(arg => new NSString(arg)));

            Interop.GKTurnBasedMatch_SetLocalizableMessageWithKey(
                Pointer,
                key,
                mutableArguments.Pointer);
        }

        #region SendReminder

        /// <summary>
        /// Send a reminder to one or more game participants.
        /// </summary>
        /// <param name="participants">An array of GKTurnBasedParticipant objects containing the participants who are to receive the reminder</param>
        /// <param name="localizableMessageKey">The location of the alert message string in the Localizable.strings file for the current localization.</param>
        /// <param name="arguments">An array of objects to be substituted using the format string.</param>
        /// <returns></returns>
        public Task SendReminder(GKTurnBasedParticipant[] participants, string localizableMessageKey, string[] arguments)
        {
            // Participants...
            var mutableParticipants = new NSMutableArray<GKTurnBasedParticipant>(participants);
            
            // Arguments...
            var mutableArguments = new NSMutableArray<NSString>(arguments?.Select(arg => new NSString(arg)));

            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_SendReminder(
                Pointer,
                taskId,
                participants: mutableParticipants.Pointer,
                localizableMessageKey: localizableMessageKey,
                arguments: mutableArguments.Pointer,
                onSuccess: OnSendReminder,
                onError: OnSendReminderError);

            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSendReminder(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSendReminderError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadMatches

        /// <summary>
        /// Loads the turn-based matches involving the local player and creates a match object for each match.
        /// </summary>
        /// <returns>An array of GKTurnBasedMatch objects containing the match objects for matches that the local player is playing in, or nil if there are no matches to load. If an error occurred, this value may be non-nil. In this case, the array holds whatever match data could be retrieved from Game Center before the error occurred.</returns>
        public static Task<NSArray<GKTurnBasedMatch>> LoadMatches()
        {
            var tcs = InteropTasks.Create<NSArray<GKTurnBasedMatch>>(out var taskId);
            Interop.GKTurnBasedMatch_LoadMatches(taskId, OnLoadMatches, OnLoadMatchesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadMatches(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKTurnBasedMatch>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadMatchesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Load

        /// <summary>
        /// Loads a specific match.
        /// </summary>
        /// <param name="matchId">The identifier for the turn-based match.</param>
        /// <returns></returns>
        public static Task<GKTurnBasedMatch> Load(string matchId)
        {
            var tcs = InteropTasks.Create<GKTurnBasedMatch>(out var taskId);
            Interop.GKTurnBasedMatch_Load(taskId, matchId, OnLoad, OnLoadError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoad(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKTurnBasedMatch>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadError(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedMatch>(taskId, new GameKitException(pointer));
        }
        #endregion
        
        #region Find

        /// <summary>
        /// Programmatically searches for a new match to join.
        /// </summary>
        /// <param name="matchRequest">A match request that specifies the properties that the new match must fulfill.</param>
        /// <returns>A newly initialized match object that contains a list of players for the match. If an error occurred, this value is nil.</returns>
        public static Task<GKTurnBasedMatch> Find(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<GKTurnBasedMatch>(out var taskId);
            Interop.GKTurnBasedMatch_Find(taskId, matchRequest.Pointer, OnFind, OnFindError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFind(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKTurnBasedMatch>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region AcceptInvite

        /// <summary>
        /// Programmatically accept an invitation to a turn-based match.
        /// </summary>
        /// <returns>A newly initialized match object that contains a list of players for the match. If an error occurred, this value is nil.</returns>
        public Task<GKTurnBasedMatch> AcceptInvite()
        {
            var tcs = InteropTasks.Create<GKTurnBasedMatch>(out var taskId);
            Interop.GKTurnBasedMatch_AcceptInvite(Pointer, taskId, OnAcceptInvite, OnAcceptInviteError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnAcceptInvite(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKTurnBasedMatch>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnAcceptInviteError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region DeclineInvite

        /// <summary>
        /// Programmatically decline an invitation to a turn-based match.
        /// </summary>
        /// <returns></returns>
        public Task DeclineInvite()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedMatch_DeclineInvite(Pointer, taskId, OnDeclineInvite, OnDeclineInviteError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnDeclineInvite(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnDeclineInviteError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Rematch

        /// <summary>
        /// Create a new turn-based match with the same participants as an existing match.
        /// </summary>
        /// <returns>A newly initialized match object that contains a list of players for the match. If an error occurred, this value is nil.</returns>
        public Task<GKTurnBasedMatch> Rematch()
        {
            var tcs = InteropTasks.Create<GKTurnBasedMatch>(out var taskId);
            Interop.GKTurnBasedMatch_Rematch(Pointer, taskId, OnRematch, OnRematchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnRematch(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKTurnBasedMatch>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnRematchError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKTurnBasedMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        /// <summary>
        /// The different states that a match can enter.
        /// </summary>
        public enum Status : long
        {
            /// <summary>
            /// The match is in an unexpected state.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The match is currently being played.
            /// </summary>
            Open = 1,
            /// <summary>
            /// The match has been completed.
            /// </summary>
            Ended = 2,
            /// <summary>
            /// Game Center is still searching for other players to join the match.
            /// </summary>
            Matching = 3
        }

        /// <summary>
        /// The state the participant was in when they left the match.
        /// </summary>
        public enum Outcome : long
        {
            None = 0,
            /// <summary>
            /// The participant forfeited the match.
            /// </summary>
            Quit = 1,
            /// <summary>
            /// The participant won the match.
            /// </summary>
            Won = 2,
            /// <summary>
            /// The participant lost the match.
            /// </summary>
            Lost = 3,
            /// <summary>
            /// The participant tied the match.
            /// </summary>
            Tied = 4,
            /// <summary>
            /// The participant was ejected from the match because he or she did not act in a timely fashion.
            /// </summary>
            TimeExpired = 5,
            /// <summary>
            /// The participant finished first.
            /// </summary>
            First = 6,
            /// <summary>
            /// The participant finished second.
            /// </summary>
            Second = 7,
            /// <summary>
            /// The participant finished third.
            /// </summary>
            Third = 8,
            /// <summary>
            /// The participant finished fourth.
            /// </summary>
            Fourth = 9,
            /// <summary>
            /// A mask used to allow your game to provide its own custom outcome. Any custom value must fit inside the mask.
            /// </summary>
            CustomRange = 16711680
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetExchangeReceivedCallback(InteropExchangeReceivedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetExchangeCanceledCallback(InteropExchangeCanceledHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetExchangeCompletedCallback(InteropExchangeCompletedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetMatchRequestedWithOtherPlayersCallback(InteropMatchRequestedWithOtherPlayersHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetMatchEndedCallback(InteropMatchEndedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetTurnEventReceivedCallback(InteropTurnEventReceivedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetPlayerWantsToQuitMatchCallback(InteropPlayerWantsToQuitMatchHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatch_GetParticipants(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatch_GetCurrentParticipant(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern InteropData GKTurnBasedMatch_GetMatchData(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern int GKTurnBasedMatch_GetMatchDataMaximumSize(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKTurnBasedMatch_GetMessage(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedMatch_GetCreationDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKTurnBasedMatch_GetMatchID(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern Status GKTurnBasedMatch_GetStatus(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_LoadMatchData(IntPtr pointer, long taskId, SuccessTaskCallback<InteropData> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SaveCurrentTurn(IntPtr pointer, long taskId, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_EndTurn(IntPtr pointer, long taskId, IntPtr participants, double timeout, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_ParticipantQuitInTurn(IntPtr pointer, long taskId, Outcome outcome, IntPtr nextParticipants, double timeout, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_ParticipantQuitOutOfTurn(IntPtr pointer, long taskId, Outcome outcome, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_EndMatchInTurn(IntPtr pointer, long taskId, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_Remove(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SaveMergedMatch(IntPtr pointer, long taskId, InteropData data, IntPtr exchanges, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SendExchange(IntPtr pointer, long taskId, IntPtr participants, InteropData data, string localizableMessageKey, IntPtr arguments, double timeout, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKTurnBasedMatch_GKExchangeTimeoutDefault();
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKTurnBasedMatch_GKExchangeTimeoutNone();
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKTurnBasedMatch_GetTurnTimeoutDefault();
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKTurnBasedMatch_GetTurnTimeoutNone();
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatch_GetActiveExchanges(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatch_GetCompletedExchanges(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatch_GetExchanges(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern int GKTurnBasedMatch_GetExchangeDataMaximumSize(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern int GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SetLocalizableMessageWithKey(IntPtr gkTurnBasedMatchPtr, string key, IntPtr arguments);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_SendReminder(IntPtr pointer, long taskId, IntPtr participants, string localizableMessageKey, IntPtr arguments, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_LoadMatches(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_Load(long taskId, string matchId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_Find(long taskId, IntPtr matchRequest, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_AcceptInvite(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_DeclineInvite(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatch_Rematch(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
        }
    }
}
