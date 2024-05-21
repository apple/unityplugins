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
        [SerializeField] private GameObject _achievementPanelPrefab = default;
        [SerializeField] private GameObject _achievementPlaceholderImagePanelPrefab = default;
        [SerializeField] private GameObject _achievementsListContent = default;
        [SerializeField] private Text _propertiesText = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private Button _resetAllButton = default;

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
            var panelObject = Instantiate(_achievementPlaceholderImagePanelPrefab, _achievementsListContent.transform, worldPositionStays: false);

            var panel = panelObject.GetComponent<AchievementPlaceholderImagePanel>();
            panel.Image = image;
            panel.Title = title;

            var panelButton = panelObject.GetComponent<Button>();
            panelButton.onClick.AddListener(() =>
            {
                _propertiesText.text = string.Empty;
            });
        }

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                var achievements = await GKAchievement.LoadAchievements();
                var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();

                var isRarityPropertyAvailable = 
                    Availability.Available(RuntimeOperatingSystem.macOS, 14, 0) ||
                    Availability.Available(RuntimeOperatingSystem.iOS, 17, 2) ||
                    Availability.Available(RuntimeOperatingSystem.tvOS, 17, 2);

                foreach (var description in descriptions)
                {
                    var panelObject = Instantiate(_achievementPanelPrefab, _achievementsListContent.transform, worldPositionStays: false);

                    var panel = panelObject.GetComponent<AchievementPanel>();
                    panel.Description = description;

                    var achievement = achievements.Where(a => a.Identifier == description.Identifier).FirstOrDefault();
                    panel.Achievement = achievement;

                    var panelButton = panelObject.GetComponent<Button>();
                    panelButton.onClick.AddListener(() =>
                    {
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

                        if (isRarityPropertyAvailable)
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
                    });
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
            var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Achievements);
            await gameCenter.Present();
        }
    }
}
