using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ActivityDefinitionButton : ListItemButtonBase<ActivityDefinitionButton>
    {
        private GKGameActivityDefinition _activityDefinition;
        public GKGameActivityDefinition ActivityDefinition
        {
            get => _activityDefinition;
            set
            {
                _activityDefinition = value;
                UpdateDisplay();
                _ = UpdateImage();
            }
        }

        [SerializeField] private RawImage _image = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Text _identifierText = default;
        [SerializeField] private Text _groupIdText = default;
        [SerializeField] private Text _releaseStateText = default;

        public ActivityDefinitionButton Instantiate(GameObject parent, GKGameActivityDefinition activityDefinition)
        {
            var button = base.Instantiate(parent);

            button.ActivityDefinition = activityDefinition;

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
                var texture = (ActivityDefinition != null) ? await ActivityDefinition.LoadImage() : null;
                _image.DestroyTextureAndAssign(texture);
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private void UpdateDisplay()
        {
            _titleText.text = ActivityDefinition?.Title ?? string.Empty;
            _identifierText.text = ActivityDefinition?.Identifier ?? string.Empty;
            _groupIdText.text = ActivityDefinition?.GroupIdentifier ?? string.Empty;
            _releaseStateText.text = ActivityDefinition?.ReleaseState.ToString() ?? string.Empty;
        }
    }
}
