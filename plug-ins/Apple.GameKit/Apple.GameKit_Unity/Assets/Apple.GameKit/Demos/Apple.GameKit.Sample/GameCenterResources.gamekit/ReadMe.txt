This GameCenterResources.gamekit folder is called a GameKit bundle.

The bundle contains definitions for the achievements, activities, challenges,
and leaderboards, and leaderboard sets used by the GameKit sample app.

Drag the entire GameCenterResources.gamekit bundle folder into your Xcode 
project after you export it from Unity.

Then you can use Xcode to push the bundle contents to App Store Connect.

You only need to do this once. The achievement, activity, challenge, 
leaderboard, and leaderboard set definitions will remain on App Store Connect
even when you re-export your Xcode project from Unity.

If you use Xcode to change achievement, activity, challenge or leaderboard
definitions or images, then you will need to push the bundle to App Store
Connect again to update the definitions on the server.

You can also use a GameKit bundle to test locally with the Game Progress
Manager in Xcode without having to upload to App Store Connect. For more
information see:
https://developer.apple.com/documentation/gamekit/creating-engaging-challenges-from-leaderboards#Test-challenges-by-using-the-Game-Progress-Manager

Or:
https://developer.apple.com/documentation/gamekit/creating-activities-for-your-game#Test-activities-by-using-the-Game-Progress-Manager