
using Godot;
using System;

namespace Monolithic.Networking
{
    public partial class Player : RigidBody3D
    {
        [Export] public float Speed = 5.0f;
        [Export] public float JumpForce = 10.0f;
        [Export] public float MouseSensitivity = 0.1f;
        [Export] public Node3D Base { get; private set; }
        [Export] public Node3D CameraGimbal { get; private set; }
        [Export] public MultiplayerSynchronizer Sync { get; private set; }


        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            CameraGimbal = GetNode<Node3D>("Base/CameraGimbal");
            Sync = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");

            Sync.PublicVisibility = false;         
            Sync.AddVisibilityFilter(new Callable(this, nameof(AddPlayerDistanceVisibilityFilter)));
            Sync.VisibilityUpdateMode = MultiplayerSynchronizer.VisibilityUpdateModeEnum.Physics;
     

            if (!IsMultiplayerAuthority()) return;

            var camera = GetTree().Root.GetCamera3D() as Camera;
            camera?.SetTarget(CameraGimbal);


        }

        public bool AddPlayerDistanceVisibilityFilter(int id)
        {
            foreach (var player in GetTree().GetNodesInGroup("Players"))
            {
                if (player is Player p && p.GetMultiplayerAuthority() == id)
                {
                    //  GD.Print($"Set visibility filter for player {id}");

                    double distance = p.GlobalPosition.DistanceTo(GlobalPosition);
                    if (distance < 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void _PhysicsProcess(double delta)
        {
            Move();
        }

        private bool Move()
        {
            if (!IsMultiplayerAuthority()) return false;


            Vector3 direction = new();

            if (Input.IsActionPressed("forward")) direction -= Base.Transform.Basis.Z;
            if (Input.IsActionPressed("backward")) direction += Base.Transform.Basis.Z;
            if (Input.IsActionPressed("left")) direction -= Base.Transform.Basis.X;
            if (Input.IsActionPressed("right")) direction += Base.Transform.Basis.X;

            direction = direction.Normalized();
            ApplyCentralForce(direction * Speed);
            return true;
        }


        public override void _Input(InputEvent @event)
        {
            if (!IsMultiplayerAuthority()) return;

            if (@event is InputEventMouseMotion mouseMotion)
            {
                Base?.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
                CameraGimbal.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * MouseSensitivity));
                CameraGimbal.Rotation = new Vector3(
                    Mathf.Clamp(CameraGimbal.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
                    CameraGimbal.Rotation.Y,
                    CameraGimbal.Rotation.Z
                );
            }

            if (Input.IsActionJustPressed("jump"))
            {
                ApplyCentralImpulse(Vector3.Up * JumpForce);
            }
        }
    }
}

