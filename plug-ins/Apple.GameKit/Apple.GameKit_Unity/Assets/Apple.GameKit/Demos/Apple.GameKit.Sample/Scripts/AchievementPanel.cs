using System;
using System.Linq;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementPanel : PanelBase<AchievementPanel>
    {
        [SerializeField] private AchievementButton _achievementButton = default;
        [SerializeField] private PropertyButton _propertyButtonPrefab = default;
        [SerializeField] private PropertyLabel _propertyLabelPrefab = default;
        [SerializeField] private GameObject _propertiesListContent = default;

        [SerializeField] private Button _reportButton = default;
        [SerializeField] private Button _refreshButton = default;

        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        private bool _useAccessPoint = false;
        private readonly bool IsAccessPointAvailable = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerWithAchievementID));
        private readonly bool IsViewControllerAvailable = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithAchievementID));

        GKAchievementDescription _achievementDescription;
        public GKAchievementDescription AchievementDescription
        {
            get => _achievementDescription;
            private set
            {
                _achievementDescription = value;
                _achievementButton.Description = value;
            }
        }

        GKAchievement _achievement;
        public GKAchievement Achievement
        {
            get => _achievement;
            private set
            {
                _achievement = value;
                _achievementButton.Achievement = value;
            }
        }

        public AchievementPanel Instantiate(GameObject parent, GKAchievementDescription achievementDescription, GKAchievement achievement)
        {
            var panel = base.Instantiate(parent);

            panel.AchievementDescription = achievementDescription;
            panel.Achievement = achievement;

            return panel;
        }

        void Start()
        {
            _achievementButton.ButtonClick += async (sender, args) =>
            {
                // Alternate using the view controller and access point APIs to show the dashboard.
                if ((_useAccessPoint || !IsViewControllerAvailable) && IsAccessPointAvailable)
                {
                    await GKAccessPoint.Shared.TriggerWithAchievementID(AchievementDescription.Identifier);
                }
                else if ((!_useAccessPoint || !IsAccessPointAvailable) && IsViewControllerAvailable)
                {
                    var viewController = GKGameCenterViewController.InitWithAchievementID(AchievementDescription.Identifier);
                    await viewController.Present();
                }
                _useAccessPoint = !_useAccessPoint;
            };

            _reportButton.onClick.AddListener(ReportButtonAction);
            _refreshButton.onClick.AddListener(RefreshButtonAction);

            Refresh();
        }

        void OnEnable()
        {
            Refresh();
        }

        private GKAchievement GetOrCreateAchievement()
        {
            var achievement = Achievement;
            if (achievement == null)
            {
                var description = AchievementDescription;
                if (description != null)
                {
                    achievement = GKAchievement.Init(AchievementDescription.Identifier);
                    Achievement = achievement;
                }
            }
            return achievement;
        }

        public async void ReportButtonAction()
        {
            var achievement = GetOrCreateAchievement();
            if (achievement != null)
            {
                achievement.PercentComplete = Math.Min(achievement.PercentComplete + 25.0, 100.0);

                try
                {
                    await GKAchievement.Report(new GKAchievement[] { achievement });
                }
                catch (Exception ex)
                {
                    GKErrorCodeExtensions.LogException(ex);

                    // show the exception text
                    var errorButton = _errorMessagePrefab.Instantiate(_propertiesListContent);
                    errorButton.Text = $"{ex.Message}";
                }

                Refresh();
            }
        }

        public void RefreshButtonAction()
        {
            Refresh();
        }

        private bool Interactable
        {
            get => _achievementButton.Interactable && _refreshButton.interactable;
            set
            {
                _achievementButton.Interactable = value;
                _refreshButton.interactable = value;
                _reportButton.interactable = value && !(Achievement?.IsCompleted ?? false);
            }
        }

        public void Refresh()
        {
            Interactable = false;

            try
            {
                Clear();

                var description = AchievementDescription;
                if (description != null)
                {
                    _propertyLabelPrefab.Instantiate(_propertiesListContent, nameof(GKAchievementDescription));

                    Action<string, Func<string>> addProperty = (name, getValue) =>
                    {
                        string value = Availability.IsPropertyAvailable<GKAchievementDescription>(name) ? $"{getValue()}" : "(not available)";
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, name, value);
                    };

                    addProperty(nameof(GKAchievementDescription.Identifier), () => $"{description.Identifier}");
                    addProperty(nameof(GKAchievementDescription.Title), () => $"{description.Title}");
                    addProperty(nameof(GKAchievementDescription.UnachievedDescription), () => $"{description.UnachievedDescription}");
                    addProperty(nameof(GKAchievementDescription.AchievedDescription), () => $"{description.AchievedDescription}");
                    addProperty(nameof(GKAchievementDescription.GroupIdentifier), () => $"{description.GroupIdentifier}");
                    addProperty(nameof(GKAchievementDescription.MaximumPoints), () => $"{description.MaximumPoints}");
                    addProperty(nameof(GKAchievementDescription.IsHidden), () => $"{description.IsHidden}");
                    addProperty(nameof(GKAchievementDescription.IsReplayable), () => $"{description.IsReplayable}");
                    addProperty(nameof(GKAchievementDescription.RarityPercent), () => $"{description.RarityPercent:F1}");
                    addProperty(nameof(GKAchievementDescription.ActivityIdentifier), () => $"{description.ActivityIdentifier}");
                    addProperty(nameof(GKAchievementDescription.ActivityProperties), () =>
                        description.ActivityProperties != null ?
                            string.Join("\n        ", description.ActivityProperties
                                .OrderBy(kvp => kvp.Key)
                                .Select(kvp => $"\"{kvp.Key}\" : \"{kvp.Value}\"")) :
                            string.Empty);
                    addProperty(nameof(GKAchievementDescription.ReleaseState), () => $"{description.ReleaseState}");

                    _achievementButton.Description = description;
                }

                var achievement = GetOrCreateAchievement();
                if (achievement != null)
                {
                    _propertyLabelPrefab.Instantiate(_propertiesListContent, nameof(GKAchievement));

                    Action<string, Func<string>> addProperty = (name, getValue) =>
                    {
                        string value = Availability.IsPropertyAvailable<GKAchievement>(name) ? $"{getValue()}" : "(not available)";
                        _propertyButtonPrefab.Instantiate(_propertiesListContent, name, value);
                    };

                    addProperty(nameof(GKAchievement.Identifier), () => $"{achievement.Identifier}");
                    addProperty(nameof(GKAchievement.Player), () => $"{achievement.Player?.DisplayName ?? string.Empty}");
                    addProperty(nameof(GKAchievement.PercentComplete), () => $"{achievement.PercentComplete:F1}");
                    addProperty(nameof(GKAchievement.IsCompleted), () => $"{achievement.IsCompleted}");
                    addProperty(nameof(GKAchievement.LastReportedDate), () => $"{achievement.LastReportedDate:u}");
                    addProperty(nameof(GKAchievement.ShowCompletionBanner), () => $"{achievement.ShowCompletionBanner}");

                    _achievementButton.Achievement = achievement;
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
        }

        private void Clear()
        {
            DestroyChildren(_propertiesListContent);
        }
    }
}
