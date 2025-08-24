using Godot;
using System;


namespace Monolithic.Networking
{
    public partial class PanelNetworkingUI : Control
    {
        [Export] public LineEdit EditAddress { get; private set; }
        [Export] public LineEdit EditPort { get; private set; }
        [Export] public Button ButtonConnect { get; private set; }
        [Export] public Button ButtonHost { get; private set; }

        public override void _Ready()
        {



            GetNode<Button>("ButtonClose").Pressed += () => QueueFree();

        }
    }
}
