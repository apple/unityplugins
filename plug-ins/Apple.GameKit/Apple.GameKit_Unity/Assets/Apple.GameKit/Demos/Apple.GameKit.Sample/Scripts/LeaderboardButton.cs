using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.GameKit.Leaderboards;

namespace Apple.GameKit.Sample
{
    public class LeaderboardButton : ListItemButtonBase<LeaderboardButton>
    {
        private GKLeaderboard _leaderboard;
        public GKLeaderboard Leaderboard
        {
            get => _leaderboard;
            set
            {
                _leaderboard = value;
                UpdateDisplay();
#if !UNITY_TVOS
                _ = UpdateImage();
#endif                
            }
        }

#pragma warning disable CS0414 // prevent unused variable warnings on tvOS
        [SerializeField] private RawImage _image = default;
#pragma warning restore CS0414

        [SerializeField] private Text _baseLeaderboardIdText = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _leaderboardTypeText = default;
        [SerializeField] private Text _groupIdentifierText = default;
        [SerializeField] private Text _startDateText = default;
        [SerializeField] private Text _nextStartDateText = default;
        [SerializeField] private Text _durationText = default;

        public LeaderboardButton Instantiate(GameObject parent, GKLeaderboard leaderboard)
        {
            var button = base.Instantiate(parent);

            button.Leaderboard = leaderboard;

            return button;
        }

#if !UNITY_TVOS
        private async Task UpdateImage()
        {
            try
            {
                var texture = (Leaderboard != null) ? await Leaderboard.LoadImage() : null;
                _image.texture = (texture != null) ? texture : Texture2D.whiteTexture;
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }
#endif // !UNITY_TVOS

        private void UpdateDisplay()
        {
            _baseLeaderboardIdText.text = Leaderboard?.BaseLeaderboardId ?? string.Empty;
            _titleText.text = Leaderboard?.Title ?? string.Empty;
            _leaderboardTypeText.text = Leaderboard?.Type.ToString() ?? string.Empty;
            _groupIdentifierText.text = Leaderboard?.GroupIdentifier ?? string.Empty;
            _startDateText.text = Leaderboard?.StartDate.ToString() ?? string.Empty;
            _nextStartDateText.text = Leaderboard?.NextStartDate.ToString() ?? string.Empty;
            _durationText.text = Leaderboard?.Duration.ToString() ?? string.Empty;
        }
    }
}
