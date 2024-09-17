using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class PlayerButton : ListItemButtonBase<PlayerButton>
    {
        private GKPlayer _player;
        public GKPlayer Player
        {
            get => _player;
            set
            {
                _player = value;
                UpdatePlayerDisplay();
                _ = UpdatePlayerImage();
            }
        }

        [SerializeField] private RawImage _playerPhotoImage = default;

        [SerializeField] private Text _displayNameText = default;
        [SerializeField] private Text _aliasText = default;
        [SerializeField] private Text _gamePlayerIdText = default;
        [SerializeField] private Text _teamPlayerIdText = default;
        [SerializeField] private Text _isInvitableText = default;

        public PlayerButton Instantiate(GameObject parent, GKPlayer player)
        {
            var button = base.Instantiate(parent);

            button.Player = player;

            return button;
        }

        private async Task UpdatePlayerImage()
        {
            try
            {
                var texture = (Player != null) ? await Player.LoadPhoto(GKPlayer.PhotoSize.Normal) : null;
                _playerPhotoImage.texture = (texture != null) ? texture : Texture2D.whiteTexture;
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        private void UpdatePlayerDisplay()
        {
            _displayNameText.text = Player?.DisplayName ?? string.Empty;
            _aliasText.text = Player?.Alias ?? string.Empty;
            _gamePlayerIdText.text = Player?.GamePlayerId ?? string.Empty;
            _teamPlayerIdText.text = Player?.TeamPlayerId ?? string.Empty;
            _isInvitableText.text = (Player?.IsInvitable ?? false) ? "Yes" : "No";
        }
    }
}
