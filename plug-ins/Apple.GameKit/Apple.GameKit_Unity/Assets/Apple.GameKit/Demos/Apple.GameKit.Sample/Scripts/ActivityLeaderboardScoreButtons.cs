using System;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivityLeaderboardScoreButtons : MonoBehaviour
    {
        public GKGameActivity Activity { get; private set; }
        public GKLeaderboard Leaderboard { get; private set; }


        [SerializeField] private Text _scoreButtonText = default;
        [SerializeField] private Button _scoreButton = default;
        [SerializeField] private Button _removeButton = default;

        public ActivityLeaderboardScoreButtons Instantiate(GameObject parent, GKGameActivity activity, GKLeaderboard leaderboard)
        {
            var buttons = Instantiate(this.gameObject, parent.transform, worldPositionStays: false).GetComponent<ActivityLeaderboardScoreButtons>();

            buttons.Activity = activity;
            buttons.Leaderboard = leaderboard;

            return buttons;
        }

        string _scoreButtonFormat = "Set Score: {0}";

        protected virtual void Start()
        {
            _scoreButtonFormat = _scoreButtonText.text;

            UpdateButtons();

            _scoreButton.onClick.AddListener(OnScoreButton);
            _removeButton.onClick.AddListener(OnRemoveButton);
        }

        private void UpdateButtons()
        {
            bool enableRemoveButton = false;

            var activity = Activity;
            var leaderboard = Leaderboard;
            if (activity != null && leaderboard != null)
            {
                var score = activity.GetScoreOnLeaderboard(leaderboard);
                var scoreValue = score?.Value ?? 0;
                _scoreButtonText.text = string.Format(_scoreButtonFormat, scoreValue);

                if (scoreValue > 0)
                {
                    enableRemoveButton = true;
                }
            }

            _removeButton.interactable = enableRemoveButton;
        }

        public event EventHandler ScoreChanged;

        void OnScoreButton()
        {
            var activity = Activity;
            var leaderboard = Leaderboard;
            if (activity != null && leaderboard != null)
            {
                var score = activity.GetScoreOnLeaderboard(leaderboard);
                var scoreValue = (score?.Value ?? 0) + 20;
                activity.SetScoreOnLeaderboard(leaderboard, scoreValue);

                // also test the version of SetScore that takes a context value
                activity.SetScoreOnLeaderboard(leaderboard, scoreValue, context:99);

                ScoreChanged?.Invoke(this, EventArgs.Empty);
                UpdateButtons();
            }
        }

        void OnRemoveButton()
        {
            var activity = Activity;
            var leaderboard = Leaderboard;
            if (activity != null && leaderboard != null)
            {
                activity.RemoveScoresFromLeaderboards(leaderboard);
                ScoreChanged?.Invoke(this, EventArgs.Empty);
                UpdateButtons();
            }
        }
    }
}
