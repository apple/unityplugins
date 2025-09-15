using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ChallengeDefinitionPanel : PanelBase<ChallengeDefinitionPanel>
    {
        [SerializeField] private ChallengeDefinitionButton _challengeDefinitionButton = default;
        [SerializeField] private PropertyButton _propertyButtonPrefab = default;
        [SerializeField] private LeaderboardButton _leaderboardButtonPrefab = default;
        [SerializeField] private LeaderboardPanel _leaderboardPanelPrefab = default;
        [SerializeField] private GameObject _propertiesListContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        public GKChallengeDefinition ChallengeDefinition
        {
            get => _challengeDefinitionButton.ChallengeDefinition;
            set
            {
                _challengeDefinitionButton.ChallengeDefinition = value;
                _ = Refresh();
            }
        }

        public ChallengeDefinitionPanel Instantiate(GameObject parent, GKChallengeDefinition challengeDefinition)
        {
            var panel = base.Instantiate(parent);

            panel.ChallengeDefinition = challengeDefinition;

            return panel;
        }

        void Start()
        {
            _challengeDefinitionButton.ButtonClick += async (sender, args) =>
            {
                if (ChallengeDefinition != null)
                {
                    await GKAccessPoint.Shared.TriggerWithChallengeDefinitionID(ChallengeDefinition.Identifier);
                }
            };

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

        private bool Interactable
        {
            get
            {
                return
                    _challengeDefinitionButton.Interactable &&
                    _refreshButton.interactable;
            }

            set
            {
                _challengeDefinitionButton.Interactable = value;
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

                if (ChallengeDefinition != null)
                {
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Title", ChallengeDefinition.Title);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Identifier", ChallengeDefinition.Identifier);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Group Identifier", ChallengeDefinition.GroupIdentifier);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Details", ChallengeDefinition.Details);
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Release State", ChallengeDefinition.ReleaseState.ToString());
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "IsRepeatable", ChallengeDefinition.IsRepeatable ? "yes" : "no");

                    // duration options
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Duration Options", 
                        ChallengeDefinition.DurationOptions != null ?
                            string.Join("\n", ChallengeDefinition.DurationOptions
                                .Where(dateComponents => dateComponents.ValidDate)
                                .Select(dateComponents => dateComponents.Date.ToString("u"))) :
                            string.Empty);

                    // hasActiveChallenges
                    _propertyButtonPrefab.Instantiate(_propertiesListContent, "Has Active Challenges", (await ChallengeDefinition.HasActiveChallenges()) ? "yes" : "no");

                    // leaderboard
                    if (ChallengeDefinition.Leaderboard != null)
                    {
                        var button = _leaderboardButtonPrefab.Instantiate(_propertiesListContent, ChallengeDefinition.Leaderboard);
                        button.ButtonClick += (sender, args) =>
                        {
                            var leaderboardPanel = _leaderboardPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, ChallengeDefinition.Leaderboard);
                            GameKitSample.Instance.PushPanel(leaderboardPanel.gameObject);
                        };
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
