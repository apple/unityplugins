using System;
using System.Threading.Tasks;
using Apple.Core.Runtime;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    using GKMatchProperties = NSDictionary<NSString, NSObject>;
    using GKPlayerConnectionState = GKMatchDelegate.GKPlayerConnectionState;

    public class MatchPlayerButton : MonoBehaviour
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

        private GKMatchProperties _matchProperties;
        public GKMatchProperties MatchProperties
        {
            get => _matchProperties;
            set
            {
                _matchProperties = value;
                UpdateMatchPropertiesDisplay();
            }
        }

        // The use of [NonSerialized] below is to prevent Unity from complaining
        // about unsupported enum types since GKPlayerConnectionState is 64-bits
        // and Unity only supports 32-bit enums.
        // https://forum.unity.com/threads/unsupported-enum-type-for-long-ulong-or-int64.1142242
        [NonSerialized] private GKPlayerConnectionState _connectionState;
        public GKPlayerConnectionState ConnectionState
        {
            get => _connectionState;
            set
            {
                _connectionState = value;
                UpdateConnectionStateDisplay();
            }
        }

        [SerializeField] private RawImage _playerPhotoImage = default;
        [SerializeField] private Text _localPlayerText = default;

        [SerializeField] private Text _displayNameText = default;
        [SerializeField] private Text _aliasText = default;
        [SerializeField] private Text _gamePlayerIdText = default;
        [SerializeField] private Text _teamPlayerIdText = default;

        [SerializeField] private Text _matchPropertiesText = default;

        [SerializeField] private Text _connectionStateText = default;

        [SerializeField] private Button _button = default;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                ButtonClick?.Invoke(this, EventArgs.Empty);
            });
        }

        void OnDisable()
        {
            _playerPhotoImage.DestroyTexture();
        }

        public event EventHandler ButtonClick;

        private async Task UpdatePlayerImage()
        {
            try
            {
                var texture = (Player != null) ? await Player.LoadPhoto(GKPlayer.PhotoSize.Normal) : null;
                _playerPhotoImage.DestroyTextureAndAssign(texture);
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

            _localPlayerText.gameObject.SetActive(Equals(Player, GKLocalPlayer.Local));
        }

        private void UpdateMatchPropertiesDisplay()
        {
            _matchPropertiesText.text = MatchProperties?.ToJson() ?? string.Empty;
        }

        private void UpdateConnectionStateDisplay()
        {
            _connectionStateText.text = ConnectionState.ToString();
            switch (ConnectionState)
            {
                case GKPlayerConnectionState.Connected:
                    _connectionStateText.color = Color.green;
                    break;
                case GKPlayerConnectionState.Disconnected:
                    _connectionStateText.color = Color.red;
                    break;
                case GKPlayerConnectionState.Unknown:
                default:
                    _connectionStateText.color = Color.yellow;
                    break;
            }
        }
    }
}