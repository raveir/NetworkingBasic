using Godot;
using System;

namespace Monolithic.Networking
{
    public partial class PlayerSpawner : MultiplayerSpawner
    {
        [Export] public PackedScene PlayerScene { get; set; }

        public override void _Ready()
        {

            if (PlayerScene == null)
            {
                GD.PrintErr("Player scene is not assigned in the PlayerSpawner.");
                return;
            }

            SpawnFunction = new Callable(this, nameof(SpawnPlayer));
        }

        public Node SpawnPlayer(long id)
        {
            var player = (Node)PlayerScene.Instantiate();
            player.Name = id.ToString();
            player.SetMultiplayerAuthority((int) id);
            return player;
        }
    }
}