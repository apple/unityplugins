using UnityEngine;

namespace Apple.Accessibility.Demo
{
    public class ModalController : MonoBehaviour
    {
        [SerializeField] RectTransform m_Panel;

        private void Start()
        {
            m_Panel.gameObject.SetActive(false);
        }

        public void ShowHideModalView(bool show)
        {
            m_Panel.gameObject.SetActive(show);
        }
    }
}
