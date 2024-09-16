using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ErrorButton : ListItemButtonBase<ErrorButton>
    {
        [SerializeField] private Text ErrorMessageText = default;

        public string Text
        {
            get => ErrorMessageText.text;
            set => ErrorMessageText.text = value;
        }
    }
}
