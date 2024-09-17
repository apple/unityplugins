using System;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AchievementPlaceholderImageButton : MonoBehaviour
    {
        [SerializeField] private RawImage _image = default;
        [SerializeField] private Text _titleText = default;
        [SerializeField] private Button _button = default;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                ButtonClick?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler ButtonClick;

        public Texture2D Image
        {
            get => _image.texture as Texture2D;
            set => _image.texture = value;
        }

        public string Title
        {
            get => _titleText.text;
            set => _titleText.text = value;
        }
    }
}
