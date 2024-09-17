using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementButton : MonoBehaviour
    {
        [SerializeField] private RawImage _image = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _identifierText = default;
        [SerializeField] private Text _completionText = default;
        [SerializeField] private Text _lastReportedDateText = default;
        [SerializeField] private Button _button = default;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                ButtonClick?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler ButtonClick;

        private GKAchievementDescription _achievementDescription;
        public GKAchievementDescription Description
        {
            get => _achievementDescription;
            set
            {
                _achievementDescription = value;
                UpdateDescriptionProperties();
                _ = UpdateDescriptionImage();
            }
        }

        private GKAchievement _achievement;
        public GKAchievement Achievement
        {
            get => _achievement;
            set
            {
                _achievement = value;
                UpdateAchievementProperties();
            }
        }

        private void UpdateDescriptionProperties()
        {
            _titleText.text = Description?.Title ?? string.Empty;
            _identifierText.text = Description?.Identifier ?? string.Empty;
        }

        private async Task UpdateDescriptionImage()
        {
            try
            {
                var texture = (Description != null) ? await Description.LoadImage() : null;
                _image.texture = (texture != null) ? texture : Texture2D.whiteTexture;
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private void UpdateAchievementProperties()
        {
            if (Achievement != null)
            {
                _completionText.text = Achievement.IsCompleted ? "Completed" : $"{Achievement.PercentComplete:F0}%";
                _lastReportedDateText.text = Achievement.LastReportedDate.ToString("u");
            }
            else
            {
                _completionText.text = string.Empty;
                _lastReportedDateText.text = string.Empty;
            }
        }
    }
}
