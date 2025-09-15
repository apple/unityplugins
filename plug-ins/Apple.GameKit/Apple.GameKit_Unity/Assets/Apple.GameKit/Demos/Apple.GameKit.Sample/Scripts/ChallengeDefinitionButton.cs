using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ChallengeDefinitionButton : ListItemButtonBase<ChallengeDefinitionButton>
    {
        private GKChallengeDefinition _challengeDefinition;
        public GKChallengeDefinition ChallengeDefinition
        {
            get => _challengeDefinition;
            set
            {
                _challengeDefinition = value;
                _ = UpdateDisplay();
                _ = UpdateImage();
            }
        }

        [SerializeField] private RawImage _image = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _identifierText = default;
        [SerializeField] private Text _groupIdText = default;
        [SerializeField] private Text _leaderboardTitleText = default;
        [SerializeField] private Text _activeText = default;
        [SerializeField] private Text _releaseStateText = default;

        public ChallengeDefinitionButton Instantiate(GameObject parent, GKChallengeDefinition challengeDefinition)
        {
            var button = base.Instantiate(parent);

            button.ChallengeDefinition = challengeDefinition;

            return button;
        }

        void OnDisable()
        {
            _image.DestroyTexture();
        }

        private async Task UpdateImage()
        {
            try
            {
                var texture = (ChallengeDefinition != null) ? await ChallengeDefinition.LoadImage() : null;
                _image.DestroyTextureAndAssign(texture);
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private async Task UpdateDisplay()
        {
            _titleText.text = ChallengeDefinition?.Title ?? string.Empty;
            _identifierText.text = ChallengeDefinition?.Identifier ?? string.Empty;
            _groupIdText.text = ChallengeDefinition?.GroupIdentifier ?? string.Empty;
            _leaderboardTitleText.text = ChallengeDefinition?.Leaderboard?.Title ?? string.Empty;
            _releaseStateText.text = ChallengeDefinition?.ReleaseState.ToString() ?? string.Empty;

            bool hasActiveChallenges = false;
            if (ChallengeDefinition != null)
            {
                hasActiveChallenges = await ChallengeDefinition.HasActiveChallenges();
            }
            _activeText.text = hasActiveChallenges ? "yes" : "no";
        }
    }
}
