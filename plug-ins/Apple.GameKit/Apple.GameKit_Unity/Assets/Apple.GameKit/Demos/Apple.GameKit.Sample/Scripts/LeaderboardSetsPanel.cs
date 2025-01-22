using System;
using System.Threading.Tasks;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class LeaderboardSetsPanel : PanelBase<LeaderboardSetsPanel>
    {
        [SerializeField] private LeaderboardSetButton _leaderboardSetButtonPrefab = default;
        [SerializeField] private GameObject _listContent = default;
        [SerializeField] private Button _refreshButton = default;
        [SerializeField] private ErrorButton _errorMessagePrefab = default;
        [SerializeField] private LeaderboardSetPanel _leaderboardSetPanelPrefab = default;

        void Start()
        {
            _refreshButton.onClick.AddListener(RefreshButtonAction);
            ShouldDestroyWhenPopped = IsPrefabInstance;
        }

        async void OnEnable()
        {
            await Refresh();
        }

        public async void RefreshButtonAction()
        {
            await Refresh();
        }

        public async Task Refresh()
        {
            _refreshButton.interactable = false;

            try
            {
                Clear();

                var leaderboardSets = await GKLeaderboardSet.LoadLeaderboardSets();
                if (leaderboardSets != null && leaderboardSets.Count > 0)
                {
                    foreach (var leaderboardSet in leaderboardSets)
                    {
                        var button = _leaderboardSetButtonPrefab.Instantiate(_listContent);
                        button.LeaderboardSet = leaderboardSet;
                        button.ButtonClick += (sender, args) =>
                        {
                            var leaderboardSetPanel = _leaderboardSetPanelPrefab.Instantiate(GameKitSample.Instance.PanelArea);
                            leaderboardSetPanel.LeaderboardSet = leaderboardSet;
                            GameKitSample.Instance.PushPanel(leaderboardSetPanel.gameObject);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                GKErrorCodeExtensions.LogException(ex);

                // show the exception text
                var errorButton = _errorMessagePrefab.Instantiate(_listContent);
                errorButton.Text = $"{ex.Message}";
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
    }
}
