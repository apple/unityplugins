using UnityEngine;
using UnityEngine.UI;

public class FriendErrorPanel : MonoBehaviour
{
    [SerializeField] private Text ErrorMessageText = default;

    public string Text
    {
        get => ErrorMessageText.text;
        set => ErrorMessageText.text = value;
    }
}
