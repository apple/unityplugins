using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementsPanel : MonoBehaviour
    {
        [SerializeField] private AchievementButton _achievementButtonPrefab = default;
        [SerializeField] private AchievementPlaceholderImageButton _achievementPlaceholderImageButtonPrefab = default;
        [SerializeField] private GameObject _achievementsListContent = default;
        [SerializeField] private Text _propertiesText = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private Button _reportButton = default;
        [SerializeField] private Button _resetAllButton = default;
        [SerializeField] private Button _showViewControllerButton = default;

        private bool _useAccessPoint = false;
        private readonly bool IsAccessPointAvailable = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerWithAchievementID));
        private readonly bool IsViewControllerAvailable = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithAchievementID));

        private readonly bool IsViewControllerAvailableForAchievements = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithState));

        private readonly bool IsRarityPropertyAvailable = Availability.IsPropertyAvailable<GKAchievementDescription>(nameof(GKAchievementDescription.RarityPercent));

        void Start()
        {
            _reportButton.interactable = false;

            _refreshButton.onClick.AddListener(RefreshButtonAction);
            _reportButton.onClick.AddListener(ReportButtonAction);
            _resetAllButton.onClick.AddListener(ResetAchievements);
            _showViewControllerButton.onClick.AddListener(ShowViewControllerButtonAction);

            _showViewControllerButton.interactable = IsViewControllerAvailableForAchievements;
        }

        async void OnEnable()
        {
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        private void AddPlaceholderImage(Texture2D image, string title)
        {
            var button = Instantiate(_achievementPlaceholderImageButtonPrefab, _achievementsListContent.transform, worldPositionStays: false);
            button.Image = image;
            button.Title = title;
            button.ButtonClick += (sender, args) =>
            {
                _propertiesText.text = string.Empty;
            };
        }

        private string _lastTappedAchievementID = string.Empty;

        public async Task Refresh()
        {
            _refreshButton.interactable = false;
            _reportButton.interactable = false;
            _lastTappedAchievementID = string.Empty;

            try
            {
                Clear();

                var achievements = await GKAchievement.LoadAchievements();
                var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();

                foreach (var description in descriptions)
                {
                    var button = Instantiate(_achievementButtonPrefab, _achievementsListContent.transform, worldPositionStays: false);
                    button.Description = description;

                    var achievement = achievements.Where(a => a.Identifier == description.Identifier).FirstOrDefault();
                    button.Achievement = achievement;
                    button.ButtonClick += async (sender, args) =>
                    {
                        if (sender is AchievementButton button)
                        {
                            var description = button.Description;
                            var achievement = button.Achievement;

                            var builder = new StringBuilder();
                            builder.Append($"{nameof(GKAchievementDescription)}:\n");
                            builder.Append($"    {nameof(GKAchievementDescription.Identifier)} = \"{description.Identifier}\"\n");
                            builder.Append($"    {nameof(GKAchievementDescription.Title)} = \"{description.Title}\"\n");
                            builder.Append($"    {nameof(GKAchievementDescription.UnachievedDescription)} = \"{description.UnachievedDescription}\"\n");
                            builder.Append($"    {nameof(GKAchievementDescription.AchievedDescription)} = \"{description.AchievedDescription}\"\n");
                            builder.Append($"    {nameof(GKAchievementDescription.GroupIdentifier)} = \"{description.GroupIdentifier}\"\n");
                            builder.Append($"    {nameof(GKAchievementDescription.MaximumPoints)} = {description.MaximumPoints}\n");
                            builder.Append($"    {nameof(GKAchievementDescription.IsHidden)} = {description.IsHidden}\n");
                            builder.Append($"    {nameof(GKAchievementDescription.IsReplayable)} = {description.IsReplayable}\n");

                            if (IsRarityPropertyAvailable)
                            {
                                builder.Append($"    {nameof(GKAchievementDescription.RarityPercent)} = {description.RarityPercent}\n");
                            }

                            if (achievement != null)
                            {
                                builder.Append($"\n{nameof(GKAchievement)}:\n");
                                builder.Append($"    {nameof(GKAchievement.Identifier)} = \"{achievement.Identifier}\"\n");
                                builder.Append($"    {nameof(GKAchievement.Player)} = \"{achievement.Player?.DisplayName ?? string.Empty}\"\n");
                                builder.Append($"    {nameof(GKAchievement.PercentComplete)} = {achievement.PercentComplete}\n");
                                builder.Append($"    {nameof(GKAchievement.IsCompleted)} = {achievement.IsCompleted}\n");
                                builder.Append($"    {nameof(GKAchievement.LastReportedDate)} = {achievement.LastReportedDate:u}\n");
                                builder.Append($"    {nameof(GKAchievement.ShowCompletionBanner)} = {achievement.ShowCompletionBanner}\n");
                            }

                            _propertiesText.text = builder.ToString();

                            // Second tap on achievement opens the dashboard.
                            if (description.Identifier == _lastTappedAchievementID)
                            {
                                // Alternate using the view controller and access point APIs to show the dashboard.
                                if ((_useAccessPoint || !IsViewControllerAvailable) && IsAccessPointAvailable)
                                {
                                    await GKAccessPoint.Shared.TriggerWithAchievementID(description.Identifier);
                                }
                                else if ((!_useAccessPoint || !IsAccessPointAvailable) && IsViewControllerAvailable)
                                {
                                    var viewController = GKGameCenterViewController.InitWithAchievementID(description.Identifier);
                                    await viewController.Present();
                                }
                                _useAccessPoint = !_useAccessPoint;
                            }
                            _lastTappedAchievementID = description.Identifier;

                            _reportButton.interactable = achievement == null || achievement.PercentComplete < 100.0f;
                        }
                    };
                }

                // Add placeholder images to the end of the list.
                AddPlaceholderImage(GKAchievementDescription.IncompleteAchievementImage, nameof(GKAchievementDescription.IncompleteAchievementImage));
                AddPlaceholderImage(GKAchievementDescription.PlaceholderCompletedAchievementImage, nameof(GKAchievementDescription.PlaceholderCompletedAchievementImage));
            }
            finally
            {
                _refreshButton.interactable = true;
            }
        }

        public void Clear()
        {
            _propertiesText.text = string.Empty;

            foreach (Transform transform in _achievementsListContent.transform)
            {
                Destroy(transform.gameObject);
            }
            _achievementsListContent.transform.DetachChildren();
        }

        public async void ResetAchievements()
        {
            _resetAllButton.interactable = false;
            try
            {
                await GKAchievement.ResetAchievements();
                await Refresh();
            }
            finally
            {
                _resetAllButton.interactable = true;
            }
        }

        public async void ShowViewControllerButtonAction()
        {
            await ShowViewController();
        }

        public async Task ShowViewController()
        {
            if (IsViewControllerAvailableForAchievements)
            {
                var viewController = GKGameCenterViewController.InitWithState(GKGameCenterViewController.GKGameCenterViewControllerState.Achievements);
                await viewController.Present();
            }
        }

        public async void ReportButtonAction()
        {
            try
            {
                var achievement = _achievementsListContent
                    .GetComponentsInChildren<AchievementButton>()
                    .Where(button => button.Achievement?.Identifier == _lastTappedAchievementID)
                    .Select(button => button.Achievement)
                    .FirstOrDefault()
                    ?? GKAchievement.Init(_lastTappedAchievementID);

                achievement.PercentComplete = Math.Min(achievement.PercentComplete + 50.0f, 100.0f);

                await GKAchievement.Report(new GKAchievement[] { achievement });
                
                await Refresh();
            }
            catch (Exception ex)
            {
                _propertiesText.text = ex.Message;
            }
        }
    }
}
