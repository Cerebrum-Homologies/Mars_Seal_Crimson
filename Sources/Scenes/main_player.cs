using Godot;
using System;

namespace Mars_Seal_Crimson
{
	public partial class main_player : CharacterBody2D
	{
		public float Speed { get; set; } = 200.0f;
		public const float JumpVelocity = -400.0f;
		private NavigationAgent2D navigationAgent2D;

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		public async void ActorSetup() {
			//await GetTree().PhysicsFrame();
			await ToSignal(GetTree(), "timeout");
			//navigationAgent2D.TargetLocation = movement_target;
		}

		public void MovePlayerToPosition(Vector2 movementTarget)
		{
			navigationAgent2D.TargetPosition = movementTarget;
		}

		public override void _Ready()
		{
			GD.Print("Ready on MainPlayer");
			navigationAgent2D = this.GetNodeOrNull<Godot.NavigationAgent2D>("NavigationAgent2D");
			Diagnostics.PrintNullValueMessage(navigationAgent2D, "navigationAgent2D");
			if (navigationAgent2D != null)
			{
				navigationAgent2D.PathDesiredDistance = 4.0f;
				navigationAgent2D.TargetDesiredDistance = 4.0f;
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (navigationAgent2D == null)
				return;
			if (navigationAgent2D.IsTargetReached())
			{
				GD.Print("MainPlayer, Target position reached at {GlobalPosition}");
				return;
			}
			
			var current_agent_position = GlobalPosition;// global_transform.origin
			var next_path_position = navigationAgent2D.GetNextPathPosition();
			Vector2 velocity = next_path_position - current_agent_position;
			if (velocity.Length() < 4.0f)
				return;
			velocity = velocity.Normalized();
			velocity = velocity * Speed;

			Velocity = velocity;
			MoveAndSlide();
		}

		/*
		public override void _PhysicsProcess(double delta)
		{
			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
				velocity.y += gravity * (float)delta;

			// Handle Jump.
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
				velocity.y = JumpVelocity;

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (direction != Vector2.Zero)
			{
				velocity.x = direction.x * Speed;
			}
			else
			{
				velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
			}

			Velocity = velocity;
			MoveAndSlide();
		}
		*/
	}
}
