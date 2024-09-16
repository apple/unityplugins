using System;
using Apple.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AccessPointPanel : MonoBehaviour
    {
        [SerializeField] Text _errorMessage = default;
        [SerializeField] CanvasGroup _buttonGroup = default;
        [SerializeField] Dropdown _stateDropdown = default;

        private readonly bool IsAccessPointAvailable = Availability.IsTypeAvailable<GKAccessPoint>();

        void Start()
        {
            _buttonGroup.interactable = IsAccessPointAvailable;
            _errorMessage.gameObject.SetActive(!IsAccessPointAvailable);
        }

        public void OnToggleAccessPoint()
        {
            if (IsAccessPointAvailable)
            {
                GKAccessPoint.Shared.IsActive = !GKAccessPoint.Shared.IsActive;
            }
        }

        public async void OnTriggerAccessPoint()
        {
            if (IsAccessPointAvailable)
            {
                await GKAccessPoint.Shared.Trigger();
            }
        }

        public async void OnTriggerAccessPointWithState()
        {
            if (IsAccessPointAvailable &&
                Enum.TryParse<GKGameCenterViewController.GKGameCenterViewControllerState>(
                    _stateDropdown.options[_stateDropdown.value].text,
                    out var state))
            {
                await GKAccessPoint.Shared.Trigger(state);
            }
        }
    }
}
