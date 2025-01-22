using System;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class ListItemButtonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Button _button = default;

        public virtual T Instantiate(GameObject parent)
        {
            return Instantiate(this.gameObject, parent.transform, worldPositionStays: false).GetComponent<T>();
        }

        public virtual void Destroy()
        {
            Destroy(this.gameObject);
        }

        protected virtual void Start()
        {
            _button.onClick.AddListener(() => ButtonClick?.Invoke(this, EventArgs.Empty));
        }

        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public event EventHandler ButtonClick;
    }
}
