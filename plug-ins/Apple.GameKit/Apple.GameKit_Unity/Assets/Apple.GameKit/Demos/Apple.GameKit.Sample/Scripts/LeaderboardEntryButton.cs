using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.GameKit.Leaderboards;

namespace Apple.GameKit.Sample
{
    public class LeaderboardEntryButton : ListItemButtonBase<LeaderboardEntryButton>
    {
        private GKLeaderboard.Entry _leaderboardEntry;
        public GKLeaderboard.Entry LeaderboardEntry
        {
            get => _leaderboardEntry;
            set
            {
                _leaderboardEntry = value;
                UpdateDisplay();
                _ = UpdateImage();
            }
        }

        [SerializeField] private RawImage _image = default;

        [SerializeField] private Text _contextText = default;
        [SerializeField] private Text _dateText = default;
        [SerializeField] private Text _formattedScoreText = default;
        [SerializeField] private Text _playerNameText = default;
        [SerializeField] private Text _rankText = default;
        [SerializeField] private Text _scoreText = default;

        public LeaderboardEntryButton Instantiate(GameObject parent, GKLeaderboard.Entry leaderboardEntry)
        {
            var button = base.Instantiate(parent);

            button.LeaderboardEntry = leaderboardEntry;

            return button;
        }

        void OnDisable()
        {
            _image.DestroyTexture();
        }

        public GKPlayer Player => LeaderboardEntry?.Player;

        private async Task UpdateImage()
        {
            try
            {
                var texture = (LeaderboardEntry?.Player != null) ? await LeaderboardEntry.Player.LoadPhoto(GKPlayer.PhotoSize.Normal) : null;
                _image.DestroyTextureAndAssign(texture);
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private void UpdateDisplay()
        {
            _contextText.text = LeaderboardEntry?.Context.ToString() ?? string.Empty;
            _dateText.text = LeaderboardEntry?.Date.ToString() ?? string.Empty;
            _formattedScoreText.text = LeaderboardEntry?.FormattedScore ?? string.Empty;
            _playerNameText.text = LeaderboardEntry?.Player?.DisplayName ?? string.Empty;
            _rankText.text = LeaderboardEntry?.Rank.ToString() ?? string.Empty;
            _scoreText.text = LeaderboardEntry?.Score.ToString() ?? string.Empty;
        }
    }
}
