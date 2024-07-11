using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementPanel : MonoBehaviour
    {
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

        [SerializeField] private RawImage _image = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _identifierText = default;
        [SerializeField] private Text _completionText = default;
        [SerializeField] private Text _lastReportedDateText = default;

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

                _image.texture = texture;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
