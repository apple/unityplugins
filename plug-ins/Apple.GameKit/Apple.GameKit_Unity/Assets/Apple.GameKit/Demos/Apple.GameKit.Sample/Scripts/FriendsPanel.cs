using System;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class FriendsPanel : MonoBehaviour
    {
        [SerializeField] private PlayerButton _friendButtonPrefab = default;
        [SerializeField] private GameObject _friendsListContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        void Start()
        {
            _refreshButton.onClick.AddListener(RefreshButtonAction);
        }

        async void OnEnable()
        {
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        private readonly bool AreFriendsApisAvailable =
            Availability.IsMethodAvailable<GKLocalPlayer>(nameof(GKLocalPlayer.LoadFriends)) &&
            Availability.IsMethodAvailable<GKLocalPlayer>(nameof(GKLocalPlayer.LoadFriendsAuthorizationStatus));

        private bool _useAccessPoint = false;
        private readonly bool IsAccessPointAvailable = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerWithPlayer));
        private readonly bool IsViewControllerAvailable = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithPlayer));

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                if (AreFriendsApisAvailable)
                {
                    var localPlayer = GKLocalPlayer.Local;
                    if (localPlayer != null)
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
                                    var button = Instantiate(_friendButtonPrefab, _friendsListContent.transform, worldPositionStays: false);
                                    button.Player = friend;
                                    button.ButtonClick += async (sender, args) =>
                                    {
                                        // Alternate using the view controller and access point APIs to show the dashboard.
                                        if ((_useAccessPoint || !IsViewControllerAvailable) && IsAccessPointAvailable)
                                        {
                                            await GKAccessPoint.Shared.TriggerWithPlayer(friend);
                                        }
                                        else if ((!_useAccessPoint || !IsAccessPointAvailable) && IsViewControllerAvailable)
                                        {
                                            var viewController = GKGameCenterViewController.InitWithPlayer(friend);
                                            await viewController.Present();
                                        }
                                        _useAccessPoint = !_useAccessPoint;
                                    };
                                }

                                // This is completely unnecessary except to verify that LoadFriends(NSArray<NSString> identifiers) returns the same result.
                                var identifiers = new NSMutableArray<NSString>(friends.Select(friend => new NSString(friend.GamePlayerId)));
                                var friends2 = await localPlayer.LoadFriends(identifiers);
                                if (friends2.Count == friends.Count)
                                {
                                    Debug.Log($"LoadFriends(identifiers) returned the correct number of results ({friends2.Count}).");
                                }
                                else
                                {
                                    Debug.LogError($"LoadFriends(identifiers) returned the wrong number of results (expected {friends.Count} but got {friends2.Count}).");
                                }
                            }
                            else
                            {
                                // Explain why the friends list might be empty.
                                var button = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                                button.Text = $"None of your friends have opted-in to Connect with Friends for this game in their Game Center profile.";
                            }
                        }
                        else
                        {
                            // show the authorization error
                            var button = Instantiate<ErrorButton>(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                            button.Text = $"{authStatus}";
                        }
                    }
                }
                else
                {
                    // show API not-available error
                    var button = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                    button.Text = $"LoadFriends and LoadFriendsAuthorizationStatus are not available on this OS version.";
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);

                // show the exception text
                var errorButton = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                errorButton.Text = $"{ex.Message}";

                // Add a helpful message about adding NSGKFriendListUsageDescription.
                var explanation = (ex as GameKitException)?.GKErrorCode.GetExplanatoryText();
                if (!string.IsNullOrWhiteSpace(explanation))
                {
                    var helpButton = Instantiate(_errorMessagePrefab, _friendsListContent.transform, worldPositionStays: false);
                    helpButton.Text = explanation;
                }
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
