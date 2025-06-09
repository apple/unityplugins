using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.GameKit.Leaderboards;

namespace Apple.GameKit.Sample
{
    public class LeaderboardSetButton : ListItemButtonBase<LeaderboardSetButton>
    {
        private GKLeaderboardSet _leaderboardSet;
        public GKLeaderboardSet LeaderboardSet
        {
            get => _leaderboardSet;
            set
            {
                _leaderboardSet = value;
                UpdateDisplay();
#if !UNITY_TVOS
                _ = UpdateImage();
#endif
            }
        }

#pragma warning disable CS0414 // prevent unused variable warnings on tvOS
        [SerializeField] private RawImage _image = default;
#pragma warning restore CS0414

        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _identifierText = default;
        [SerializeField] private Text _groupIdentifierText = default;

        public LeaderboardSetButton Instantiate(GameObject parent, GKLeaderboardSet leaderboardSet)
        {
            var button = base.Instantiate(parent);

            button.LeaderboardSet = leaderboardSet;

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
                var texture = (LeaderboardSet != null) ? await LeaderboardSet.LoadImage() : null;
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
            _titleText.text = LeaderboardSet?.Title ?? string.Empty;
            _identifierText.text = LeaderboardSet?.Identifier ?? string.Empty;
            _groupIdentifierText.text = LeaderboardSet?.GroupIdentifier ?? string.Empty;
        }
    }
}
