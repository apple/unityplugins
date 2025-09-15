using System;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

namespace Apple.GameKit.Sample
{
    public class LeaderboardsPanel : PanelBase<LeaderboardsPanel>
    {
        [SerializeField] private LeaderboardButton _leaderboardButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;
        [SerializeField] private LeaderboardPanel _leaderboardPanelPrefab = default;
        [SerializeField] private Dropdown _defaultLeaderboardDropdown = default;

        private readonly bool IsLoadLeaderboardsAvailable = Availability.IsMethodAvailable<GKLeaderboard>(nameof(GKLeaderboard.LoadLeaderboards));

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

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                if (IsLoadLeaderboardsAvailable)
                {
                    var leaderboards = await GKLeaderboard.LoadLeaderboards();
                    if (leaderboards != null && leaderboards.Count > 0)
                    {
                        foreach (var leaderboard in leaderboards)
                        {
                            var button = _leaderboardButtonPrefab.Instantiate(_listContent, leaderboard);
                            button.ButtonClick += (sender, args) =>
                            {
                                var leaderboardPanel = _leaderboardPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, leaderboard);
                                GameKitSample.Instance.PushPanel(leaderboardPanel.gameObject);
                            };
                        }

                        // populate the default leaderboard id dropdown
                        _defaultLeaderboardDropdown.options = leaderboards
                            .OrderBy(lb => lb.BaseLeaderboardId)
                            .Select(lb => new OptionData { text = lb.BaseLeaderboardId })
                            .ToList();

                        // select the current default leaderboard
                        var defaultId = await GKLocalPlayer.Local.LoadDefaultLeaderboardIdentifier();
                        if (!string.IsNullOrWhiteSpace(defaultId))
                        {
                            var index = _defaultLeaderboardDropdown.options.FindIndex(od => od.text == defaultId);
                            if (index >= 0)
                            {
                                _defaultLeaderboardDropdown.value = index;
                            }
                        }

                        // react to selection changes
                        _defaultLeaderboardDropdown.onValueChanged.AddListener(async index => 
                        {
                            if (index >= 0 && index < _defaultLeaderboardDropdown.options.Count)
                            {
                                var defaultId = _defaultLeaderboardDropdown.options[index].text;
                                if (!string.IsNullOrWhiteSpace(defaultId))
                                {
                                    await GKLocalPlayer.Local.SetDefaultLeaderboardIdentifier(defaultId);
                                }
                            }
                        });
                    }
                }
                else
                {
                    // show the API unavailable message
                    var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                    errorButton.Text = $"LoadLeaderboards is not available on this OS version.";
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
            _defaultLeaderboardDropdown.ClearOptions();
            _defaultLeaderboardDropdown.onValueChanged.RemoveAllListeners();
        }
    }
}
