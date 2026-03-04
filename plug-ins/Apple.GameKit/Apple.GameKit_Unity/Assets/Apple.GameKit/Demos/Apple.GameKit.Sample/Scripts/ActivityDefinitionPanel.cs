using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivityDefinitionPanel : PanelBase<ActivityDefinitionPanel>
    {
        [SerializeField] private ActivityDefinitionButton _activityDefinitionButton = default;
        [SerializeField] private PropertyButton _propertyButtonPrefab = default;
        [SerializeField] private LeaderboardButton _leaderboardButtonPrefab = default;
        [SerializeField] private LeaderboardPanel _leaderboardPanelPrefab = default;
        [SerializeField] private AchievementButton _achievementButtonPrefab = default;
        [SerializeField] private AchievementPanel _achievementPanelPrefab = default;
        [SerializeField] private ActivityPanel _activityPanelPrefab = default;
        [SerializeField] private GameObject _propertiesListContent = default;

        [SerializeField] private GameObject _bottomButtonArea = default;
        [SerializeField] private Button _initButton = default;
        [SerializeField] private Button _startButton = default;
        [SerializeField] private Button _refreshButton = default;

        [SerializeField] private GameObject _partyCodeArea = default;
        [SerializeField] private InputField _partyCodeInputField = default;
        [SerializeField] private Button _startPartyCodeButton = default;
        [SerializeField] private Button _cancelPartyCodeButton = default;

        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        private Color _defaultTextInputColor = Color.black;

        public GKGameActivityDefinition ActivityDefinition
        {
            get => _activityDefinitionButton.ActivityDefinition;
            set
            {
                _activityDefinitionButton.ActivityDefinition = value;
                _ = Refresh();
            }
        }

        public ActivityDefinitionPanel Instantiate(GameObject parent, GKGameActivityDefinition activityDefinition)
        {
            var panel = base.Instantiate(parent);

            panel.ActivityDefinition = activityDefinition;

            return panel;
        }

        // smash the valid alphabet characters into a single string
        static string _validPartyCodeAlphabet = null;
        static string ValidPartyCodeAlphabet => _validPartyCodeAlphabet ??= string.Join(null, GKGameActivity.ValidPartyCodeAlphabet.Select(s => s.ToString()));

        void Start()
        {
            #if UNITY_IOS || UNITY_STANDALONE_OSX
            _activityDefinitionButton.ButtonClick += async (sender, args) =>
            {
                if (ActivityDefinition != null)
                {
                    await GKAccessPoint.Shared.TriggerWithGameActivityDefinitionID(ActivityDefinition.Identifier);
                }
            };
            #endif

            _initButton.onClick.AddListener(InitButtonAction);
            _startButton.onClick.AddListener(StartButtonAction);
            _refreshButton.onClick.AddListener(RefreshButtonAction);

            _startPartyCodeButton.onClick.AddListener(StartPartyCodeButtonAction);
            _cancelPartyCodeButton.onClick.AddListener(CancelPartyCodeButtonAction);

            _defaultTextInputColor = _partyCodeInputField.textComponent.color;
            _startPartyCodeButton.interactable = false;

            // Party code should be two parts of strings with the same length (2-6) connected with a dash, 
            // and the code can be either pure digits (0-9), or both parts are uppercased characters from validPartyCodeAlphabet.
            StringBuilder builder = new StringBuilder();
            const int PartyCodeLengthWithoutDash = 8; // must be even and between 4 and 12
            for (int i = 0; i < PartyCodeLengthWithoutDash; i++)
            {
                builder.Append(ValidPartyCodeAlphabet[UnityEngine.Random.Range(0, ValidPartyCodeAlphabet.Length - 1)]);

                // insert the dash at the halfway point
                if (i == PartyCodeLengthWithoutDash / 2 - 1)
                {
                    builder.Append('-');
                }
            }

            _partyCodeInputField.onValueChanged.AddListener(value =>
            {
                var isValid = GKGameActivity.IsValidPartyCode(_partyCodeInputField.text);
                _startPartyCodeButton.interactable = isValid;
                _partyCodeInputField.textComponent.color = isValid ? _defaultTextInputColor : Color.red;
            });

            _partyCodeInputField.onValidateInput += (input, charIndex, addedChar) =>
                (ValidPartyCodeAlphabet.Contains(addedChar) || addedChar == '-') ? addedChar : '\0';

            _partyCodeInputField.text = builder.ToString();
        }

        async void OnEnable()
        {
            ShowPartyCodeControls(false);

            await Refresh();
        }

        public void InitButtonAction()
        {
            var activity = GKGameActivity.Init(ActivityDefinition);
            var activityPanel = _activityPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, activity);
            GameKitSample.Instance.PushPanel(activityPanel.gameObject);
        }

        public void StartButtonAction()
        {
            var definition = ActivityDefinition;
            if (definition.SupportsPartyCode)
            {
                // show the party code controls
                ShowPartyCodeControls(true);
            }
            else
            {
                // start the activity
                var activity = GKGameActivity.Start(definition);
                var activityPanel = _activityPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, activity);
                GameKitSample.Instance.PushPanel(activityPanel.gameObject);
            }
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        private void ShowPartyCodeControls(bool show)
        {
            _partyCodeArea.SetActive(show);
            _bottomButtonArea.SetActive(!show);
        }

        public void StartPartyCodeButtonAction()
        {
            var activity = GKGameActivity.Start(ActivityDefinition, _partyCodeInputField.text);
            var activityPanel = _activityPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, activity);
            GameKitSample.Instance.PushPanel(activityPanel.gameObject);
        }

        public void CancelPartyCodeButtonAction()
        {
            ShowPartyCodeControls(false);
        }

        private bool Interactable
        {
            get
            {
                return
                    _activityDefinitionButton.Interactable &&
                    _refreshButton.interactable;
            }

            set
            {
                _activityDefinitionButton.Interactable = value;
                _refreshButton.interactable = value;
            }
        }

        public async Task<int> Refresh()
        {
            int numEntries = 0;

            Interactable = false;

            try
            {
                Clear();

                if (ActivityDefinition != null)
                {
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Title", ActivityDefinition.Title);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Identifier", ActivityDefinition.Identifier);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "GroupIdentifier", ActivityDefinition.GroupIdentifier);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Details", ActivityDefinition.Details);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "ReleaseState", ActivityDefinition.ReleaseState.ToString());
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Fallback URL", ActivityDefinition.FallbackURL?.ToString() ?? string.Empty);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "PlayStyle", ActivityDefinition.PlayStyle.ToString());
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "SupportsPartyCode", ActivityDefinition.SupportsPartyCode ? "yes" : "no");
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "SupportsUnlimitedPlayers", ActivityDefinition.SupportsUnlimitedPlayers ? "yes" : "no");
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "MaxPlayers", ActivityDefinition.MaxPlayers?.ToString() ?? "(undefined)");
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "MinPlayers", ActivityDefinition.MinPlayers?.ToString() ?? "(undefined)");

                    // duration options
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "DefaultProperties", 
                        ActivityDefinition.DefaultProperties != null ?
                            string.Join("\n", ActivityDefinition.DefaultProperties
                                .Select(kvp => $"\"{kvp.Key}\": \"{kvp.Value}\"")) :
                            string.Empty);

                    // achievements
                    var achievementDescriptions = await ActivityDefinition.LoadAchievementDescriptions();
                    if (achievementDescriptions?.Count > 0)
                    {
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, "Achievement Descriptions Count", achievementDescriptions.Count.ToString());
                        foreach (var description in achievementDescriptions)
                        {
                            var button = _achievementButtonPrefab.Instantiate(_propertiesListContent, description);
                            button.ButtonClick += (sender, args) =>
                            {
                                var achievementPanel = _achievementPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, description, null);
                                GameKitSample.Instance.PushPanel(achievementPanel.gameObject);
                            };

                        }
                    }

                    // leaderboards
                    var leaderboards = await ActivityDefinition.LoadLeaderboards();
                    if (leaderboards?.Count > 0)
                    {
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, "Leaderboards Count", leaderboards.Count.ToString());
                        foreach (var leaderboard in leaderboards)
                        {
                            var button = _leaderboardButtonPrefab.Instantiate(_propertiesListContent, leaderboard);
                            button.ButtonClick += (sender, args) =>
                            {
                                var leaderboardPanel = _leaderboardPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, leaderboard);
                                GameKitSample.Instance.PushPanel(leaderboardPanel.gameObject);
                            };
                        }
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
