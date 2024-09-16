using System;
using System.Threading.Tasks;
using Apple.Core;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class LeaderboardPanel : PanelBase<LeaderboardPanel>
    {
        [SerializeField] private LeaderboardButton _leaderboardButton = default;

        [SerializeField] private LeaderboardEntryButton _leaderboardEntryButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;
        
        [SerializeField] private Dropdown _playerScopeDropdown = default;
        [SerializeField] private Dropdown _timeScopeDropdown = default;

        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private Button _prevButton = default;
        [SerializeField] private Button _nextButton = default;
        
        [SerializeField] private Button _submitScoreButton = default;

        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        public GKLeaderboard Leaderboard
        {
            get => _leaderboardButton.Leaderboard;
            set
            {
                _leaderboardButton.Leaderboard = value;
                _ = Refresh();
            }
        }

        public int FirstEntry { get; set; } = 1;
        public int NumEntriesPerPage { get; set; } = 100;

        [field: NonSerialized]
        public GKLeaderboard.PlayerScope PlayerScope { get; set; } = GKLeaderboard.PlayerScope.Global; // or FriendsOnly
        
        [field: NonSerialized]
        public GKLeaderboard.TimeScope TimeScope { get; set; } = GKLeaderboard.TimeScope.AllTime; // or Today or Week

        private bool _useAccessPoint = false;
        private readonly bool IsAccessPointAvailable = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerWithLeaderboardID));
        private readonly bool IsViewControllerAvailable = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithLeaderboardID));

        private readonly bool IsViewControllerAvailableForPlayer = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithPlayer));

        void Start()
        {
            _leaderboardButton.ButtonClick += async (sender, args) =>
            {
                // Alternate using the view controller and access point APIs to show the dashboard.
                if ((_useAccessPoint || !IsViewControllerAvailable) && IsAccessPointAvailable)
                {
                    await GKAccessPoint.Shared.TriggerWithLeaderboardID(Leaderboard.BaseLeaderboardId, PlayerScope, TimeScope);
                }
                else if ((!_useAccessPoint || !IsAccessPointAvailable) && IsViewControllerAvailable)
                {
                    var viewController = GKGameCenterViewController.InitWithLeaderboardID(Leaderboard.BaseLeaderboardId, PlayerScope, TimeScope);
                    await viewController.Present();
                }
                _useAccessPoint = !_useAccessPoint;
            };

            _refreshButton.onClick.AddListener(RefreshButtonAction);
            _prevButton.onClick.AddListener(PrevButtonAction);
            _nextButton.onClick.AddListener(NextButtonAction);
            _submitScoreButton.onClick.AddListener(SubmitScoreAction);

            ShouldDestroyWhenPopped = IsPrefabInstance;
        }

        async void OnEnable()
        {
            _playerScopeDropdown.value = (int)PlayerScope;
            _timeScopeDropdown.value = (int)TimeScope;
            await Refresh();
        }

        public async void OnPlayerScopeChanged(Int32 newValue)
        {
            PlayerScope = (GKLeaderboard.PlayerScope)newValue;
            await Refresh();
        }

        public async void OnTimeScopeChanged(Int32 newValue)
        {
            TimeScope = (GKLeaderboard.TimeScope)newValue;
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        public async void PrevButtonAction()
        {
            FirstEntry = Math.Max(1, FirstEntry - NumEntriesPerPage);
            await Refresh();
        }

        public async void NextButtonAction()
        {
            FirstEntry += NumEntriesPerPage;
            var numEntries = await Refresh();
            if (numEntries == 0)
            {
                // We have gone beyond the end of the list. Go back to the previous position.
                FirstEntry = Math.Max(1, FirstEntry - NumEntriesPerPage);
                await Refresh();
            }
        }

        private long _scoreIncrement = 100;
        private long _lastScoreSubmitted = 0;

        public async void SubmitScoreAction()
        {
            if (Leaderboard != null)
            {
                try
                {
                    Interactable = false;

                    _lastScoreSubmitted += _scoreIncrement;
                    await Leaderboard.SubmitScore(_lastScoreSubmitted, 0, GKLocalPlayer.Local);
                    await Refresh();
                }
                finally
                {
                    Interactable = true;
                }
            }
        }

        private bool Interactable
        {
            get
            {
                return
                    _leaderboardButton.Interactable &&
                    _refreshButton.interactable;
            }

            set
            {
                _leaderboardButton.Interactable = value;
                _refreshButton.interactable = value;
                _prevButton.interactable = value && FirstEntry > 1;
                _nextButton.interactable = value;
                _submitScoreButton.interactable = value;
            }
        }

        public async Task<int> Refresh()
        {
            int numEntries = 0;

            Interactable = false;

            try
            {
                Clear();

                if (Leaderboard != null)
                {
                    var response = await Leaderboard.LoadEntries(PlayerScope, TimeScope, FirstEntry, FirstEntry + NumEntriesPerPage - 1);
                    if (response.Entries.Count > 0)
                    {
                        numEntries = response.Entries.Count;
                        foreach (var entry in response.Entries)
                        {
                            var button = _leaderboardEntryButtonPrefab.Instantiate(_listContent, entry);
                            if (IsViewControllerAvailableForPlayer)
                            {
                                button.ButtonClick += async (sender, args) =>
                                {
                                    var viewController = GKGameCenterViewController.InitWithPlayer(button.Player);
                                    await viewController.Present();
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);

                // show the exception text
                var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                errorButton.Text = $"{ex.Message}";
            }
            finally
            {
                Interactable = true;
            }

            return numEntries;
        }

        private void Clear()
        {
            DestroyChildren(_listContent);
        }
    }
}
