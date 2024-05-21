using System;

namespace Apple.GameKit
{
    public enum GKFriendsAuthorizationStatus : int
    {
        // User has not yet made a choice with regards to this application.
        // A call to loadFriendsListWithHandler: in this state will result
        // into a prompt that might pause your application.
        NotDetermined = 0,

        // This application is not authorized to use friend list data.  Due
        // to active restrictions on friend list data, the user cannot change
        // this status, and may not have personally denied authorization.
        // If you have previously collected data for this player's friend list,
        // You should delete the data collected on your end.
        Restricted = 1,

        // User has explicitly denied this application access to friend list data,
        // or global friend list access are disabled in Settings.
        Denied = 2,

        // User has authorized this application to access friend list data.
        // A call to loadFriends: will return the player's
        // friend list via a completion block
        Authorized = 3
    }
}
