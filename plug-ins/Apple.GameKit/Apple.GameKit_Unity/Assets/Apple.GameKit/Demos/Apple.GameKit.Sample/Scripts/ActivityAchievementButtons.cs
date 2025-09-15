using System;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivityAchievementButtons : MonoBehaviour
    {
        public GKGameActivity Activity { get; private set; }
        public GKAchievement Achievement { get; private set; }

        [SerializeField] private Text _progressButtonText = default;
        [SerializeField] private Button _progressButton = default;
        [SerializeField] private Button _completeButton = default;
        [SerializeField] private Button _removeButton = default;

        public ActivityAchievementButtons Instantiate(GameObject parent, GKGameActivity activity, GKAchievement achievement)
        {
            var buttons = Instantiate(this.gameObject, parent.transform, worldPositionStays: false).GetComponent<ActivityAchievementButtons>();

            buttons.Activity = activity;
            buttons.Achievement = achievement;

            return buttons;
        }

        string _progressButtonFormat = "Progress: {0:F0}%";

        protected virtual void Start()
        {
            _progressButtonFormat = _progressButtonText.text;

            UpdateButtons();

            _progressButton.onClick.AddListener(OnProgressButton);
            _completeButton.onClick.AddListener(OnCompleteButton);
            _removeButton.onClick.AddListener(OnRemoveButton);
        }

        private void UpdateButtons()
        {
            bool isComplete = false;
            bool hasProgress = false;

            var activity = Activity;
            var achievement = Achievement;
            if (activity != null && achievement != null)
            {
                var progressPercent = activity.GetProgressOnAchievement(achievement);
                _progressButtonText.text = string.Format(_progressButtonFormat, progressPercent);

                isComplete = progressPercent >= 100.0;
                hasProgress = progressPercent > 0.0;
            }

            _progressButton.interactable = !isComplete;
            _completeButton.interactable = !isComplete;
            _removeButton.interactable = hasProgress;
        }

        public event EventHandler ProgressChanged;

        void OnProgressButton()
        {
            var activity = Activity;
            var achievement = Achievement;
            if (activity != null && achievement != null)
            {
                var progressPercent = Math.Min(activity.GetProgressOnAchievement(achievement) + 20.0, 100.0);
                activity.SetProgressOnAchievement(achievement, progressPercent);
                if (progressPercent >= 100.0)
                {
                    activity.SetAchievementCompleted(achievement);
                }
                ProgressChanged?.Invoke(this, EventArgs.Empty);
                UpdateButtons();
            }
        }

        void OnCompleteButton()
        {
            var activity = Activity;
            var achievement = Achievement;
            if (activity != null && achievement != null)
            {
                activity.SetAchievementCompleted(achievement);
                ProgressChanged?.Invoke(this, EventArgs.Empty);
                UpdateButtons();
            }
        }

        void OnRemoveButton()
        {
            var activity = Activity;
            var achievement = Achievement;
            if (activity != null && achievement != null)
            {
                activity.RemoveAchievements(achievement);
                ProgressChanged?.Invoke(this, EventArgs.Empty);
                UpdateButtons();
            }
        }
    }
}
