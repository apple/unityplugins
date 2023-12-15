using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class WaitPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Text _messageText;
        [SerializeField] private Button _cancelButton;
#pragma warning restore 0649

        public string Message
        {
            get => _messageText.text;
            set => _messageText.text = value;
        }

        public Action CancelAction
        {
            get; set;
        }

        public void OnCancel()
        {
            CancelAction?.Invoke();
        }

        public Func<Task> TaskToAwait
        {
            get; set;
        }

        async void OnEnable()
        {
            _cancelButton.gameObject.SetActive(CancelAction != null);

            if (TaskToAwait != null)
            {
                await TaskToAwait.Invoke();
            }
        }

        void OnDisable()
        {
            // restore default state
            CancelAction = null;
            TaskToAwait = null;
        }
    }
}