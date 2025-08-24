using Godot;
using System;

namespace Monolithic.Networking
{
    /// <summary>
    /// Camera script placeholder.
    /// </summary>
    public partial class Camera : Camera3D
    {
        [Export] private Node3D _target;

        public override void _Process(double delta)
        {
            GlobalTransform = _target?.GlobalTransform ?? GlobalTransform;
        }
        
        public void SetTarget(Node3D target)
        {
            _target = target;
        }
    }
}
