using System;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementsPanel : PanelBase<AchievementsPanel>
    {
        [SerializeField] private AchievementPanel _achievementPanelPrefab = default;
        [SerializeField] private AchievementButton _achievementButtonPrefab = default;
        [SerializeField] private AchievementPlaceholderImageButton _achievementPlaceholderImageButtonPrefab = default;
        [SerializeField] private GameObject _achievementsListContent = default;
        [SerializeField] private Button _showViewControllerButton = default;
        [SerializeField] private Button _resetAllButton = default;
        [SerializeField] private Button _refreshButton = default;

        private readonly bool IsViewControllerAvailableForAchievements = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithState));

        void Start()
        {
            _refreshButton.onClick.AddListener(RefreshButtonAction);
            _resetAllButton.onClick.AddListener(ResetAchievements);
            _showViewControllerButton.onClick.AddListener(ShowViewControllerButtonAction);

            _showViewControllerButton.gameObject.SetActive(IsViewControllerAvailableForAchievements);
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
        }

        private bool Interactable
        {
            get => _refreshButton.interactable;
            set
            {
                _refreshButton.interactable = value;
                _resetAllButton.interactable = value;
                _showViewControllerButton.interactable = value;
            }
        }

        public async Task Refresh()
        {
            Interactable = false;

            try
            {
                Clear();

                var descriptions = await GKAchievementDescription.LoadAchievementDescriptions();
                var achievements = await GKAchievement.LoadAchievements();

                foreach (var description in descriptions)
                {
                    var achievement = achievements.Where(a => a.Identifier == description.Identifier).FirstOrDefault();
                    var button = _achievementButtonPrefab.Instantiate(_achievementsListContent, description, achievement);
                    button.ButtonClick += async (sender, args) =>
                    {
                        var achievementPanel = _achievementPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea, description, achievement);
                        GameKitSample.Instance.PushPanel(achievementPanel.gameObject);
                    };
                }

                // Add placeholder images to the end of the list.
                AddPlaceholderImage(GKAchievementDescription.IncompleteAchievementImage, nameof(GKAchievementDescription.IncompleteAchievementImage));
                AddPlaceholderImage(GKAchievementDescription.PlaceholderCompletedAchievementImage, nameof(GKAchievementDescription.PlaceholderCompletedAchievementImage));
            }
            finally
            {
                Interactable = true;
            }
        }

        public void Clear()
        {
            DestroyChildren(_achievementsListContent);
        }

        public async void ResetAchievements()
        {
            Interactable = false;
            try
            {
                await GKAchievement.ResetAchievements();
                await Refresh();
            }
            finally
            {
                Interactable = true;
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
                var viewController = GKGameCenterViewController.InitWithState(GKGameCenterViewControllerState.Achievements);
                await viewController.Present();
            }
        }
    }
}
