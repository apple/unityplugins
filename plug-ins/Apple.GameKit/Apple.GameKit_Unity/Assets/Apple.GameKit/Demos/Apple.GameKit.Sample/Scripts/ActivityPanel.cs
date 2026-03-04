using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivityPanel : PanelBase<ActivityPanel>
    {
        [SerializeField] private ActivityDefinitionButton _activityDefinitionButton = default;
        [SerializeField] private ActivityDefinitionButton _activityButton = default;
        [SerializeField] private AchievementPanel _achievementPanelPrefab = default;
        [SerializeField] private AchievementButton _achievementButtonPrefab = default;
        [SerializeField] private ActivityAchievementButtons _activityAchievementButtonsPrefab = default;

        [SerializeField] private LeaderboardPanel _leaderboardPanelPrefab = default;
        [SerializeField] private LeaderboardButton _leaderboardButtonPrefab = default;
        [SerializeField] private ActivityLeaderboardScoreButtons _activityLeaderboardScoreButtonsPrefab = default;

        [SerializeField] private PropertyButton _propertyButtonPrefab = default;
        [SerializeField] private GameObject _propertiesListContent = default;

        [SerializeField] private Button _startButton = default;
        [SerializeField] private Button _pauseButton = default;
        [SerializeField] private Button _resumeButton = default;
        [SerializeField] private Button _endButton = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private Button _removeAchievementsButton = default;
        [SerializeField] private Button _removeLeaderboardScoresButton = default;

        [SerializeField] private Button _makeMatchRequestButton = default;
        [SerializeField] private Button _findMatchButton = default;
        [SerializeField] private Button _findPlayersForHostedMatch = default;

        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        private GKGameActivity _activity;
        public GKGameActivity Activity
        {
            get => _activity;
            set
            {
                _activity = value;
                _ = Refresh();
            }
        }

        public ActivityPanel Instantiate(GameObject parent, GKGameActivity activity)
        {
            var panel = base.Instantiate(parent);

            panel.Activity = activity;

            return panel;
        }

        void Start()
        {
            #if UNITY_IOS || UNITY_STANDALONE_OSX
            _activityDefinitionButton.ButtonClick += async (sender, args) =>
            {
                if (Activity?.ActivityDefinition != null)
                {
                    await GKAccessPoint.Shared.TriggerWithGameActivityDefinitionID(Activity.ActivityDefinition.Identifier);
                }
            };
            #endif

            _activityButton.ButtonClick += async (sender, args) =>
            {
                if (Activity != null)
                {
                    await GKAccessPoint.Shared.TriggerWithGameActivity(Activity);
                }
            };

            _startButton.onClick.AddListener(StartButtonAction);
            _pauseButton.onClick.AddListener(PauseButtonAction);
            _resumeButton.onClick.AddListener(ResumeButtonAction);
            _endButton.onClick.AddListener(EndButtonAction);

            _refreshButton.onClick.AddListener(RefreshButtonAction);

            _removeAchievementsButton.onClick.AddListener(RemoveAchievementsButtonAction);
            _removeLeaderboardScoresButton.onClick.AddListener(RemoveLeaderboardScoresButtonAction);

            _makeMatchRequestButton.onClick.AddListener(MakeMatchRequestButtonAction);
            _findMatchButton.onClick.AddListener(FindMatchButtonAction);
            _findPlayersForHostedMatch.onClick.AddListener(FindPlayersForHostedMatchButtonAction);
        }

        async void OnEnable()
        {
            await Refresh();
        }

        // For an activity to support multiplayer, it must have a party code
        // with a minimum number of players specified.
        private bool SupportsMultiplayer
        {
            get
            {
                var definition = Activity?.ActivityDefinition;
                return
                    definition != null &&
                    definition.SupportsPartyCode &&
                    definition.MinPlayers.HasValue &&
                    (definition.MaxPlayers.HasValue || definition.SupportsUnlimitedPlayers);
            }
        }

        public void UpdateButtonsForCurrentState()
        {
            var state = Activity?.State ?? GKGameActivityState.Initialized;

            switch (state)
                {
                    case GKGameActivityState.Initialized:
                        _startButton.gameObject.SetActive(true);
                        _pauseButton.gameObject.SetActive(false);
                        _resumeButton.gameObject.SetActive(false);
                        _endButton.gameObject.SetActive(false);
                        _removeAchievementsButton.gameObject.SetActive(false);
                        _removeLeaderboardScoresButton.gameObject.SetActive(false);
                        _makeMatchRequestButton.gameObject.SetActive(false);
                        _findMatchButton.gameObject.SetActive(false);
                        _findPlayersForHostedMatch.gameObject.SetActive(false);
                        break;
                    case GKGameActivityState.Active:
                        _startButton.gameObject.SetActive(false);
                        _pauseButton.gameObject.SetActive(true);
                        _resumeButton.gameObject.SetActive(false);
                        _endButton.gameObject.SetActive(true);
                        _removeAchievementsButton.gameObject.SetActive(true);
                        _removeLeaderboardScoresButton.gameObject.SetActive(true);
                        _makeMatchRequestButton.gameObject.SetActive(SupportsMultiplayer);
                        _findMatchButton.gameObject.SetActive(SupportsMultiplayer);
                        _findPlayersForHostedMatch.gameObject.SetActive(SupportsMultiplayer);
                        break;
                    case GKGameActivityState.Paused:
                        _startButton.gameObject.SetActive(false);
                        _pauseButton.gameObject.SetActive(false);
                        _resumeButton.gameObject.SetActive(true);
                        _endButton.gameObject.SetActive(true);
                        _removeAchievementsButton.gameObject.SetActive(true);
                        _removeLeaderboardScoresButton.gameObject.SetActive(true);
                        _makeMatchRequestButton.gameObject.SetActive(SupportsMultiplayer);
                        _findMatchButton.gameObject.SetActive(SupportsMultiplayer);
                        _findPlayersForHostedMatch.gameObject.SetActive(SupportsMultiplayer);
                        break;
                    case GKGameActivityState.Ended:
                        _startButton.gameObject.SetActive(false);
                        _pauseButton.gameObject.SetActive(false);
                        _resumeButton.gameObject.SetActive(false);
                        _endButton.gameObject.SetActive(false);
                        _removeAchievementsButton.gameObject.SetActive(false);
                        _removeLeaderboardScoresButton.gameObject.SetActive(false);
                        _makeMatchRequestButton.gameObject.SetActive(false);
                        _findMatchButton.gameObject.SetActive(false);
                        _findPlayersForHostedMatch.gameObject.SetActive(false);
                        break;
                }
        }

        public async void StartButtonAction()
        {
            Activity.Start();
            await Refresh();
        }

        public async void PauseButtonAction()
        {
            Activity.Pause();
            await Refresh();
        }

        public async void ResumeButtonAction()
        {
            Activity.Resume();
            await Refresh();
        }

        public async void EndButtonAction()
        {
            Activity.End();
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        public async void RemoveAchievementsButtonAction()
        {
            Interactable = false;

            try
            {
                Activity.RemoveAchievements(Activity.AchievementsAsArray);
                await Refresh();
            }
            finally
            {
                Interactable = true;
            }
        }

        public async void RemoveLeaderboardScoresButtonAction()
        {
            Interactable = false;

            try
            {
                var leaderboards = await Activity.ActivityDefinition.LoadLeaderboards();
                if (leaderboards?.Count > 0)
                {
                    Activity.RemoveScoresFromLeaderboards(leaderboards);
                    await Refresh();
                }
            }
            finally
            {
                Interactable = true;
            }
        }

        public void MakeMatchRequestButtonAction()
        {
            var request = Activity.MakeMatchRequest();

            // TODO: This test was not completed in time for 2025 Beta 1.
            Debug.Log($"GKGameActivity.MakeMatchRequest() -> {request}");
        }

        public async void FindMatchButtonAction()
        {
            var match = await Activity.FindMatch();

            // TODO: This test was not completed in time for 2025 Beta 1.
            Debug.Log($"GKGameActivity.FindMatch() -> {match}");
        }

        public async void FindPlayersForHostedMatchButtonAction()
        {
            var players = await Activity.FindPlayersForHostedMatch();

            // TODO: This test was not completed in time for 2025 Beta 1.
            Debug.Log($"GKGameActivity.FindPlayersForHostedMatch() -> {players}");
        }

        private bool Interactable
        {
            get
            {
                return
                    _activityDefinitionButton.Interactable &&
                    _refreshButton.interactable &&
                    _activityButton.Interactable;
            }

            set
            {
                _activityDefinitionButton.Interactable = value;
                _refreshButton.interactable = value;
                _activityButton.Interactable = value;
            }
        }

        public async Task<int> Refresh()
        {
            int numEntries = 0;

            Interactable = false;

            try
            {
                Clear();

                UpdateButtonsForCurrentState();

                var activity = Activity;
                if (activity != null)
                {
                    var definition = activity.ActivityDefinition;
                    _activityDefinitionButton.ActivityDefinition = definition;
                    _activityButton.ActivityDefinition = definition;

                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Identifier", activity.Identifier);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "State", activity.State.ToString());
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Duration", activity.Duration.ToString("g"));
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "CreationDate", activity.CreationDate.ToString("u"));
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "StartDate", activity.StartDate.ToString("u"));
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "EndDate", activity.EndDate.ToString("u"));
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "LastResumeDate", activity.LastResumeDate.ToString("u"));
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "PartyCode", activity.PartyCode);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "PartyURL", activity.PartyURL?.ToString());

                    // achievements
                    var achievementDescriptions = await definition.LoadAchievementDescriptions();
                    if (achievementDescriptions?.Count > 0)
                    {
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, "Achievement Count", achievementDescriptions.Count.ToString());

                        var achievements = activity.AchievementsAsArray;
                        foreach (var achievementDescription in achievementDescriptions)
                        {
                            var achievement = achievements?
                                .Where(ach => ach.Identifier == achievementDescription.Identifier)
                                .FirstOrDefault() ??
                                GKAchievement.Init(achievementDescription.Identifier);

                            var button = _achievementButtonPrefab.Instantiate(_propertiesListContent, achievementDescription, achievement);
                            button.ButtonClick += (sender, args) =>
                            {
                                var achievementPanel = _achievementPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, achievementDescription, achievement);
                                GameKitSample.Instance.PushPanel(achievementPanel.gameObject);
                            };

                            if ((activity.State == GKGameActivityState.Active || activity.State == GKGameActivityState.Paused) && !achievement.IsCompleted)
                            {
                                var buttons = _activityAchievementButtonsPrefab.Instantiate(_propertiesListContent, activity, achievement);
                                buttons.ProgressChanged += (sender, args) =>
                                {
                                    button.UpdateAchievementProperties();
                                };
                            }
                        }
                    }

                    // leaderboards
                    var leaderboards = await definition.LoadLeaderboards();
                    if (leaderboards?.Count > 0)
                    {
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, "Leaderboard Count", leaderboards.Count.ToString());

                        var scores = activity.LeaderboardScoresAsArray;
                        foreach (var leaderboard in leaderboards)
                        {
                            var score = scores?
                                .Where(score => score.LeaderboardID == leaderboard.BaseLeaderboardId)
                                .FirstOrDefault();

                            var button = _leaderboardButtonPrefab.Instantiate(_propertiesListContent, leaderboard);
                            button.ButtonClick += (sender, args) =>
                            {
                                var leaderboardPanel = _leaderboardPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, leaderboard);
                                GameKitSample.Instance.PushPanel(leaderboardPanel.gameObject);
                            };

                            if (activity.State == GKGameActivityState.Active || activity.State == GKGameActivityState.Paused)
                            {
                                var buttons = _activityLeaderboardScoreButtonsPrefab.Instantiate(_propertiesListContent, activity, leaderboard);
                                buttons.ScoreChanged += (sender, args) =>
                                {
                                    // nothing to do
                                };
                            }
                        }
                    }

                    var properties = activity.Properties;
                    if (properties?.Count > 0)
                    {
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, "Properties",
                            string.Join("\n", properties.Select(kvp => $"\"{kvp.Key}\": \"{kvp.Value}\"")));
                    }
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);

                // show the exception text
                var errorButton = _errorMessagePrefab.Instantiate(_propertiesListContent);
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
            DestroyChildren(_propertiesListContent);
        }
    }
}
