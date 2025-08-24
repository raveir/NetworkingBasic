using Godot;
using System;
using System.Collections.Specialized;

namespace Monolithic.Networking
{
    public partial class Server : Node
    {
        [Export] private PanelNetworkingUI _panelNetworkingUI;
        [Export] private PlayerSpawner _playerSpawner;
        private const int DefaultPort = 7777;

        public override void _Ready()
        {
            if (_playerSpawner == null)
            {
                GD.PrintErr("PlayerSpawner is not assigned in the Server.");
                return;
            }

            Multiplayer.ConnectionFailed += () => GD.Print("Connection to server failed.");
            Multiplayer.ServerDisconnected += () => GD.Print("Disconnected from server.");

            _panelNetworkingUI.ButtonHost.Pressed += Host;
            _panelNetworkingUI.ButtonConnect.Pressed += Connect;

        }

        private void Host()
        {
            var peer = new ENetMultiplayerPeer();
            peer.CreateServer(DefaultPort);
            Multiplayer.MultiplayerPeer = peer;

            GD.Print($"Server started on port {DefaultPort}");
            _panelNetworkingUI.Hide();
            _playerSpawner.Spawn(1);

            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;
        }


        private void Connect()
        {
            GD.Print($"Connecting to Server");

            var peer = new ENetMultiplayerPeer();
            peer.CreateClient(_panelNetworkingUI.EditAddress.Text, DefaultPort);
            Multiplayer.MultiplayerPeer = peer;

            GD.Print($"Connecting to server");
            _panelNetworkingUI.QueueFree();
        }

        private void OnPeerDisconnected(long id)
        {
            GD.Print($"Peer connected with ID: {id}");
        }


        private void OnPeerConnected(long id)
        {
            GD.Print($"Peer connected with ID: {id}");
            var player = _playerSpawner.Spawn(id);
            player.SetMultiplayerAuthority((int)id);
        }

    }
}

