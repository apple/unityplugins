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

        [SerializeField] Text _iOS19ErrorMessage = default;
        [SerializeField] Button _triggerForChallengesButton = default;
        [SerializeField] Button _triggerForPlayTogetherButton = default;
        [SerializeField] Button _triggerForFriendingButton = default;

        private readonly bool IsAccessPointAvailable = Availability.IsTypeAvailable<GKAccessPoint>();

        private readonly bool IsTriggerForChallengesAvailable 
#if UNITY_IOS || UNITY_STANDALONE_OSX
            = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerForChallenges));
#else
            = false;
#endif
        private readonly bool IsTriggerForPlayTogetherAvailable 
#if UNITY_IOS || UNITY_STANDALONE_OSX
            = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerForPlayTogether));
#else
            = false;
#endif
        private readonly bool IsTriggerForFriendingAvailable 
#if UNITY_IOS || UNITY_STANDALONE_OSX
            = Availability.IsMethodAvailable<GKAccessPoint>(nameof(GKAccessPoint.TriggerForFriending));
#else
            = false;
#endif


        void Start()
        {
            _buttonGroup.interactable = IsAccessPointAvailable;
            _errorMessage.gameObject.SetActive(!IsAccessPointAvailable);

            _iOS19ErrorMessage.gameObject.SetActive(!IsTriggerForChallengesAvailable || !IsTriggerForPlayTogetherAvailable || !IsTriggerForFriendingAvailable);
            _triggerForChallengesButton.interactable = IsTriggerForChallengesAvailable;
            _triggerForPlayTogetherButton.interactable = IsTriggerForPlayTogetherAvailable;
            _triggerForFriendingButton.interactable = IsTriggerForFriendingAvailable;
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
                Enum.TryParse<GKGameCenterViewControllerState>(
                    _stateDropdown.options[_stateDropdown.value].text,
                    out var state))
            {
                await GKAccessPoint.Shared.Trigger(state);
            }
        }

        public async void OnTriggerForChallenges()
        {
#if UNITY_IOS || UNITY_STANDALONE_OSX
            if (IsTriggerForChallengesAvailable)
            {
                await GKAccessPoint.Shared.TriggerForChallenges();
            }
#endif
        }

        public async void OnTriggerForPlayTogether()
        {
#if UNITY_IOS || UNITY_STANDALONE_OSX
            if (IsTriggerForPlayTogetherAvailable)
            {
                await GKAccessPoint.Shared.TriggerForPlayTogether();
            }
#endif
        }

        public async void OnTriggerForFriending()
        {
#if UNITY_IOS || UNITY_STANDALONE_OSX
            if (IsTriggerForFriendingAvailable)
            {
                await GKAccessPoint.Shared.TriggerForFriending();
            }
#endif
        }

    }
}
