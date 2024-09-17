using System;
using System.Threading.Tasks;
using Apple.Core;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class LeaderboardSetPanel : PanelBase<LeaderboardSetPanel>
    {
        [SerializeField] private LeaderboardButton _leaderboardButtonPrefab = default;
        [SerializeField] private LeaderboardSetButton _leaderboardSetButton = default;
        [SerializeField] private GameObject _listContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;
        [SerializeField] private LeaderboardPanel _leaderboardPanelPrefab = default;

        public GKLeaderboardSet LeaderboardSet
        {
            get => _leaderboardSetButton.LeaderboardSet;
            set
            {
                _leaderboardSetButton.LeaderboardSet = value;
                _ = Refresh();
            }
        }

        private bool _useAccessPoint = false;
        private readonly bool IsAccessPointAvailable = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerWithLeaderboardSetID));
        private readonly bool IsViewControllerAvailable = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithLeaderboardSetID));

        private readonly bool IsLoadLeaderboardsAvailable = Availability.IsMethodAvailable<GKLeaderboard>(nameof(GKLeaderboard.LoadLeaderboards));

        void Start()
        {
            if (IsAccessPointAvailable || IsViewControllerAvailable)
            {
                _leaderboardSetButton.ButtonClick += async (sender, args) =>
                {
                    // Alternate using the view controller and access point APIs to show the dashboard.
                    if ((_useAccessPoint || !IsViewControllerAvailable) && IsAccessPointAvailable)
                    {
                        await GKAccessPoint.Shared.TriggerWithLeaderboardSetID(LeaderboardSet.Identifier);
                    }
                    else if ((!_useAccessPoint || !IsAccessPointAvailable) && IsViewControllerAvailable)
                    {
                        var viewController = GKGameCenterViewController.InitWithLeaderboardSetID(LeaderboardSet.Identifier);
                        await viewController.Present();
                    }
                    _useAccessPoint = !_useAccessPoint;
                };
            }

            _refreshButton.onClick.AddListener(RefreshButtonAction);

            ShouldDestroyWhenPopped = IsPrefabInstance;
        }

        async void OnEnable()
        {
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                if (LeaderboardSet != null)
                {
                    if (IsLoadLeaderboardsAvailable)
                    {
                        var leaderboards = await LeaderboardSet.LoadLeaderboards();
                        if (leaderboards != null && leaderboards.Count > 0)
                        {
                            foreach (var leaderboard in leaderboards)
                            {
                                var button = _leaderboardButtonPrefab.Instantiate(_listContent);
                                button.Leaderboard = leaderboard;
                                button.ButtonClick += (sender, args) =>
                                {
                                    var leaderboardPanel = _leaderboardPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea);
                                    leaderboardPanel.Leaderboard = leaderboard;
                                    GameKitSample.Instance.PushPanel(leaderboardPanel.gameObject);
                                };
                            }
                        }
                    }
                    else
                    {
                        // show API unavailable message
                        var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                        errorButton.Text = $"LoadLeaderboards is not available on this OS version.";
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
                _refreshButton.interactable = true;
            }
        }

        public void Clear()
        {
            DestroyChildren(_listContent);
        }
    }
}
