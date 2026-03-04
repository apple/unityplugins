using System;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ChallengesPanel : PanelBase<ChallengesPanel>
    {
        [SerializeField] private ChallengeDefinitionButton _challengeDefinitionButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;
        [SerializeField] private ChallengeDefinitionPanel _challengeDefinitionPanelPrefab = default;

        private readonly bool IsLoadChallengeDefinitionsAvailable = Availability.IsMethodAvailable<GKChallengeDefinition>(nameof(GKChallengeDefinition.LoadChallengeDefinitions));

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

                if (IsLoadChallengeDefinitionsAvailable)
                {
                    var challengeDefinitions = await GKChallengeDefinition.LoadChallengeDefinitions();
                    if (challengeDefinitions != null && challengeDefinitions.Count > 0)
                    {
                        // Challenge definitions are ordered randomly. Sort them here.
                        var sortedChallengeDefinitions = challengeDefinitions.OrderBy(def => def.Title).ToList();

                        foreach (var challengeDefinition in sortedChallengeDefinitions)
                        {
                            var button = _challengeDefinitionButtonPrefab.Instantiate(_listContent, challengeDefinition);
                            button.ButtonClick += (sender, args) =>
                            {
                                var challengeDefinitionPanel = _challengeDefinitionPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, challengeDefinition);
                                GameKitSample.Instance.PushPanel(challengeDefinitionPanel.gameObject);
                            };
                        }
                    }
                }
                else
                {
                    // show the API unavailable message
                    var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                    errorButton.Text = $"LoadChallengeDefinitions is not available on this OS version.";
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
