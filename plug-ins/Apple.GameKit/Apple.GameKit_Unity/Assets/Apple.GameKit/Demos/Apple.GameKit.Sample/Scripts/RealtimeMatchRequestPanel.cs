using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apple.Core;
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
        [SerializeField] private Dropdown _minPlayersDropdown = default;
        [SerializeField] private Dropdown _maxPlayersDropdown = default;
        [SerializeField] private Dropdown _matchmakingModeDropdown = default;

        [SerializeField] private Toggle _fastStartToggle = default;
        [SerializeField] private Toggle _serverHostedToggle = default;

        [SerializeField] private InputField _queueNameInputField = default;

        [SerializeField] private GameObject _queueRelatedItems = default;
        [SerializeField] private InputField _propertiesInputField = default;
        [SerializeField] private InputField _recipientPropertiesInputField = default;
        [SerializeField] private Button _queryQueueActivityButton = default;
        [SerializeField] private Text   _queueQueueActivityButtonText = default;
        [SerializeField] private string _updateString = default;
        [SerializeField] private string _updatingString = default;

        [SerializeField] private Button _callFindMatchedPlayersButton = default;
        [SerializeField] private CanvasGroup _rbmmCanvasGroup = default;

        [SerializeField] private RealtimeMatchStatusPanel _realtimeMatchStatusPanel = default;
        [SerializeField] private WaitPanel _waitPanel = default;

        private Color _propertiesTextDefaultColor = Color.black;
        private GKMatchProperties _propertiesDictionary = null;
        private NSDictionary<NSString, GKMatchProperties> _recipientPropertiesDictionary = null;

        private readonly bool IsFindMatchedPlayersAvailable = Availability.IsMethodAvailable<GKMatchmaker>(nameof(GKMatchmaker.FindMatchedPlayers));
        private readonly bool IsRuleBasedMatchmakingAvailable = Availability.IsPropertyAvailable<GKMatchRequest>(nameof(GKMatchRequest.QueueName));
        private readonly bool IsMatchmakingModeAvailable = Availability.IsTypeAvailable<GKMatchmakingMode>();

        private static List<Dropdown.OptionData> CreatePlayerCountDropdownContent(GKMatchRequest.GKMatchType matchType)
        {
            int maxPlayers = (int)GKMatchRequest.MaxPlayersAllowedForMatch(matchType);
            var list = new List<Dropdown.OptionData>(maxPlayers - 1);
            for (int i = 2; i <= maxPlayers; i++)
            {
                list.Add(new Dropdown.OptionData($"{i} Players"));
            }
            return list;
        }

        private List<Dropdown.OptionData> _peerToPeerDropDownOptionData = null;
        private List<Dropdown.OptionData> _hostedDropDownOptionData = null;

        void Start()
        {
            _rbmmCanvasGroup.interactable = IsRuleBasedMatchmakingAvailable;
            _callFindMatchedPlayersButton.interactable = IsFindMatchedPlayersAvailable;

            _propertiesTextDefaultColor = _propertiesInputField.textComponent.color;

            _peerToPeerDropDownOptionData = CreatePlayerCountDropdownContent(GKMatchRequest.GKMatchType.PeerToPeer);
            _hostedDropDownOptionData = CreatePlayerCountDropdownContent(GKMatchRequest.GKMatchType.Hosted);

            _matchmakingModeDropdown.interactable = IsMatchmakingModeAvailable;
            if (IsMatchmakingModeAvailable)
            {
                _matchmakingModeDropdown.options = Enum.GetNames(typeof(GKMatchmakingMode))
                    .Where(name => Availability.IsFieldAvailable<GKMatchmakingMode>(name))
                    .Select(name => new Dropdown.OptionData(name))
                    .ToList();
            }
            else
            {
                _matchmakingModeDropdown.options.Clear();
            }

            if (!IsRuleBasedMatchmakingAvailable)
            {
                _queueRelatedItems.SetActive(false);
            }

            LoadSettings();
            OnServerHostedChanged(_serverHostedToggle.isOn);
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
            PlayerPrefs.SetInt(FastStartPrefsKey, _fastStartToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(ServerHostedPrefsKey, _serverHostedToggle.isOn ? 1 : 0);

            if (IsMatchmakingModeAvailable)
            {
                PlayerPrefs.SetInt(MatchmakingModePrefsKey, _matchmakingModeDropdown.value);
            }

            if (IsRuleBasedMatchmakingAvailable)
            {
                PlayerPrefs.SetString(QueueNamePrefsKey, _queueNameInputField.text);
                PlayerPrefs.SetString(PropertiesPrefsKey, _propertiesInputField.text);
                PlayerPrefs.SetString(RecipientPropertiesPrefsKey, _recipientPropertiesInputField.text);
            }
            
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            _minPlayersDropdown.value = PlayerPrefs.GetInt(MinPlayersPrefsKey, _minPlayersDropdown.value);
            _maxPlayersDropdown.value = PlayerPrefs.GetInt(MaxPlayersPrefsKey, _maxPlayersDropdown.value);
            _fastStartToggle.isOn = PlayerPrefs.GetInt(FastStartPrefsKey, _fastStartToggle.isOn ? 1 : 0) != 0;
            _serverHostedToggle.isOn = PlayerPrefs.GetInt(ServerHostedPrefsKey, _serverHostedToggle.isOn ? 1 : 0) != 0;

            if (IsMatchmakingModeAvailable)
            {
                _matchmakingModeDropdown.value = PlayerPrefs.GetInt(MatchmakingModePrefsKey, _matchmakingModeDropdown.value);
            }

            if (IsRuleBasedMatchmakingAvailable)
            {
                _queueNameInputField.text = PlayerPrefs.GetString(QueueNamePrefsKey, _queueNameInputField.text);
                _propertiesInputField.text = PlayerPrefs.GetString(PropertiesPrefsKey, _propertiesInputField.text);
                _recipientPropertiesInputField.text = PlayerPrefs.GetString(RecipientPropertiesPrefsKey, _recipientPropertiesInputField.text);
            }

            // Make sure UI rules get a chance to run on the new values.
            OnMinPlayersChanged(_minPlayersDropdown.value);
            OnMaxPlayersChanged(_maxPlayersDropdown.value);

            if (IsRuleBasedMatchmakingAvailable)
            {
                OnQueueNameChanged(_queueNameInputField.text);
                OnPropertiesChanged(_propertiesInputField.text);
                OnRecipientPropertiesChanged(_recipientPropertiesInputField.text);
            }
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

        public void OnServerHostedChanged(bool isHosted)
        {
            _minPlayersDropdown.options = _maxPlayersDropdown.options = isHosted ? _hostedDropDownOptionData : _peerToPeerDropDownOptionData;
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
                if (ex is GameKitException gkex && gkex.IsGKErrorDomain && gkex.GKErrorCode == GKErrorCode.Cancelled)
                {
                    // User canceled the request
                }
                else
                {
                    GKErrorCodeExtensions.LogException(ex);
                }
                _queueQueueActivityButtonText.text = _updateString;
            }
            finally
            {
                _queryQueueActivityButton.interactable = true;
            }
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

            if (IsRuleBasedMatchmakingAvailable && !string.IsNullOrWhiteSpace(_queueNameInputField.text))
            {
                request.QueueName = _queueNameInputField.text;
                request.Properties = _propertiesDictionary;
            }

            if (!IsMatchmakingModeAvailable || !Enum.TryParse<GKMatchmakingMode>(_matchmakingModeDropdown.options[_matchmakingModeDropdown.value].text, out mode))
            {
                mode = GKMatchmakingMode.Default;
            }

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
                        (IsRuleBasedMatchmakingAvailable && request.QueueName != default) ? GetMatchPropertiesForRecipientHandler : default,
                        HostedPlayerDidAcceptHandler);

                    _realtimeMatchStatusPanel.Populate(request, players);
                }
                else
                {
                    var match = await GKMatchmakerViewController.Request(
                        request, 
                        mode, 
                        canStartWithMinimumPlayers,
                        (IsRuleBasedMatchmakingAvailable && request.QueueName != default) ? GetMatchPropertiesForRecipientHandler : default);

                    _realtimeMatchStatusPanel.Populate(request, match, showStartGameButton: false);
                }

                GameKitSample.Instance.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
            }
            catch (TaskCanceledException)
            {
                // User canceled the matchmaking attempt.
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
        }

        public void CallFindMatchedPlayers()
        {
            Debug.Assert(IsFindMatchedPlayersAvailable);
            
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
                    GameKitSample.Instance.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
                }
                catch (Exception ex)
                {
                    if (ex is GameKitException gkex && gkex.IsGKErrorDomain && gkex.GKErrorCode == GKErrorCode.Cancelled)
                    {
                        // User canceled the request
                    }
                    else
                    {
                        GKErrorCodeExtensions.LogException(ex);
                    }

                    GameKitSample.Instance.PopPanel();
                }
            };

            // Wait for the list of matched players.
            GameKitSample.Instance.PushPanel(_waitPanel.gameObject);
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
            matchRequest.Recipients = matchedPlayers.Players;

            // Add optional recipient properties to the match request.
            if (IsRuleBasedMatchmakingAvailable && _recipientPropertiesDictionary?.Count > 0)
            {
                var recipientProperties = new NSMutableDictionary<GKPlayer, GKMatchProperties>();
                foreach (var recipient in matchRequest.Recipients)
                {
                    if (_recipientPropertiesDictionary.TryGetValue(recipient.DisplayName, out var matchProperties))
                    {
                        recipientProperties.Add(recipient, matchProperties);
                    }
                }
                matchRequest.RecipientProperties = recipientProperties;
            }

            matchRequest.InviteMessage = "Please join my match!";

            matchRequest.RecipientResponse += (player, response) =>
            {
                Debug.Log($"GKMatchRequest RecipientResponse: player={player.DisplayName} response={response}");
            };

            try
            {
                // programmatic matchmaking
                var match = await GKMatchmaker.Shared.FindMatch(matchRequest);

                _realtimeMatchStatusPanel.Populate(matchRequest, match, showStartGameButton: true);

                GameKitSample.Instance.ReplaceActivePanel(_realtimeMatchStatusPanel.gameObject);
            }
            catch (TaskCanceledException)
            {
                // User canceled the matchmaking attempt.
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);
            }
       }
    }
}