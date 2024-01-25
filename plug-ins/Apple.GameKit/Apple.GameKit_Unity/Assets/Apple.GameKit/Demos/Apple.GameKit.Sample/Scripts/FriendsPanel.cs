using System;
using System.Threading.Tasks;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class FriendsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _friendPanelPrefab = default;
        [SerializeField] private GameObject _friendsListContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private GameObject _errorMessagePrefab = default;

        async void OnEnable()
        {
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        private bool AreFriendsApisAvailable =>
            Availability.Available(RuntimeOperatingSystem.macOS, 11, 3) ||
            Availability.Available(RuntimeOperatingSystem.iOS, 14, 5) ||
            Availability.Available(RuntimeOperatingSystem.tvOS, 14, 5);

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                var localPlayer = GKLocalPlayer.Local;
                if (localPlayer != null && AreFriendsApisAvailable)
                {
                    var authStatus = await localPlayer.LoadFriendsAuthorizationStatus();
                    if (authStatus == GKFriendsAuthorizationStatus.Authorized ||
                        authStatus == GKFriendsAuthorizationStatus.NotDetermined)
                    {
                        var friends = await localPlayer.LoadFriends();
                        if (friends.Count > 0)
                        {
                            foreach (var friend in friends)
                            {
                                var panelObject = Instantiate(_friendPanelPrefab, _friendsListContent.transform, worldPositionStays: false);

                                var panel = panelObject.GetComponent<FriendPanel>();
                                panel.Player = friend;
                            }

                            // This is completely unnecessary except to verify that LoadFriends(NSArray<NSString> identifiers) returns the same result.
                            var identifiers = new NSMutableArray<NSString>();
                            foreach (var friend in friends)
                            {
                                identifiers.Add(friend.GamePlayerId);
                            }
                            var friends2 = await localPlayer.LoadFriends(identifiers);
                            if (friends2.Count != friends.Count)
                            {
                                throw new Exception("LoadFriends(identifiers) returned the wrong number of results.");
                            }
                        }
                    }
                    else
                    {
                        // show the authorization error
                        var errorObject = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                        var panel = errorObject.GetComponent<FriendErrorPanel>();
                        panel.Text = $"{authStatus}";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                // show the exception text
                var errorObject = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                var errorPanel = errorObject.GetComponent<FriendErrorPanel>();
                errorPanel.Text = $"{ex.Message}";

                // Add a helpful message about adding NSGKFriendListUsageDescription.
                var helpObject = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                var helpPanel = helpObject.GetComponent<FriendErrorPanel>();
                helpPanel.Text = $"If the exception mentions NSGKFriendListUsageDescription, you can set this string in the Unity Editor by choosing Build Settings... -> Player Settings -> Apple Build Settings -> GameKit -> Friend List Usage Description.";
            }
            finally
            {
                _refreshButton.interactable = true;
            }
        }

        public void Clear()
        {
            foreach (Transform transform in _friendsListContent.transform)
            {
                Destroy(transform.gameObject);
            }
            _friendsListContent.transform.DetachChildren();
        }
    }
}
