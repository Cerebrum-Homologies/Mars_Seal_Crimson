using Godot;
using System;
using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
	public enum EnumScene4State
	{
		SCENE4_STATE_WAIT,
		SCENE4_STATE_START_WALK,
		SCENE4_STATE_NARRATIVE1,
		SCENE4_STATE_NARRATIVE2,
		SCENE4_STATE_SWITCH_SCENE,

	}
	public partial class DeepRavine : Node3D
	{
		const string nextSceneResource = "res://Scenes/ForestWalk#3.tscn";
		private EnumScene4State levelState = EnumScene4State.SCENE4_STATE_WAIT;
		private SceneUtilities sceneUtil;
		private Godot.Timer processTimer;
		private Godot.Camera3D sceneCamera;
		private Godot.CanvasLayer canvasOverlay;
		private Godot.AnimationPlayer animationPlayer;
		private main_player playerCharacter;
		private Godot.NavigationRegion2D navigationPlayerPath;
		private float detectPickupRadius = 120.0f;
		private Vector2 positionChest = new Vector2(30.0f, 80.0f);
		private List<(string value, Godot.Vector2 position)> listPickups = new List<(string value, Godot.Vector2 position)>();
		private bool flagSceneExitAllowed = true;
		private bool switchSceneTrigger = false;
		private int processTimerCounter = 0;
		private double walkSpeed = 25.0;

		public delegate void ElegantExitDelegate();
		public event ElegantExitDelegate ElegantExit;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on DeepRavine");
			sceneUtil = new SceneUtilities();
			sceneUtil.CleanPreviousScenes(this);
			processTimer = this.GetNodeOrNull<Godot.Timer>("Timer-Process");
			Diagnostics.PrintNullValueMessage(processTimer, "processTimer");
			if (processTimer != null)
			{
				//introTimer.WaitTime = 0.1f;
				processTimer.Connect("timeout",new Callable(this,nameof(_on_ProcessTimer_timeout)));
				processTimer.Start();
			}
			canvasOverlay = this.GetNodeOrNull<Godot.CanvasLayer>("CanvasLayer");
			if (canvasOverlay != null)
			{
				animationPlayer = canvasOverlay.GetNodeOrNull<Godot.AnimationPlayer>("AnimationPlayer");
				navigationPlayerPath = canvasOverlay.GetNodeOrNull<NavigationRegion2D>("NavigationRegionWalkPath");
				playerCharacter = navigationPlayerPath.GetNodeOrNull<main_player>("MainPlayer");
			}
			ElegantExit += _on_ElegantExit;
		}

		private EnumScene4State GetLevelState(int ticker)
		{
			var lState = EnumScene4State.SCENE4_STATE_WAIT;
			if (ticker == 260)
				lState = EnumScene4State.SCENE4_STATE_SWITCH_SCENE;
			return lState;
		}

		public void _on_ProcessTimer_timeout()
		{
			processTimerCounter += 1;
			levelState = GetLevelState(processTimerCounter);
			switch (levelState)
			{
				case EnumScene4State.SCENE4_STATE_WAIT:
					{
						break;
					}
				case EnumScene4State.SCENE4_STATE_START_WALK:
					{
						break;
					}
				case EnumScene4State.SCENE4_STATE_NARRATIVE1:
					{
						break;
					}
				case EnumScene4State.SCENE4_STATE_SWITCH_SCENE:
					{
						sceneUtil.ChangeSceneToFile(this, nextSceneResource);
						break;
					}
				default:
					{
						break;
					}
			}
			if (flagSceneExitAllowed)
			{
				if (switchSceneTrigger)
				{
					processTimer.WaitTime = 5.0f;
					GD.Print($"Switching scenes, Timer Counter = {processTimer}");
					processTimer.Stop();
					/* send a signal here */
					if (sceneUtil != null)
					{
						ElegantExit.Invoke();
					}
				}
			}
		}

		private void _on_ElegantExit()
		{
			if (processTimer != null)
			{
				processTimer.Disconnect("timeout",new Callable(this,nameof(_on_ProcessTimer_timeout)));
				processTimer.Stop();
			}
			for (int ix = 0; ix < 4; ix++)
			{
				System.Threading.Thread.Sleep(100);
			}
			GD.Print($"_on_ElegantExit, switch scene to {nextSceneResource}\n");
			sceneUtil.ChangeSceneToFile(this, nextSceneResource);
		}

		private bool ShowInGameMenu()
		{
			bool res = false;
			SceneUtilities.ExitApplication(this);
			return res;
		}

		private void ShowContextHelp(EnumScene4State currState)
		{

		}

		private bool PickupObjectWithinRadius(float radius, Vector2 clickPosition) {
			foreach ((string, Vector2) pickup in listPickups) {
				(string name, Vector2 posPick) = pickup;
				float pickDistance = posPick.DistanceTo(clickPosition);
				if (pickDistance <= radius)
					return true;
			}
			return false;
		}

		public override void _Input(InputEvent @event)
		{
			//if ((levelState != EnumScene2State.SCENE2_STATE_WAIT) 
			/* && (levelState != EnumScene2State.SCENE2_STATE_START_WALK) )*/
			{
				if (@event is InputEventMouseButton eventMouseButton)
				{
					if (eventMouseButton.IsPressed())
					{
						if (eventMouseButton.ButtonIndex == MouseButton.Left)
						{
							GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
							//moveNode = true;
							playerCharacter.MovePlayerToPosition(eventMouseButton.Position);
						}
						if (eventMouseButton.ButtonIndex == MouseButton.Right) //Used for the secondary action -- picking things up , etc..
						{
							if (PickupObjectWithinRadius(detectPickupRadius, eventMouseButton.Position)) {

							}
						}
					}
				}
			}
		}

		public override void _Process(double delta)
		{
			double speed = walkSpeed;

			if (Input.IsActionPressed("ui_cancel"))
			{
				ShowInGameMenu();
				return;
			}
			if (Input.IsActionPressed("ui_help"))
			{
				ShowContextHelp(levelState);
			}
		}
	}
}
