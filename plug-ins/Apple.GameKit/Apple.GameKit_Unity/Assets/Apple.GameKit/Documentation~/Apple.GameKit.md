# Apple - GameKit

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Usage
Since most calls to GameKit are asynchronous, the public methods are `Task`, or `Task<>` based. For a comprehensive guide to GameKit on Apple devices, please see [GameKit Developer Documentation](https://developer.apple.com/documentation/gamekit/)

### Exceptions
If there is any error reported from GameKit, it will be reported by throwing a `GameKitException`. In all cases, a `try-catch` should be used to properly handle exceptions.

```C#
private async Task Start()
{
  try
  {
    var result = await ...
  }
  catch(GameKitException exception)
  {
    Debug.LogError(exception);
  }
}
```

## Table of Contents
1. [Players](#1-players)
2. [Achievements](#2-achievements)
3. [GKGameCenterViewController](#3-ui-dialogs)
4. [Realtime Matchmaking](#4-realtimematchmaking)
5. [Turn Based Matchmaking](#5-turnbasedmatchmaking)
6. [Leaderboards](#6-leaderboards)
7. [Access Point](#7-accesspoint)
8. [Challenges](#8-challenges)
9. [Activities](#9-activities)
10. [Invites](#10-invites)

### 1. Players
##### [GKLocalPlayer - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gklocalplayer)

#### 1.1 Authentication
```csharp
var player = await GKLocalPlayer.Authenticate();
Debug.Log($"GameKit Authentication: isAuthenticated => {player.IsAuthenticated}");
```

> [!IMPORTANT]
> Before calling `GKLocalPlayer.Authenticate()` make sure that callbacks related to
> user being authenticated are hooked up (such as `GKChallenge.ChallengeReceived`) to 
> ensure no events are missed.

Use the following events to be notified of changes to the local player's authentication status after the initial request completes.
```csharp
GKLocalPlayer.AuthenticateSuccess += (GKLocalPlayer localPlayer) {
  // handle newly authenticated (or re-authenticated) player
}
GKLocalPlayer.AuthenticateError += (NSError error) {
  // handle authentication error
}
```

#### 1.2 Fetch Local Player
**Note:** This call is not asynchronous.
```csharp
var localPlayer = GKLocalPlayer.Local;
Debug.Log($"Local Player: {localPlayer.DisplayName}");

if(!localPlayer.IsUnderage) { 
  // Ask for analytics permissions, etc.
}
```

#### 1.3 Loading Player Photo
Each call to LoadPlayerPhoto generates a new Texture2D so ensure you cache as necessary per your own use-case.
```csharp
var player = GKLocalPlayer.Local;

// Resolves a new instance of the players photo as a Texture2D.
var photo = await player.LoadPhoto(size);
```

#### 1.4 Friends
```csharp
// Loads the local player's friends list if the local player and their friends grant access.
var friends = await GKLocalPlayer.Local.LoadFriends();

// Loads players to whom the local player can issue a challenge.
var challengeableFriends = await GKLocalPlayer.Local.LoadChallengeableFriends();

// Loads players from the friends list or players that recently participated in a game with the local player.
var recentPlayers = await GKLocalPlayer.Local.LoadRecentPlayers();
```

#### 1.5 FetchItemsForIdentityVerificationSignature
##### [FetchItemsForIdentityVerificationSignature - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gklocalplayer/3516283-fetchitems)

```csharp
var fetchItemsResponse = await GKLocalPlayer.Local.FetchItemsForIdentityVerificationSignature();

var signature = fetchItemsResponse.GetSignature(); 
var salt = fetchItemsResponse.GetSalt();
var publicKeyUrl = fetchItemsResponse.PublicKeyUrl;
var timestamp = fetchItemsResponse.Timestamp;
```

### 2. Achievements
##### [GKAchievement - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkachievement)
##### [GKAchievementDescription - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkachievementdescription)

#### 2.1 List All Achievements
**Note:** This only returns achievements with progress that the player has reported. Use GKAchievementDescription for a list of all available achievements.

```csharp
var achievements = await GKAchievement.LoadAchievements();

foreach (var a in achievements) 
{
  Debug.Log($"Achievement: {a.Identifier}");
}
```

#### 2.2 List Descriptions
```csharp
var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();

foreach (var d in descriptions) 
{
  Debug.Log($"Achievement: {d.Identifier}, Unachieved Description: {d.UnachievedDescription}, Achieved Description: {d.AchievedDescription}");
}
```

#### 2.3 Load Image
```csharp
var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();

foreach (var d in descriptions) 
{
  var image = await d.LoadImage();
  // Do something with the image.
}
```

#### 2.4 Report Progress 
```csharp
var achievementId = "a001";
var progressPercentage = 100;
var showCompletionBanner = true;

var achievements = await GKAchievement.LoadAchievements();

// Only completed achievements are returned.
var achievement = achievements.FirstOrDefault(a => a.Identifier == achievementId);

// If null, initialize it
achievement ??= GKAchievement.Init(achievementId);

if(!achievement.IsCompleted) {
    achievement.PercentComplete = progressPercentage;
    achievement.ShowCompletionBanner = showCompletionBanner;

    // Accepts a param GKAchievement[] for reporting multiple achievements.
    await GKAchievement.Report(achievement, ...);
}

```

#### 2.5 Reset All Achievements
```csharp
await GKAchievement.ResetAchievements();
```

### 3 GKGameCenterViewController
##### [GKGameCenterViewController - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkgamecenterviewcontroller)

#### 3.1 Show Achievements, Leaderboards and LeaderboardSets, Challenges, and Player Details Dialog
The Task will resolve when the dialog has been closed.
```csharp
var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Achievement);
// await for user to dismiss...
await gameCenter.Present();
```

### 4. Realtime Matchmaking
##### [GKMatch - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkmatch)
##### [GKMatchRequest - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkmatchrequest)

#### 4.1 Events
```csharp
var matchRequest = GKMatchRequest.Init();
matchRequest.MinPlayers = 2;
matchRequest.MaxPlayers = 2;

GKMatch match = await GKMatchmakerViewController.Request(matchRequest);

match.Delegate.DataReceived += OnMatchDataReceived;
match.Delegate.DataReceivedForPlayer += OnMatchDataReceivedForPlayer;
match.Delegate.DidFailWithError += OnMatchErrorReceived;
match.Delegate.PlayerConnectionChanged += OnMatchPlayerConnectionChanged;
match.Delegate.ShouldReinviteDisconnectedPlayer += OnShouldReinviteDisconnectedPlayer;

private void OnMatchDataReceived(byte[] data, GKPlayer fromPlayer)
{
  // Process data
}

private void OnMatchDataReceivedForPlayer(byte[] data, GKPlayer forRecipient, GKPlayer fromPlayer)
{
  // Process data
}

private void OnMatchErrorReceived(GameKitException exception)
{
  // Handle error
}

private void OnMatchPlayerConnectionChanged(GKPlayer player, GKPlayerConnectionState state)
{
  // Handle state change
}

private bool OnShouldReinviteDisconnectedPlayer(GKPlayer player)
{
  // Reinvite disconnected player
}
```

#### 4.2 Request Match
```csharp
var request = GKMatchRequest.Init();

request.MinPlayers = 1;
request.MaxPlayers = 4;
request.PlayerAttributes = 0;
request.PlayerGroup = 0;
request.RestrictToAutomatch = false;

// If using rule-based matchmaking...
request.QueueName = "NameOfYourMatchmakerQueue";

request.Properties = GKMatchProperties.FromJson(jsonPropertyBagToSendToServer); 
// -or-
request.Properties = new NSMutableDictionary<NSString, NSObject> { 
  { "YourPropertyNameHere", new NSNumber(3.14159) },
  { "AnotherPropertyName", new NSString("some string value") }
};
```

##### 4.2.1 Request using native OS UI
```csharp
var match = await GKMatchmakerViewController.Request(request);
```

##### 4.2.2 Request without using native OS UI.
```csharp
// A match based on the GKMatchRequest.
var match = await GKMatchmaker.Shared.FindMatch(request);

// A match from an accepted invite.
var matchForInvite = await GKMatchmaker.Shared.Match(invite);

// Initiates a request to find players for a hosted match.
var players = await GKMatchmaker.Shared.FindPlayers(request);

// Initiates a request to find players for a hosted match via rule-based matchmaking.
GKMatchedPlayers matchedPlayers = GKMatchmaker.Shared.FindMatchedPlayers(request);

// Invites additional players to an existing match...
await GKMatchmaker.Shared.AddPlayers(match, request);

// Finds the number of players, across player groups, who recently requested a match.
var numMatchRequests = await GKMatchmaker.Shared.QueryActivity();

// Finds the number of players, across player groups, who recently requested a match via the specified rule-based matchmaking queue.
var numMatchRequests = await GKMatchmaker.Shared.QueryQueueActivity("NameOfYourQueue");

// Finds the number of players in a player group who recently requested a match.
var numMatchRequestsInGroup = await GKMatchmaker.Shared.QueryPlayerGroupActivity(playerGroupId);

// Cancels a matchmaking request.
GKMatchmaker.Shared.Cancel();

// Cancels a pending invitation to another player.
GKMatchmaker.Shared.CancelPendingInvite(playerBeingInvited);
```

#### 4.3 Disconnect
```csharp
match.Disconnect();
```

#### 4.4 Send To All
Sends a message to all players
```csharp
var data = Encoding.UTF8.GetBytes("Hello World");
match.Send(data, GKMatch.GKSendDataMode.Reliable);
```

#### 4.5 Send To Players
Sends a message to the selected players
```csharp
var players = new GKPlayer[] { ... };
var data = Encoding.UTF8.GetBytes("Hello World");
match.SendToPlayers(data, players, GKMatch.GKSendDataMode.Reliable);
```

### 5. Turn Based Matchmaking
##### [GKTurnBasedMatch - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkturnbasedmatch)

#### 5.1 Events
```csharp
GKTurnBasedMatch.TurnEventReceived += OnTurnEnded;
GKTurnBasedMatch.MatchEnded += OnMatchEnded;

private void OnTurnEnded(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive)
{
  // Handle turn ended
}

private void OnMatchEnded(GKPlayer player, GKTurnBasedMatch match)
{
  // Handle match ended
}
```

#### 5.2 List all TurnBasedMatches for Player
```csharp
var turnBasedMatches = await GKTurnBasedMatch.LoadMatches();

foreach (var match in turnBasedMatches)
{
  Debug.Log($"TurnBasedMatch: {match.MatchId}");
}
```

#### 5.3 Find Match with User Interaction (GKTurnBasedMatchmakerViewController)
This call creates a GKMatchRequest with the specified parameters and passes it into a GKTurnBasedMatchmakerViewController. Once a user has selected or created a new match,
the Task<TurnBasedMatch> will resolve. If the user cancels, or an error occurs, a GameKitException will be thrown.
```csharp
var request = GKMatchRequest.Init();
request.MinPlayers = 2;
request.MaxPlayers = 2;

var match = await GKTurnBasedMatchmakerViewController.Request(request);
```

#### 5.4 Match Status Callbacks
##### 5.4.1 End Turn
When the local player takes their turn, they can pass the updated TurnBasedMatch to the EndTurn method.
```csharp
[System.Serializable]
public class GameData
{
  public int PlayerId;
  public int Count;
};

var formatter = new BinaryFormatter();
using (var stream = new MemoryStream())
{
  var gameData = new GameData()
  {
    PlayerId = 0,
    Count = 1
  };

  formatter.Serialize(stream, gameData);
  var data = stream.ToArray();

  await match.EndTurn(data);
}
```

##### 5.4.2 End Match
```csharp
[System.Serializable]
public class GameData
{
  public int PlayerId;
  public int Count;
};

var formatter = new BinaryFormatter();
using (var stream = new MemoryStream())
{
  var gameData = new GameData()
  {
    PlayerId = 0,
    Count = 1
  };

  formatter.Serialize(stream, gameData);
  var data = stream.ToArray();

  // Set the outcomes...
  foreach (var participant in match.Participants) {
    participant.MatchOutcome = Outcome.Won;
  }

  await match.EndMatch(data);
}
```
#### 5.5 Exchanges
##### [GKTurnBasedExchange - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkturnbasedexchange)

##### 5.5.1 Delegates
```csharp
GKTurnBasedMatch.ExchangeReceived += OnExchangeReceived;
GKTurnBasedMatch.ExchangeCompleted += OnExchangeCompleted;
GKTurnBasedMatch.ExchangeCanceled += OnExchangeCanceled;

private void OnExchangeReceived(GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match) {

}

private void OnExchangeCompleted(GKPlayer player, NSArray<GKTurnBasedExchangeReply> replies, GKTurnBasedExchange exchange, GKTurnBasedMatch match) {

}

private void OnExchangeCanceled(GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match) {

}
```

##### 5.5.2 Send Exchange
```csharp
// Creates and sends the exchange to the participants
var exchange = await turnBasedMatch.SendExchange(participants, data, localizableMessageKey, arguments, GKTurnBasedMatch.ExchangeTimeoutDefault);
```

##### 5.5.3 Replies
```csharp
// Cancel
await exchange.Cancel(localizableMessageKey, arguments);

// Reply
await exchange.Reply(localizableMessageKey, arguments, data);
```

##### 5.5.4 Complete and Wrap-up Exchange
```csharp
// Saves the merged data for the current turn without ending the turn
await turnBasedMatch.SaveMergedMatch(data, exchanges);
```

### 6. Leaderboards
##### [GKLeaderboard - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkleaderboard)

#### 6.1 Report Score
```csharp
var leaderboardId = "MyLeaderboard";
var score = 100;
var context = 0;

// Filter leadboards by params string[] identifiers
var leaderboards = await GKLeaderboard.LoadLeaderboards("MyLeaderboard");
var leaderboard = leaderboards.FirstOrDefault();

// Submit
await leaderboard.SubmitScore(score, context, GKLocalPlayer.Local);
```

#### 6.2 Load Leaderboards
```csharp
var allLeaderboards = await GKLeaderboard.LoadLeaderboards();
var filteredLeaderboards = await GKLeaderboard.LoadLeaderboards("lb1", "lb3");
```

#### 6.3 Load Scores
Using a leaderboard reference, you can load the entries:

```csharp
var playerScope = GKLeaderboard.PlayerScope.Global;
var timeScope = GKLeaderboard.TimeScope.AllTime;
var rankMin = 1;
var rankMax = 100;

var scores = await leaderboard.LoadEntries(playerScope, timeScope, rankMin, rankMax);
```

#### 6.4 Load Leaderboard Image
```csharp
// Unsupported on tvOS
var image = await leaderboard.LoadImage();
```

### 7. Access Point
##### [GKAccessPoint - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkaccesspoint)

#### 7.1 Game Center Visibility
If you need to check whether the GameCenter / AccessPoint overlay is currently visible, you can query the following API call.
```csharp
private void Update()
{
  if(GKAccessPoint.Shared.IsPresentingGameCenter) 
  {
    // Disable controllers
  }
}
```

#### 7.2 Show & Hide
**Note:** This call is not asynchronous.
```csharp
GKAccessPoint.Shared.Location = GKAccessPoint.GKAccessPointLocation.TopLeading;
GKAccessPoint.Shared.ShowHighlights = true; 
GKAccessPoint.Shared.IsActive = true; // or false to hide.
```

#### 7.3 Trigger
**Note:** This call is asynchronous as of Apple.GameKit version 3.0.0.
```csharp
await GKAccessPoint.Shared.Trigger();
```

Trigger also supports triggering for different scenarios. See the `TriggerWith*` and `TriggerFor*` methods that match the [trigger instance methods](https://developer.apple.com/documentation/gamekit/gkaccesspoint#Instance-Methods) in GameKit.
```csharp
await GKAccessPoint.Shared.TriggerWithChallengeDefinitionId(GKChallengeDefinition.Identifier)
```

### 8. Challenges
##### [Creating engaging challenges from leaderboards - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/creating-engaging-challenges-from-leaderboards)
Note: The older [`GKChallenge`](https://developer.apple.com/documentation/gamekit/gkchallenge) has been deprecated.

#### 8.1 Get the object that represents the challenge
```csharp
// Load all challenges for a game.
var challengeDefinitions = await GKChallengeDefinition.LoadChallengeDefinitions();
const string challengeID = "com.example.mygame.challenge.sprint";
const string leaderboardID = "com.example.mygame.leaderboard.highscore";

// Find a challenge definition by using an identifier.
var challenge = challengeDefinitions?.Where(def => def.Identifier == challengeID).FirstOrDefault();

// Find a leaderboard you associate with a challenge.
var leaderboard = challengeDefinitions?.Where(def => def.Leaderboard?.BaseLeaderboardId == leaderboardID).FirstOrDefault();
```

#### 8.2 Create a challenge
Present the default system UI that shows the available challenges for your game.
```csharp
// Show the system UI to show a list of available challenges the player selects from.
await GKAccessPoint.Shared.TriggerForPlayTogether();
```

Present the system UI for a specific challenge.
```csharp
// Show the system UI to create a challenge based on the configuration.
await GKAccessPoint.Shared.TriggerWithChallengeDefinitionID(challenge.Identifier);
```

### 9. Activities
##### [Creating activities for your game - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/creating-activities-for-your-game)

#### 9.1 Get the object that represents the activity
```csharp
// Load the game’s activities.
var activityDescriptions = await GKGameActivityDefinition.LoadGameActivityDefinitions();
const string activityID = "com.example.mygame.score_attack_mode";

// Find an activity by using an identifier.
var activityDescription = activityDescriptions?.Where(act => act.Identifier == activityID).FirstOrDefault();
```
Load the associated leaderboards or achievements that you configure to use with the activity:
```csharp
// Load the resources associated with the activity.
var achievementDescription = await activityDescription.LoadAchievementDescriptions();
var leaderboards = await activityDescription.LoadLeaderboards();
```

#### 9.2 Handle deep linking through activity listener
```csharp
GKGameActivity.WantsToPlay += async (GKPlayer player, GKGameActivity activity) =>
{
    if (activity.Identifier == "com.example.mygame.score_attack_mode")
    {
        await StartScoreAttackMode(activity);
        return true;
    }
    else if (activity.Identifier == "com.example.mygame.versus_mode")
    {
        await StartMultiplayerMode(activity, activity.PartyCode);
        return true;
    }

    return false;
};
```
```csharp
// Start matchmaking to find a match.
var match = await activity.FindMatch();
sampleGameUI.StartGame(match);
```

#### 9.3 Start a game activity life cycle
```csharp
// Start the activity with a party code.
var activity = GKGameActivity.Start(activityDescription, partyCode: "2345-CFGH");
```
Setting a score on a leaderboard or progress on an achievement.
```csharp
Leaderboards.GKLeaderboard leaderboard = null;
GKAchievement achievement = null;
// Set the score on a leaderboard for the local player.
long score = 100;
ulong context = 1;
activity.SetScoreOnLeaderboard(leaderboard, score, context);

// Set the progress on an achievement to 60 percent.
activity.SetProgressOnAchievement(achievement, percentComplete: 60);
```

#### 9.4 Report progress for an activity
```csharp
// Start the activity
public void Init(GKGameActivity activity)
{
    this.activity = activity;
    activity.Start();
}

// Handle tracking score updates at the local level.
public long CurrentScore
{
    get => activity.GetScoreOnLeaderboard(leaderboard)?.Value ?? 0;
    set => activity.SetScoreOnLeaderboard(leaderboard, value);
}

// End the activity to submit the score on your behalf.
void Deinit()
{
    activity.End();
}        
```

### 10. Invites
##### [GKInvite - Apple Developer Documentation](https://developer.apple.com/documentation/gamekit/gkinvite)

#### 10.1 Checking for Accepted Invites on Start
```csharp
GKInvite.InviteAccepted += OnInviteAccepted;

public void OnInviteAccepted(GKPlayer invitedPlayer, GKInvite invite) {

}
```

