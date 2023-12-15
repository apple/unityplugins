using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apple.Core.Runtime;
using Apple.GameKit.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

namespace Apple.GameKit.Sample
{
    using GKMatchProperties = NSDictionary<NSString, NSObject>;

    public class RealtimeMatchRequestPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Dropdown _minPlayersDropdown;
        [SerializeField] private Dropdown _maxPlayersDropdown;
        [SerializeField] private Dropdown _matchmakingModeDropdown;

        [SerializeField] private Toggle _fastStartToggle;
        [SerializeField] private Toggle _serverHostedToggle;

        [SerializeField] private InputField _queueNameInputField;

        [SerializeField] private GameObject _queueRelatedItems;
        [SerializeField] private InputField _propertiesInputField;
        [SerializeField] private InputField _recipientPropertiesInputField;
        [SerializeField] private Button _queryQueueActivityButton;
        [SerializeField] private Text   _queueQueueActivityButtonText;
        [SerializeField] private string _updateString;
        [SerializeField] private string _updatingString;

        [SerializeField] private RealtimeMatchStatusPanel _realtimeMatchStatusPanel;
        [SerializeField] private WaitPanel _waitPanel;
#pragma warning restore 0649

        private Color _propertiesTextDefaultColor = Color.black;
        private GKMatchProperties _propertiesDictionary = null;
        private NSDictionary<NSString, GKMatchProperties> _recipientPropertiesDictionary = null;

        void Start()
        {
            _propertiesTextDefaultColor = _propertiesInputField.textComponent.color;

            _minPlayersDropdown.options = _maxPlayersDropdown.options = new List<Dropdown.OptionData>
            {
                new Dropdown.OptionData("2 Players"),
                new Dropdown.OptionData("3 Players"),
                new Dropdown.OptionData("4 Players"),
                new Dropdown.OptionData("5 Players"),
                new Dropdown.OptionData("6 Players"),
                new Dropdown.OptionData("7 Players"),
                new Dropdown.OptionData("8 Players"),
                new Dropdown.OptionData("9 Players"),
                new Dropdown.OptionData("10 Players"),
                new Dropdown.OptionData("11 Players"),
                new Dropdown.OptionData("12 Players"),
                new Dropdown.OptionData("13 Players"),
                new Dropdown.OptionData("14 Players"),
                new Dropdown.OptionData("15 Players"),
                new Dropdown.OptionData("16 Players"),
            };

            LoadSettings();
        }

        static readonly string MinPlayersPrefsKey = "RealtimeMatchRequestPanel.MinPlayers";
        static readonly string MaxPlayersPrefsKey = "RealtimeMatchRequestPanel.MaxPlayers";
        static readonly string MatchmakingModePrefsKey = "RealtimeMatchRequestPanel.MatchmakingMode";
        static readonly string FastStartPrefsKey = "RealtimeMatchRequestPanel.FastStart";
        static readonly string ServerHostedPrefsKey = "RealtimeMatchRequestPanel.ServerHosted";
        static readonly string QueueNamePrefsKey = "RealtimeMatchRequestPanel.QueueName";
        static readonly string PropertiesPrefsKey = "RealtimeMatchRequestPanel.Properties";
        static readonly string RecipientPropertiesPrefsKey = "RealtimeMatchRequestPanel.RecipientProperties";

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(MinPlayersPrefsKey, _minPlayersDropdown.value);
            PlayerPrefs.SetInt(MaxPlayersPrefsKey, _maxPlayersDropdown.value);
            PlayerPrefs.SetInt(MatchmakingModePrefsKey, _matchmakingModeDropdown.value);
            PlayerPrefs.SetInt(FastStartPrefsKey, _fastStartToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(ServerHostedPrefsKey, _serverHostedToggle.isOn ? 1 : 0);
            PlayerPrefs.SetString(QueueNamePrefsKey, _queueNameInputField.text);
            PlayerPrefs.SetString(PropertiesPrefsKey, _propertiesInputField.text);
            PlayerPrefs.SetString(RecipientPropertiesPrefsKey, _recipientPropertiesInputField.text);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            _minPlayersDropdown.value = PlayerPrefs.GetInt(MinPlayersPrefsKey, _minPlayersDropdown.value);
            _maxPlayersDropdown.value = PlayerPrefs.GetInt(MaxPlayersPrefsKey, _maxPlayersDropdown.value);
            _matchmakingModeDropdown.value = PlayerPrefs.GetInt(MatchmakingModePrefsKey, _matchmakingModeDropdown.value);
            _fastStartToggle.isOn = PlayerPrefs.GetInt(FastStartPrefsKey, _fastStartToggle.isOn ? 1 : 0) != 0;
            _serverHostedToggle.isOn = PlayerPrefs.GetInt(ServerHostedPrefsKey, _serverHostedToggle.isOn ? 1 : 0) != 0;
            _queueNameInputField.text = PlayerPrefs.GetString(QueueNamePrefsKey, _queueNameInputField.text);
            _propertiesInputField.text = PlayerPrefs.GetString(PropertiesPrefsKey, _propertiesInputField.text);
            _recipientPropertiesInputField.text = PlayerPrefs.GetString(RecipientPropertiesPrefsKey, _recipientPropertiesInputField.text);

            // Make sure UI rules get a chance to run on the new values.
            OnMinPlayersChanged(_minPlayersDropdown.value);
            OnMaxPlayersChanged(_maxPlayersDropdown.value);
            OnQueueNameChanged(_queueNameInputField.text);
            OnPropertiesChanged(_propertiesInputField.text);
            OnRecipientPropertiesChanged(_recipientPropertiesInputField.text);
        }

        public void OnMinPlayersChanged(Int32 newValue)
        {
            var validValue = Math.Min(newValue, _maxPlayersDropdown.value);
            if (validValue != newValue)
            {
                _minPlayersDropdown.value = validValue;
            }
        }

        public void OnMaxPlayersChanged(Int32 newValue)
        {
            var validValue = Math.Max(newValue, _minPlayersDropdown.value);
            if (validValue != newValue)
            {
                _maxPlayersDropdown.value = validValue;
            }
        }

        public void OnQueueNameChanged(string queueName)
        {
            var hasValue = !string.IsNullOrWhiteSpace(queueName);
            if (hasValue != _queueRelatedItems.activeSelf)
            {
                _queueRelatedItems.SetActive(hasValue);
                _queueQueueActivityButtonText.text = _updateString;
            }
        }

        public async void OnQueryQueueActivity()
        {
            try
            {
                _queryQueueActivityButton.interactable = false;
                _queueQueueActivityButtonText.text = _updatingString;

                var requestsPerMinute = await GKMatchmaker.Shared.QueryQueueActivity(_queueNameInputField.text);
                _queueQueueActivityButtonText.text = $"{requestsPerMinute}";
            }
            catch (Exception ex)
            {
                if (ex is GameKitException gkex && gkex.Code == (long)GKErrorCode.Cancelled)
                {
                    // User canceled the request
                }
                else
                {
                    Debug.LogException(ex);
                }
                _queueQueueActivityButtonText.text = _updateString;
            }
            _queryQueueActivityButton.interactable = true;
        }

        public void OnPropertiesChanged(string propertiesJson)
        {
            if (!string.IsNullOrWhiteSpace(propertiesJson))
            {
                try
                {
                    _propertiesDictionary = GKMatchProperties.FromJson(propertiesJson);
                }
                catch (Exception)
                {
                    _propertiesInputField.textComponent.color = Color.red;
                    _propertiesDictionary = null;
                    return;
                }
            }
            else
            {
                _propertiesDictionary = null;
            }
            _propertiesInputField.textComponent.color = _propertiesTextDefaultColor;
        }

        public void OnRecipientPropertiesChanged(string recipientPropertiesJson)
        {
            if (!string.IsNullOrWhiteSpace(recipientPropertiesJson))
            {
                try
                {
                    _recipientPropertiesDictionary = NSDictionary<NSString, GKMatchProperties>.FromJson(recipientPropertiesJson);
                }
                catch (Exception)
                {
                    _recipientPropertiesInputField.textComponent.color = Color.red;
                    _recipientPropertiesDictionary = null;
                    return;
                }
            }
            else
            {
                _recipientPropertiesDictionary = null;
            }
            _recipientPropertiesInputField.textComponent.color = _propertiesTextDefaultColor;
        }

        private GKMatchRequest CreateMatchRequestFromUI(out GKMatchmakingMode mode, out bool isHosted, out bool canStartWithMinimumPlayers)
        {
            var request = GKMatchRequest.Init();

            // dropdowns start with 2 players == value 0
            const Int32 PlayersForDropdownValue0 = 2;
            request.MinPlayers = _minPlayersDropdown.value + PlayersForDropdownValue0;
            request.MaxPlayers = _maxPlayersDropdown.value + PlayersForDropdownValue0;

            if (!string.IsNullOrWhiteSpace(_queueNameInputField.text))
            {
                request.QueueName = _queueNameInputField.text;
                request.Properties = _propertiesDictionary;
            }

            mode = (GKMatchmakingMode)_matchmakingModeDropdown.value;
            isHosted = _serverHostedToggle.isOn;
            canStartWithMinimumPlayers = _fastStartToggle.isOn;

            SaveSettings();

            return request;
        }

        private Task<GKMatchProperties> GetMatchPropertiesForRecipientHandler(GKMatchmakerViewController matchmakerViewController, GKPlayer invitedPlayer)
        {
            // look up the player by name in the dictionary
            GKMatchProperties properties = null;
            _recipientPropertiesDictionary?.TryGetValue(invitedPlayer.DisplayName, out properties);
            return Task.FromResult(properties);
        }

        private void HostedPlayerDidAcceptHandler(GKMatchmakerViewController matchmakerViewController, GKPlayer acceptingPlayer)
        {
            // Automatically accept all players since we don't have an actual server to communicate with.
            matchmakerViewController.SetHostedPlayerDidConnect(acceptingPlayer, true);
        }

        public async void ShowGKMatchmakerViewController()
        {
            var request = CreateMatchRequestFromUI(out var mode, out var isHosted, out var canStartWithMinimumPlayers);

            // Wait for match to start...
            try
            {
                if (isHosted)
                {
                    var players = await GKMatchmakerViewController.RequestHosted(
                        request,
                        mode,
                        canStartWithMinimumPlayers,
                        (request.QueueName != default) ? GetMatchPropertiesForRecipientHandler : default,
                        HostedPlayerDidAcceptHandler);

                    _realtimeMatchStatusPanel.Populate(request, players);
                }
                else
                {
                    var match = await GKMatchmakerViewController.Request(
                        request, 
                        mode, 
                        canStartWithMinimumPlayers,
                        (request.QueueName != default) ? GetMatchPropertiesForRecipientHandler : default);

                    _realtimeMatchStatusPanel.Populate(request, match);
                }

                GameKitSample.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
            }
            catch (TaskCanceledException)
            {
                // User canceled the matchmaking attempt.
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void CallFindMatchedPlayers()
        {
            var request = CreateMatchRequestFromUI(out var mode, out var isHosted, out var canStartWithMinimumPlayers);

            _waitPanel.CancelAction = () =>
            {
                GKMatchmaker.Shared.Cancel();
            };

            _waitPanel.TaskToAwait = async () =>
            {
                try
                {
                    var matchedPlayers = await GKMatchmaker.Shared.FindMatchedPlayers(request);

                    // Put the matchmaker back into the default state.
                    GKMatchmaker.Shared.Cancel();

                    _realtimeMatchStatusPanel.Populate(request, matchedPlayers);
                    GameKitSample.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
                }
                catch (Exception ex)
                {
                    if (ex is GameKitException gkex && gkex.Code == (long)GKErrorCode.Cancelled)
                    {
                        // User canceled the request
                    }
                    else
                    {
                        Debug.LogException(ex);
                    }

                    GameKitSample.PopPanel();
                }
            };

            // Wait for the list of matched players.
            GameKitSample.PushPanel(_waitPanel.gameObject);
        }

        private GKMatchRequest MatchRequest { get; set; }
        private GKMatchedPlayers MatchedPlayers { get; set; }

        private async void OnEnable()
        {
            // Did we arrive here because the status panel invite button was pressed?
            var matchRequest = _realtimeMatchStatusPanel.MatchRequest;
            var matchedPlayers = _realtimeMatchStatusPanel.MatchedPlayers;
            if (_realtimeMatchStatusPanel.IsRequestingMatch &&
                matchRequest != null &&
                matchedPlayers != null)
            {
                _realtimeMatchStatusPanel.Clear();
                await InviteMatchedPlayers(matchRequest, matchedPlayers);
            }
        }

        public async Task InviteMatchedPlayers(GKMatchRequest matchRequest, GKMatchedPlayers matchedPlayers)
        {
            // Add recipient properties to the match request.
            matchRequest.Recipients = matchedPlayers.Players;
            var recipientProperties = new NSMutableDictionary<GKPlayer, GKMatchProperties>();
            foreach (var recipient in matchRequest.Recipients)
            {
                if (_recipientPropertiesDictionary.TryGetValue(recipient.DisplayName, out var matchProperties))
                {
                    recipientProperties.Add(recipient, matchProperties);
                }
            }
            matchRequest.RecipientProperties = recipientProperties;

            try
            {
                var match = await GKMatchmakerViewController.Request(
                    matchRequest, 
                    (GKMatchmakingMode)_matchmakingModeDropdown.value, 
                    _fastStartToggle.isOn,
                    (matchRequest.QueueName != default) ? GetMatchPropertiesForRecipientHandler : default);

                _realtimeMatchStatusPanel.Populate(matchRequest, match);

                GameKitSample.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
            }
            catch (TaskCanceledException)
            {
                // User canceled the matchmaking attempt.
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
       }
    }
}