using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class FriendPanel : MonoBehaviour
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

        private async Task UpdatePlayerImage()
        {
            try
            {
                _playerPhotoImage.texture = (Player != null) ? await Player.LoadPhoto(GKPlayer.PhotoSize.Normal) : null;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
