using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using Apple.GameKit.Multiplayer;

namespace Apple.GameKit
{
    #region GKTurnBasedParticipant
    public class NSArrayGKTurnBasedParticipant : NSArray<GKTurnBasedParticipant>
    {
        public NSArrayGKTurnBasedParticipant(IntPtr pointer) : base(pointer) {}
        
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKTurnBasedParticipant(IntPtr pointer, int index);

        public override GKTurnBasedParticipant ElementAtIndex(int index)
        {
            return PointerCast<GKTurnBasedParticipant>(NSArray_GetGKTurnBasedParticipant(Pointer, index));
        }
    }

    public class NSMutableArrayGKTurnBasedParticipant : NSArrayGKTurnBasedParticipant, INSMutableArray<GKTurnBasedParticipant>
    {
        public NSMutableArrayGKTurnBasedParticipant(IntPtr pointer) : base(pointer) {}

        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKTurnBasedParticipant(IntPtr pointer, IntPtr value);
        
        public void Add(GKTurnBasedParticipant value)
        {
            NSMutableArray_AddGKTurnBasedParticipant(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKTurnBasedExchange

    public class NSArrayGKTurnBasedExchange : NSArray<GKTurnBasedExchange>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKTurnBasedExchangeAt(IntPtr pointer, int index);
        public override GKTurnBasedExchange ElementAtIndex(int index)
        {
            return PointerCast<GKTurnBasedExchange>(NSArray_GetGKTurnBasedExchangeAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKTurnBasedExchange : NSArrayGKTurnBasedExchange, INSMutableArray<GKTurnBasedExchange>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKTurnBasedExchange(IntPtr pointer, IntPtr value);
        public void Add(GKTurnBasedExchange value)
        {
            NSMutableArray_AddGKTurnBasedExchange(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKTurnBasedExchangeReply

    public class NSArrayGKTurnBasedExchangeReply : NSArray<GKTurnBasedExchangeReply>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKTurnBasedExchangeReplyAt(IntPtr pointer, int index);
        public override GKTurnBasedExchangeReply ElementAtIndex(int index)
        {
            return PointerCast<GKTurnBasedExchangeReply>(NSArray_GetGKTurnBasedExchangeReplyAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKTurnBasedExchangeReply : NSArrayGKTurnBasedExchangeReply, INSMutableArray<GKTurnBasedExchangeReply>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKTurnBasedExchangeReply(IntPtr pointer, IntPtr value);
        public void Add(GKTurnBasedExchangeReply value)
        {
            NSMutableArray_AddGKTurnBasedExchangeReply(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKTurnBasedMatch

    public class NSArrayGKTurnBasedMatch : NSArray<GKTurnBasedMatch>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKTurnBasedMatchAt(IntPtr pointer, int index);
        public override GKTurnBasedMatch ElementAtIndex(int index)
        {
            return PointerCast<GKTurnBasedMatch>(NSArray_GetGKTurnBasedMatchAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKTurnBasedMatch : NSArrayGKTurnBasedMatch, INSMutableArray<GKTurnBasedMatch>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKTurnBasedMatch(IntPtr pointer, IntPtr value);
        public void Add(GKTurnBasedMatch value)
        {
            NSMutableArray_AddGKTurnBasedMatch(Pointer, value.Pointer);
        }
    }
    #endregion

    #region GKAchievement
    public class NSArrayGKAchievement : NSArray<GKAchievement>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKAchievementAt(IntPtr pointer, int index);
        
        public override GKAchievement ElementAtIndex(int index)
        {
            return PointerCast<GKAchievement>(NSArray_GetGKAchievementAt(Pointer, index));
        }
    }
    
    public class NSMutableArrayGKAchievement : NSArrayGKAchievement, INSMutableArray<GKAchievement>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKAchievement(IntPtr pointer, IntPtr achievementPtr);
        
        public void Add(GKAchievement value)
        {
            NSMutableArray_AddGKAchievement(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKAchievementDescription
    public class NSArrayGKAchievementDescription : NSArray<GKAchievementDescription>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKAchievementDescriptionAt(IntPtr pointer, int index);
        
        public override GKAchievementDescription ElementAtIndex(int index)
        {
            return PointerCast<GKAchievementDescription>(NSArray_GetGKAchievementDescriptionAt(Pointer, index));
        }
    }
    
    public class NSMutableArrayGKAchievementDescription : NSArrayGKAchievementDescription, INSMutableArray<GKAchievementDescription>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKAchievementDescription(IntPtr pointer, IntPtr achievementPtr);
        
        public void Add(GKAchievementDescription value)
        {
            NSMutableArray_AddGKAchievementDescription(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKPlayer
    public class NSArrayGKPlayer : NSArray<GKPlayer>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKPlayerAt(IntPtr pointer, int insdex);
        
        public override GKPlayer ElementAtIndex(int index)
        {
            return PointerCast<GKPlayer>(NSArray_GetGKPlayerAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKPlayer : NSArrayGKPlayer, INSMutableArray<GKPlayer>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKPlayer(IntPtr pointer, IntPtr value);

        public void Add(GKPlayer value)
        {
            NSMutableArray_AddGKPlayer(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKLeaderboardSet
    public class NSArrayGKLeaderboardSet : NSArray<GKLeaderboardSet>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKLeaderboardSetAt(IntPtr pointer, int index);
        
        public override GKLeaderboardSet ElementAtIndex(int index)
        {
            return PointerCast<GKLeaderboardSet>(NSArray_GetGKLeaderboardSetAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKLeaderboardSet : NSArrayGKLeaderboardSet, INSMutableArray<GKLeaderboardSet> 
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKLeaderboardSet(IntPtr pointer, IntPtr value);
        
        public void Add(GKLeaderboardSet value)
        {
            NSMutableArray_AddGKLeaderboardSet(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKLeaderboard
    public class NSArrayGKLeaderboard : NSArray<GKLeaderboard>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKLeaderboardAt(IntPtr pointer, int index);
        
        public override GKLeaderboard ElementAtIndex(int index)
        {
            return PointerCast<GKLeaderboard>(NSArray_GetGKLeaderboardAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKLeaderboard : NSArrayGKLeaderboard, INSMutableArray<GKLeaderboard>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKLeaderboard(IntPtr pointer, IntPtr value);
        
        public void Add(GKLeaderboard value)
        {
            NSMutableArray_AddGKLeaderboard(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKLeaderboardEntry

    public class NSArrayGKLeaderboardEntry : NSArray<GKLeaderboard.Entry>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKLeaderboardEntryAt(IntPtr pointer, int index);
        
        public override GKLeaderboard.Entry ElementAtIndex(int index)
        {
            return PointerCast<GKLeaderboard.Entry>(NSArray_GetGKLeaderboardEntryAt(Pointer, index));
        }
    }

    public class NSMutableArrayGKLeaderboardEntry : NSArrayGKLeaderboardEntry, INSMutableArray<GKLeaderboard.Entry>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKLeaderboardEntry(IntPtr pointer, IntPtr value);
        
        public void Add(GKLeaderboard.Entry value)
        {
            NSMutableArray_AddGKLeaderboardEntry(Pointer, value.Pointer);
        }
    }
    #endregion
    
    #region GKChallenge
    public class NSArrayGKChallenge : NSArray<GKChallenge>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSArray_GetGKChallengeAt(IntPtr pointer, int index);
        
        public override GKChallenge ElementAtIndex(int index)
        {
            var challenge = PointerCast<GKChallenge>(NSArray_GetGKChallengeAt(Pointer, index));

            if (challenge != null)
            {
                switch (challenge.ChallengeType)
                {
                    case GKChallenge.GKChallengeType.Score:
#pragma warning disable 618
                        challenge = PointerCast<GKScoreChallenge>(challenge.Pointer);
#pragma warning restore 618
                        break;
                    case GKChallenge.GKChallengeType.Achievement:
                        challenge = PointerCast<GKAchievementChallenge>(challenge.Pointer);
                        break;
                }
            }

            return challenge;
        }
    }

    public class NSMutableArrayGKChallenge : NSArrayGKChallenge, INSMutableArray<GKChallenge>
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddGKChallenge(IntPtr pointer, IntPtr value);
        
        public void Add(GKChallenge value)
        {
            NSMutableArray_AddGKChallenge(Pointer, value.Pointer);
        }
    }
    #endregion
}