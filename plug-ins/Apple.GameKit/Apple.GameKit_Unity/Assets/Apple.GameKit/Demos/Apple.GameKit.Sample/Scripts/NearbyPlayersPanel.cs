using System;
using System.Collections.Generic;
using Apple.Core;
using Apple.GameKit.Multiplayer;
using UnityEngine;

namespace Apple.GameKit.Sample
{
    public class NearbyPlayersPanel : MonoBehaviour
    {
        [SerializeField] private PlayerButton _playerButtonPrefab = default;
        [SerializeField] private GameObject _playersContent = default;

        private Dictionary<GKPlayer, PlayerButton> _players = new Dictionary<GKPlayer, PlayerButton>();

        private readonly bool IsViewControllerAvailableForPlayer = Availability.IsMethodAvailable<GKGameCenterViewController>(nameof(GKGameCenterViewController.InitWithPlayer));

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

        void AddButtonForPlayer(GKPlayer player)
        {
            var button = Instantiate(_playerButtonPrefab, _playersContent.transform, worldPositionStays: false);
            button.Player = player;

            if (IsViewControllerAvailableForPlayer)
            {
                button.ButtonClick += async (sender, args) =>
                {
                    var viewController = GKGameCenterViewController.InitWithPlayer(player);
                    await viewController.Present();
                };
            }

            _players.Add(player, button);
        }

        void RemovePlayerButton(PlayerButton button)
        {
            var player = button.Player;
            _players.Remove(player);
            button.transform.SetParent(null);
            button.Player = null;
            Destroy(button);
        }

        void NearbyPlayerReachableHandler(GKPlayer player, bool isReachable)
        {
            if (_players.TryGetValue(player, out var button))
            {
                if (!isReachable)
                {
                    // remove existing player from the list
                    RemovePlayerButton(button);
                }
            }
            else
            {
                if (isReachable)
                {
                    // add new player to the list
                    AddButtonForPlayer(player);
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
