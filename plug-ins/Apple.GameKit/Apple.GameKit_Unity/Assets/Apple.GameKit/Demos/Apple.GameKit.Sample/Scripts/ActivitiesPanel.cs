using System;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivitiesPanel : PanelBase<ActivitiesPanel>
    {
        [SerializeField] private Text _hasPendingActivityText = default;
        [SerializeField] private ActivityDefinitionButton _activityDefinitionButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;
        [SerializeField] private ActivityDefinitionPanel _activityDefinitionPanelPrefab = default;

        private readonly bool IsLoadGameActivityDefinitionsAvailable = Availability.IsMethodAvailable<GKGameActivityDefinition>(nameof(GKGameActivityDefinition.LoadGameActivityDefinitions));

        private string _hasPendingActivityFormat = "Has Pending Activities: {0}";

        void Start()
        {
            _hasPendingActivityFormat = _hasPendingActivityText.text;
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

                _hasPendingActivityText.text = string.Format(_hasPendingActivityFormat, await GKGameActivity.CheckPendingGameActivityExistence() ? "YES" : "NO");

                if (IsLoadGameActivityDefinitionsAvailable)
                {
                    var activityDefinitions = await GKGameActivityDefinition.LoadGameActivityDefinitions();
                    if (activityDefinitions != null && activityDefinitions.Count > 0)
                    {
                        // Activity definitions are ordered randomly. Sort them here.
                        var sortedActivityDefinitions = activityDefinitions.OrderBy(def => def.Title).ToList();

                        foreach (var activityDefinition in sortedActivityDefinitions)
                        {
                            var button = _activityDefinitionButtonPrefab.Instantiate(_listContent);
                            button.ActivityDefinition = activityDefinition;
                            button.ButtonClick += (sender, args) =>
                            {
                                var activityDefinitionPanel = _activityDefinitionPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, activityDefinition);
                                GameKitSample.Instance.PushPanel(activityDefinitionPanel.gameObject);
                            };
                        }
                    }
                }
                else
                {
                    // show the API unavailable message
                    var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                    errorButton.Text = $"LoadGameActivityDefinitions is not available on this OS version.";
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
