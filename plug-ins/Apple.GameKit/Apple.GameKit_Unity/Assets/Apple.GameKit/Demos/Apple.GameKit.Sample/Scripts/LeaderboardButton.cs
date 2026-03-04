using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.GameKit.Leaderboards;
using Apple.Core;

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

        [SerializeField] private Text _baseIdText = default;
        [SerializeField] private Text _groupIdText = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _leaderboardTypeText = default;
        [SerializeField] private Text _releaseStateText = default;

        public LeaderboardButton Instantiate(GameObject parent, GKLeaderboard leaderboard)
        {
            var button = base.Instantiate(parent);

            button.Leaderboard = leaderboard;

            return button;
        }

        void OnDisable()
        {
            _image.DestroyTexture();
        }

#if !UNITY_TVOS
        private async Task UpdateImage()
        {
            try
            {
                var texture = (Leaderboard != null) ? await Leaderboard.LoadImage() : null;
                _image.DestroyTextureAndAssign(texture);
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }
#endif // !UNITY_TVOS

        private void UpdateDisplay()
        {
            Func<string, Func<string>, string> formatPropertyValue = (propertyName, getValue) =>
                Availability.IsPropertyAvailable<GKLeaderboard>(propertyName) ? getValue() : "Not available";

            _baseIdText.text = formatPropertyValue(nameof(GKLeaderboard.BaseLeaderboardId), () => Leaderboard?.BaseLeaderboardId ?? string.Empty);
            _groupIdText.text = formatPropertyValue(nameof(GKLeaderboard.GroupIdentifier), () => Leaderboard?.GroupIdentifier ?? string.Empty);
            _titleText.text = formatPropertyValue(nameof(GKLeaderboard.Title), () => Leaderboard?.Title ?? string.Empty);
            _leaderboardTypeText.text = formatPropertyValue(nameof(GKLeaderboard.Type), () => Leaderboard?.Type.ToString() ?? string.Empty);
            _releaseStateText.text = formatPropertyValue(nameof(GKLeaderboard.ReleaseState), () => Leaderboard?.ReleaseState.ToString() ?? string.Empty);
        }
    }
}
