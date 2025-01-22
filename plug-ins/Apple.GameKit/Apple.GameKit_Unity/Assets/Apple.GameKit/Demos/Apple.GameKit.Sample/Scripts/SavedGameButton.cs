using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.Core.Runtime;

namespace Apple.GameKit.Sample
{
    public class SavedGameButton : ListItemButtonBase<SavedGameButton>
    {
#pragma warning disable CS0414 // prevent unused variable warnings on tvOS
        [SerializeField] private Text _nameText = default;
        [SerializeField] private Text _modificationDateText = default;
        [SerializeField] private Text _deviceNameText = default;
        [SerializeField] private Text _dataText = default;

        [SerializeField] private Image _image = default;
        private Color _originalColor = default;
#pragma warning restore CS0414

#if !UNITY_TVOS
        protected override void Start()
        {
            _originalColor = _image.color;
            base.Start();
        }

        private GKSavedGame _savedGame;
        public GKSavedGame SavedGame
        {
            get => _savedGame;
            set
            {
                _savedGame = value;
                UpdateDisplay();
                _ = LoadData();
            }
        }

        public SavedGameButton Instantiate(GameObject parent, GKSavedGame savedGame)
        {
            var button = base.Instantiate(parent);

            button.SavedGame = savedGame;

            return button;
        }

        private async Task LoadData()
        {
            NSData nsData = null;
            if (SavedGame != null)
            {
                try
                {
                    nsData = await SavedGame.LoadData();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            if (nsData != null && nsData.Length > 0)
            {
                _dataText.text = nsData.ToString(maxLength: 64);
            }
            else
            {
                _dataText.text = "(no data)";
            }
        }

        private void UpdateDisplay()
        {
            _nameText.text = SavedGame?.Name ?? string.Empty;
            _deviceNameText.text = SavedGame?.DeviceName ?? string.Empty;
            _modificationDateText.text = SavedGame?.ModificationDate.ToString("u");
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void RestoreColor()
        {
            _image.color = _originalColor;
        }
#endif // !UNITY_TVOS
    }
}
