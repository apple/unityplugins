using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class AccessPointPanel : MonoBehaviour
    {
        [SerializeField] Dropdown _stateDropdown = default;

        public void OnToggleAccessPoint()
        {
            GKAccessPoint.Shared.IsActive = !GKAccessPoint.Shared.IsActive;
        }

        public void OnTriggerAccessPoint()
        {
            GKAccessPoint.Shared.Trigger();
        }

        public void OnTriggerAccessPointWithState()
        {
            if (Enum.TryParse<GKGameCenterViewController.GKGameCenterViewControllerState>(
                _stateDropdown.options[_stateDropdown.value].text,
                out var state))
            {
                GKAccessPoint.Shared.Trigger(state);
            }
        }
    }
}
