using System;
using System.Collections.Generic;
using Apple.GameKit.Multiplayer;
using UnityEngine;

namespace Apple.GameKit.Sample
{
    public class NearbyPlayersPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPanelPrefab = default;
        [SerializeField] private GameObject _playersContent = default;

        Dictionary<GKPlayer, PlayerPanel> _players = new Dictionary<GKPlayer, PlayerPanel>();

        void OnEnable()
        {
            var matchmaker = GKMatchmaker.Shared;
            if (matchmaker != null)
            {
                matchmaker.NearbyPlayerReachable += NearbyPlayerReachableHandler;
                matchmaker.StartBrowsingForNearbyPlayers();
            }
        }

        void OnDisable()
        {
            var matchmaker = GKMatchmaker.Shared;
            if (matchmaker != null)
            {
                matchmaker.StopBrowsingForNearbyPlayers();
                matchmaker.NearbyPlayerReachable -= NearbyPlayerReachableHandler;
            }

            Clear();
        }

        void AddPanelForPlayer(GKPlayer player)
        {
            var panelObject = Instantiate(_playerPanelPrefab, _playersContent.transform, worldPositionStays: false);
            var panel = panelObject.GetComponent<PlayerPanel>();
            panel.Player = player;
            _players.Add(player, panel);
        }

        void RemovePlayerPanel(PlayerPanel panel)
        {
            var player = panel.Player;
            _players.Remove(player);
            panel.transform.SetParent(null);
            panel.Player = null;
            Destroy(panel);
        }

        void NearbyPlayerReachableHandler(GKPlayer player, bool isReachable)
        {
            if (_players.TryGetValue(player, out var panel))
            {
                if (!isReachable)
                {
                    // remove existing player from the list
                    RemovePlayerPanel(panel);
                }
            }
            else
            {
                if (isReachable)
                {
                    // add new player to the list
                    AddPanelForPlayer(player);
                }
            }
        }

        void Clear()
        {
            foreach (Transform transform in _playersContent.transform)
            {
                Destroy(transform.gameObject);
            }
            _playersContent.transform.DetachChildren();

            _players.Clear();
        }
    }
}
