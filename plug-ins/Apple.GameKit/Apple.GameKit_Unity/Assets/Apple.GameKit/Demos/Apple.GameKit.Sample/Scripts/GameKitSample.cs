using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[assembly:Preserve]

namespace Apple.GameKit.Sample
{
    public class GameKitSample : MonoBehaviour
    {
        [SerializeField] private RawImage _playerPhotoImage = default;

        [SerializeField] private Text _playerDisplayName = default;
        [SerializeField] private Text _isMultiplayerGamingRestrictedText = default;
        [SerializeField] private Text _isPersonalizedCommunicationRestrictedText = default;
        [SerializeField] private Text _isUnderageText = default;

        [SerializeField] private GameObject _backButtonArea = default;
        [SerializeField] private GameObject _panelArea = default;

        [SerializeField] private GameObject _mainButtonLayout = default;
        [SerializeField] private AccessPointPanel _accessPointPanel = default;
        [SerializeField] private FriendsPanel _friendsPanel = default;
        [SerializeField] private AchievementsPanel _achievementsPanel = default;
        [SerializeField] private NearbyPlayersPanel _nearbyPlayersPanel = default;
        [SerializeField] private LeaderboardSetsPanel _leaderboardSetsPanel = default;
        [SerializeField] private LeaderboardsPanel _leaderboardsPanel = default;
        [SerializeField] private TurnBasedMatchesPanel _turnBasedMatchesPanel = default;
        [SerializeField] private RealtimeMatchRequestPanel _realtimeMatchRequestPanel = default;
        [SerializeField] private RealtimeMatchStatusPanel _realtimeMatchStatusPanel = default;

#pragma warning disable CS0414 // prevent unused variable warnings on tvOS
        [SerializeField] private SavedGamesPanel _savedGamesPanel = default;
#pragma warning disable CS0414

        [SerializeField] private Button _authenticateBtn = default;
        [SerializeField] private Text _authenticateBtnText = default;

        [SerializeField] private CanvasGroup _otherButtonsGroup = default;

        [SerializeField] private Button _accessPointButton = default;
        [SerializeField] private Button _friendsButton = default;
        [SerializeField] private Button _showAchievementsBtn = default;
        [SerializeField] private Button _leaderboardSetsButton = default;
        [SerializeField] private Button _leaderboardsButton = default;
        [SerializeField] private Button _nearbyPlayersButton = default;
        [SerializeField] private Button _turnBasedMatchesButton = default;
        [SerializeField] private Button _realtimeMatchmakingButton = default;
        [SerializeField] private Button _savedGamesButton = default;

        private GKLocalPlayer _localPlayer;
        private bool _hasHookedAuthenticateEvents = false;

        public static GameKitSample Instance { get; private set; }

        private void Start()
        {
            Instance = this;

            try
            {
                // Send Unity log messages to NSLog.
                _ = new AppleLogger();

                _authenticateBtn.onClick.AddListener(OnAuthenticate);
                _accessPointButton.onClick.AddListener(OnShowAccessPointPanel);
                _friendsButton.onClick.AddListener(OnShowFriendsPanel);
                _showAchievementsBtn.onClick.AddListener(OnShowAchievements);
                _leaderboardSetsButton.onClick.AddListener(OnShowLeaderboardSets);
                _leaderboardsButton.onClick.AddListener(OnShowLeaderboards);
                _nearbyPlayersButton.onClick.AddListener(OnShowNearbyPlayersPanel);
                _turnBasedMatchesButton.onClick.AddListener(OnShowTurnBasedMatches);
                _realtimeMatchmakingButton.onClick.AddListener(OnRealtimeMatchmaking);
#if UNITY_TVOS
                // Saved games not available on tvOS.
                _savedGamesButton.gameObject.SetActive(false);
#else
                _savedGamesButton.onClick.AddListener(OnShowSavedGames);
#endif
                _authenticateBtn.interactable = true;
                _otherButtonsGroup.interactable = false;

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

                if (!_hasHookedAuthenticateEvents)
                {
                    _hasHookedAuthenticateEvents = true;
                    GKLocalPlayer.AuthenticateUpdate += OnAuthenticateUpdate;
                    GKLocalPlayer.AuthenticateError += OnAuthenticateError;
                }

                await GKLocalPlayer.Authenticate();
            }
            catch (GameKitException)
            {
                // Suppress GameKitExceptions here because we'll handle them as errors in OnAuthenticateError.
            }
            catch (Exception ex)
            {
                // Any other kind of exception is fatal.
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private async void OnAuthenticateUpdate(GKLocalPlayer localPlayer)
        {
            await HandleAuthenticateUpdate(localPlayer);
        }

        private async Task HandleAuthenticateUpdate(GKLocalPlayer localPlayer)
        {
            _localPlayer = localPlayer;

            Debug.Log($"GameKit authentication update: isAuthenticated => user {_localPlayer.DisplayName} {(_localPlayer.IsAuthenticated ? "is" : "is NOT")} authenticated");

            if (_localPlayer != null && _localPlayer.IsAuthenticated)
            {
                _authenticateBtnText.text = "Authenticated";
                _authenticateBtn.interactable = false;
                _otherButtonsGroup.interactable = true;

                _playerDisplayName.text = _localPlayer.DisplayName;

                try
                {
                    var texture = await _localPlayer.LoadPhoto(GKPlayer.PhotoSize.Normal);
                    _playerPhotoImage.texture = (texture != null) ? texture : Texture2D.whiteTexture;
                }
                catch (Exception ex)
                {
                    GKErrorCodeExtensions.LogException(ex);
                }

                _isMultiplayerGamingRestrictedText.text = $"IsMultiplayerGamingRestricted: {_localPlayer.IsMultiplayerGamingRestricted}";
                _isUnderageText.text = $"IsUnderage: {_localPlayer.IsUnderage}";

                var commsRestrictedResult = Availability.IsPropertyAvailable<GKLocalPlayer>(nameof(GKLocalPlayer.IsPersonalizedCommunicationRestricted)) ? _localPlayer.IsPersonalizedCommunicationRestricted.ToString() : "n/a";
                _isPersonalizedCommunicationRestrictedText.text = $"IsPersonalizedCommunicationRestricted: {commsRestrictedResult}";

                await TestFetchItemsForIdentityVerificationSignature();
            }
            else
            {
                // user is not authenticated
                _authenticateBtnText.text = "Not Authenticated";
                _authenticateBtn.interactable = true;
                _otherButtonsGroup.interactable = false;

                // force a return to the main page
                ReturnToRootPanel();
            }
        }

        private async void OnAuthenticateError(NSError error)
        {
            await HandleAuthenticateError(error);
        }

        private async Task HandleAuthenticateError(NSError error)
        {
            // Authentication will fail when running in the Unity Editor because Game Center will think it's an 
            // unregistered app. This is because Game Center sees Unity's bundle id rather than the app's bundle id.
            // This is normal, but not ideal during development. To at least allow some of the sample app UI to work
            // during development, we'll look for this specific failure case and then still allow the cached instance of
            // GKLocalPlayer to be used.
            if (Application.isEditor && error.Domain == GKErrorDomain.Name)
            {
                var code = (GKErrorCode)error.Code;
                if (code == GKErrorCode.GameUnrecognized || code == GKErrorCode.NotAuthenticated)
                {
                    await HandleAuthenticateUpdate(GKLocalPlayer.Local);
                    return;
                }
            }

            Debug.Log($"GameKit authentication error: Code={error.Code} Domain={error.Domain} Description={error.LocalizedDescription}");

            _authenticateBtnText.text = "Authentication Error";
            _authenticateBtn.interactable = true;
            _otherButtonsGroup.interactable = false;
        }

        private async Task TestFetchItemsForIdentityVerificationSignature()
        {
            if (Availability.IsMethodAvailable<GKLocalPlayer>(nameof(GKLocalPlayer.FetchItemsForIdentityVerificationSignature)))
            {
                var items = await GKLocalPlayer.Local.FetchItemsForIdentityVerificationSignature();
                Debug.Log(
                    "GKLocalPlayer.FetchItemsForIdentityVerificationSignature:\n" + 
                    $"  PublicKeyUrl={items.PublicKeyUrl}\n" + 
                    $"  Signature={Convert.ToBase64String(items.GetSignature())} ({items.Signature.Length} bytes)\n" + 
                    $"  Salt={Convert.ToBase64String(items.GetSalt())} ({items.Salt.Length} bytes)\n" +
                    $"  Timestamp={items.Timestamp}\n");
            }
        }

        private Stack<GameObject> _panelStack;
        private Stack<GameObject> PanelStack => _panelStack ??= new Stack<GameObject>();

        public GameObject PanelArea => _panelArea;

        public void PushPanel(GameObject panel)
        {
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Peek() : null;
            if (oldPanel != null)
            {
                oldPanel.SetActive(false);
            }

            PanelStack.Push(panel);
            panel.SetActive(true);

            _backButtonArea.SetActive(PanelStack.Count > 1);
        }

        public void PopPanel()
        {
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Pop() : null;
            if (oldPanel != null)
            {
                var panelBase = oldPanel.GetComponent<PanelBase>();
                if (panelBase != null && panelBase.ShouldDestroyWhenPopped)
                {
                    panelBase.Destroy();
                }
                else
                {
                    oldPanel.SetActive(false);
                }
            }

            if (PanelStack.Count > 0)
            {
                PanelStack.Peek().SetActive(true);
            }

            _backButtonArea.SetActive(PanelStack.Count > 1);
        }

        public void ReplaceActivePanel(GameObject panel)
        {
            // Important: Deactivate last because it might end the script if the
            // caller is deactivating itself.
            var oldPanel = (PanelStack.Count > 0) ? PanelStack.Pop() : null;

            PanelStack.Push(panel);
            panel.SetActive(true);

            if (oldPanel != null)
            {
                var panelBase = oldPanel.GetComponent<PanelBase>();
                if (panelBase != null && panelBase.ShouldDestroyWhenPopped)
                {
                    panelBase.Destroy();
                }
                else
                {
                    oldPanel.SetActive(false);
                }
            }

            _backButtonArea.SetActive(PanelStack.Count > 1);
        }

        public void ReturnToRootPanel()
        {
            while (PanelStack.Count > 1)
            {
                PopPanel();
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

        private void OnShowAccessPointPanel()
        {
            PushPanel(_accessPointPanel.gameObject);
        }

        private void OnShowFriendsPanel()
        {
            PushPanel(_friendsPanel.gameObject);
        }

        private void OnShowAchievements()
        {
            PushPanel(_achievementsPanel.gameObject);
        }

        private void OnShowNearbyPlayersPanel()
        {
            PushPanel(_nearbyPlayersPanel.gameObject);
        }

        private void OnShowLeaderboardSets()
        {
            PushPanel(_leaderboardSetsPanel.Instantiate(_panelArea).gameObject);
        }

        private void OnShowLeaderboards()
        {
            PushPanel(_leaderboardsPanel.Instantiate(_panelArea).gameObject);
        }

        private void OnShowTurnBasedMatches()
        {
            PushPanel(_turnBasedMatchesPanel.gameObject);
        }

#if !UNITY_TVOS
        private void OnShowSavedGames()
        {
            PushPanel(_savedGamesPanel.gameObject);
        }
#endif
    }
}
