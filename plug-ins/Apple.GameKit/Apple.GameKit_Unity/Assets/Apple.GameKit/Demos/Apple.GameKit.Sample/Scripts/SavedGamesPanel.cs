using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Apple.Core.Runtime;
using System.Linq;

namespace Apple.GameKit.Sample
{
    public class SavedGamesPanel : PanelBase<SavedGamesPanel>
    {
#pragma warning disable CS0414 // prevent unused variable warnings on tvOS
        [SerializeField] private SavedGameButton _savedGameButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;

        [SerializeField] private GameObject _defaultGroup = default;
        [SerializeField] private Button _saveButton = default;
        [SerializeField] private Button _refreshButton = default;

        [SerializeField] private GameObject _deleteGroup = default;

        [SerializeField] private GameObject _saveGroup = default;
        [SerializeField] private InputField _saveNameInputField = default;
        [SerializeField] private InputField _saveDataInputField = default;

        [SerializeField] private GameObject _confirmationGroup = default;
        [SerializeField] private Button _okButton = default;
        [SerializeField] private Button _cancelButton = default;

        [SerializeField] private ErrorButton _errorMessagePrefab = default;

        [SerializeField] private MessageLog _messageLog = default;

        private GameObject[] _groups = default;
#pragma warning restore CS0414

#if !UNITY_TVOS
        void Start()
        {
            ShouldDestroyWhenPopped = IsPrefabInstance;

            _saveButton.onClick.AddListener(BeginSave);
            _refreshButton.onClick.AddListener(RefreshButtonAction);

            _groups = new GameObject[]
            {
                _defaultGroup,
                _deleteGroup,
                _saveGroup,
                _confirmationGroup
            };

            EnableGroups(_defaultGroup);

            _saveNameInputField.text = "Test Save";
            _saveDataInputField.text = "test save data";
            LoadSettings();

            _messageLog.LinesToKeep = 50;
        }

        void EnableGroups(params GameObject[] groups)
        {
            foreach (var group in _groups)
            {
                group.SetActive(false);
            }
            foreach (var group in groups)
            {
                group.SetActive(true);
            }
        }

        void RestoreDefaultState()
        {
            _okButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _okButton.interactable = true;

            EnableGroups(_defaultGroup);
        }

        async void OnEnable()
        {
            GKSavedGame.SavedGameModified += OnSavedGameModified;
            GKSavedGame.SavedGamesConflicting += OnSavedGamesConflicting;

            await Refresh();
        }

        void OnDisable()
        {
            GKSavedGame.SavedGameModified -= OnSavedGameModified;
            GKSavedGame.SavedGamesConflicting -= OnSavedGamesConflicting;
        }

        static readonly string SaveNamePrefsKey = "SavedGamesPanel.SaveName";
        static readonly string SaveDataPrefsKey = "SavedGamesPanel.SaveData";

        private void SaveSettings()
        {
            PlayerPrefs.SetString(SaveNamePrefsKey, _saveNameInputField.text);
            PlayerPrefs.SetString(SaveDataPrefsKey, _saveDataInputField.text);
        }

        private void LoadSettings()
        {
            _saveNameInputField.text = PlayerPrefs.GetString(SaveNamePrefsKey, _saveNameInputField.text);
            _saveDataInputField.text = PlayerPrefs.GetString(SaveDataPrefsKey, _saveDataInputField.text);
        }

        public async void RefreshButtonAction()
        {
            _messageLog.ClearLog();
            await Refresh();
        }

        private void AddSavedGameToList(GKSavedGame savedGame)
        {
            var button = _savedGameButtonPrefab.Instantiate(_listContent, savedGame);
            button.ButtonClick += (sender, args) =>
            {
                // ask to delete
                BeginDelete(button);
            };
        }

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                var savedGames = await GKLocalPlayer.Local.FetchSavedGames();

                Clear();

                if (savedGames != null && savedGames.Count > 0)
                {
                    foreach (var savedGame in savedGames)
                    {
                        AddSavedGameToList(savedGame);
                    }
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);

                // show the exception text
                var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                errorButton.Text = $"{ex.Message}";

                // Add a helpful message about setting up iCloud.
                var explanation = (ex as GameKitException)?.GKErrorCode.GetExplanatoryText();
                if (!string.IsNullOrWhiteSpace(explanation))
                {
                    var helpButton = _errorMessagePrefab.Instantiate(_listContent);
                    helpButton.Text = explanation;
                }
            }
            finally
            {
                _refreshButton.interactable = true;
            }
        }

        public void Clear()
        {
            DestroyChildren(_listContent);
        }

        public void BeginSave()
        {
            EnableGroups(_saveGroup, _confirmationGroup);

            _okButton.onClick.AddListener(async () =>
            {
                await EndSave(isConfirmed: true);
            });

            _cancelButton.onClick.AddListener(async () =>
            {
                await EndSave(isConfirmed: false);
            });

            _okButton.interactable = !string.IsNullOrWhiteSpace(_saveNameInputField.text);

            _saveNameInputField.onValueChanged.AddListener(value =>
            {
                _okButton.interactable = !string.IsNullOrWhiteSpace(value);
            });
        }

        private async Task EndSave(bool isConfirmed)
        {
            RestoreDefaultState();

            if (isConfirmed)
            {
                SaveSettings();
                try
                {
                    var name = _saveNameInputField.text;
                    var data = new NSString(_saveDataInputField.text);
                    _messageLog.AppendMessageToLog($"{DateTime.Now}: Saving: {name}");
                    var savedGame = await GKLocalPlayer.Local.SaveGameData(data.Utf8Data, name);
                    await Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        } 

        private void BeginDelete(SavedGameButton savedGameButton)
        {
            // highlight the seleted item
            savedGameButton.SetColor(Color.red);

            EnableGroups(_deleteGroup, _confirmationGroup);

            _okButton.onClick.AddListener(async () =>
            {
                await EndDelete(savedGameButton, isConfirmed: true);
            });

            _cancelButton.onClick.AddListener(async () =>
            {
                await EndDelete(savedGameButton, isConfirmed: false);
            });
        }

        private async Task EndDelete(SavedGameButton savedGameButton, bool isConfirmed)
        {
            // remove the highlight
            savedGameButton.RestoreColor();

            RestoreDefaultState();

            if (isConfirmed)
            {
                var name = savedGameButton.SavedGame.Name;
                _messageLog.AppendMessageToLog($"{DateTime.Now}: Deleting: {name}");
                await GKLocalPlayer.Local.DeleteSavedGames(name);
                await Refresh();
            }
        }

        private async void OnSavedGameModified(GKPlayer player, GKSavedGame savedGame)
        {
            _messageLog.AppendMessageToLog($"{DateTime.Now}: Save modified: {savedGame.Name} for player: {player.DisplayName} ({player.GamePlayerId})");
            await Refresh();
        }

        private async void OnSavedGamesConflicting(GKPlayer player, NSArray<GKSavedGame> savedGames)
        {
            // savedGames may contain multiple names so group by name.
            var groups = savedGames.GroupBy(sg => sg.Name.ToString());

            foreach (var group in groups)
            {
                // preserve the most recent save
                var mostRecentSave = group.OrderBy(sg => sg.ModificationDate).LastOrDefault();
                if (mostRecentSave != null)
                {
                    var data = await mostRecentSave.LoadData();
                    var nsArray = new NSMutableArray<GKSavedGame>(group.ToArray());
                    _messageLog.AppendMessageToLog($"{DateTime.Now}: Resolving conflicts for {nsArray.Count} copies of {group.Key} with data: {{{data.ToString(maxLength: 64)}}} for player: {player.DisplayName} ({player.GamePlayerId})");
                    var results = await GKLocalPlayer.Local.ResolveConflictingSavedGames(nsArray, data);
                    await Refresh();
                }
            }
        }
#endif // !UNITY_TVOS
    }
}
