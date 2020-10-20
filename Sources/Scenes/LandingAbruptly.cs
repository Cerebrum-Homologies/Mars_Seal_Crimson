using Godot;
using Godot.Collections;
using System.Linq;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
	public enum EnumScene1State
	{
		SCENE1_STATE_WAIT,
		SCENE1_STATE_START_WALK,
		SCENE1_STATE_NARRATIVE1,
		SCENE1_STATE_NARRATIVE2,
		SCENE1_STATE_SWITCH_SCENE,

	}

	public class LandingAbruptly : Spatial
	{
		const string nextSceneResource = "res://Scenes/ForestClearing#2.tscn";
		const string panelInfoResource = "res://Scenes/UI/PanelInfo.tscn";
		private EnumScene1State levelState = EnumScene1State.SCENE1_STATE_WAIT;
		private SceneUtilities sceneUtil;
		private Godot.Timer processTimer;
		private Godot.Camera sceneCamera;
		private Godot.CanvasLayer canvasOverlay;
		private Godot.AnimationPlayer animationPlayer;
		private Godot.AnimatedSprite playerCharacter;
		private Navigation2D navigationPlayerPath;
		private PanelInfo helpBoard;
		private bool flagSceneExitAllowed = true;
		private bool switchSceneTrigger = false;
		private bool playerWalkAction = false;
		private bool moveNode = false;
		private int processTimerCounter = 0;
		private float walkSpeed = 250.0f;
		private Vector2[] pathPlotPlayer = new Vector2[] { };

		public delegate void ElegantExitDelegate();
		public event ElegantExitDelegate ElegantExit;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on IntroScene");
			sceneUtil = new SceneUtilities();
			sceneUtil.CleanPreviousScenes(this);
			processTimer = this.GetNodeOrNull<Godot.Timer>("Timer-Process");
			Diagnostics.PrintNullValueMessage(processTimer, "processTimer");
			if (processTimer != null)
			{
				//introTimer.WaitTime = 0.1f;
				processTimer.Connect("timeout", this, nameof(_on_ProcessTimer_timeout));
				processTimer.Start();
			}
			//buttonGameIntroStart = this.GetNodeOrNull<TextureButton>("Button-Intro-Start");
			//Diagnostics.PrintNullValueMessage(buttonGameIntroStart, "buttonGameIntroStart");
			canvasOverlay = this.GetNodeOrNull<Godot.CanvasLayer>("CanvasLayer");
			Diagnostics.PrintNullValueMessage(canvasOverlay, "canvasOverlay");
			if (canvasOverlay != null)
			{
				animationPlayer = canvasOverlay.GetNodeOrNull<Godot.AnimationPlayer>("AnimationPlayer");
				navigationPlayerPath = canvasOverlay.GetNodeOrNull<Navigation2D>("Navigation2D");
				playerCharacter = canvasOverlay.GetNodeOrNull<Godot.AnimatedSprite>("Animated-Player-Character");
			}
			ElegantExit += _on_ElegantExit;
			GeneratePanelInfo();
		}

		private void _on_HelpBoard_Ready()
		{
			GD.Print($"_on_HelpBoard_Ready(), helpBoard = {helpBoard.Name}");
			//storyArcBoard.SetTitle(storyCaption);
		}
		private void GeneratePanelInfo()
		{
			PackedScene viewportScene = (PackedScene)ResourceLoader.Load(panelInfoResource);
			if (viewportScene != null)
			{
				helpBoard = (PanelInfo)viewportScene.Instance();
				helpBoard.Connect("ready", this, nameof(_on_HelpBoard_Ready));
				GD.Print($"Loading PanelInfo from PackedScene, PanelInfo = {helpBoard.Name}");
				helpBoard.Visible = false;
				AddChild(helpBoard);
			}
		}

		private void StartAnimation(String action, bool loop = true)
		{
			if (animationPlayer != null)
			{
				animationPlayer.Play(action);
				animationPlayer.PlaybackActive = loop;
			}
		}

		private EnumScene1State GetLevelState(int ticker)
		{
			var lState = EnumScene1State.SCENE1_STATE_WAIT;
			if (levelState != EnumScene1State.SCENE1_STATE_WAIT)
				lState = levelState;
			if (ticker == 30)
				lState = EnumScene1State.SCENE1_STATE_START_WALK;
			if (ticker == 210)
				lState = EnumScene1State.SCENE1_STATE_NARRATIVE1;
			//if (ticker == 360)
			//    lState = EnumScene1State.SCENE1_STATE_SWITCH_SCENE;
			if (levelState != lState) {
				lState = levelState;
			}
			return lState;
		}

		public void _on_ProcessTimer_timeout()
		{
			processTimerCounter += 1;
			levelState = GetLevelState(processTimerCounter);
			switch (levelState)
			{
				case EnumScene1State.SCENE1_STATE_WAIT:
					{
						break;
					}
				case EnumScene1State.SCENE1_STATE_START_WALK:
					{
						StartAnimation("Walk1");
						break;
					}
				case EnumScene1State.SCENE1_STATE_NARRATIVE1:
					{
						break;
					}
				case EnumScene1State.SCENE1_STATE_SWITCH_SCENE:
					{
						sceneUtil.ChangeScene(this, nextSceneResource);
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
				processTimer.Disconnect("timeout", this, nameof(_on_ProcessTimer_timeout));
				processTimer.Stop();
			}
			for (int ix = 0; ix < 10; ix++)
			{
				System.Threading.Thread.Sleep(200);
			}
			sceneUtil.ChangeScene(this, nextSceneResource);
		}

		private bool ShowInGameMenu()
		{
			bool res = false;
			SceneUtilities.ExitApplication(this);
			return res;
		}

		private void ShowContextHelp(EnumScene1State currState)
		{

		}

		private void ShowInventory() {

		}

		private void PerformPlayerWalkTo(Vector2 startPosition, Vector2 clickPosition)
		{
			if ((startPosition != null) && (clickPosition != null))
			{
				pathPlotPlayer = navigationPlayerPath.GetSimplePath(startPosition, clickPosition);
				pathPlotPlayer = pathPlotPlayer.Skip(1).Take(pathPlotPlayer.Length - 1).ToArray();
			}
		}

		private void TestPlayerPosition(Vector2 playerPosition) {
			float coordExitScreenX = 1540.0f; // Should be calculated proportionally using the viewport dimension
			if (playerPosition.x >= coordExitScreenX) {
				levelState = EnumScene1State.SCENE1_STATE_SWITCH_SCENE;
			}
		}

		private void PlayerWalkAction(float speed) {
			var lastPosition = playerCharacter.Position;
			for (var x = 0; x < pathPlotPlayer.Length; x++)
			{
				var distanceBetweenPoints = lastPosition.DistanceTo(pathPlotPlayer[x]);
				if (speed <= distanceBetweenPoints)
				{
					playerCharacter.Position = lastPosition.LinearInterpolate(pathPlotPlayer[x], speed / distanceBetweenPoints);
					TestPlayerPosition(playerCharacter.Position);
					break;
				}
				
				if (speed < 0.0) 
				{
					playerCharacter.Position = pathPlotPlayer[0];
					moveNode = false;
					break;
				}

				speed -= distanceBetweenPoints;
				lastPosition = pathPlotPlayer[x];
				pathPlotPlayer = pathPlotPlayer.Skip(1).Take(pathPlotPlayer.Length - 1).ToArray();
			}
			if (pathPlotPlayer.Length <= 0) {
				moveNode = false;
			}
		}

		public override void _Input(InputEvent @event)
		{
			//if ((levelState != EnumScene1State.SCENE1_STATE_WAIT) 
			/* && (levelState != EnumScene1State.SCENE1_STATE_START_WALK) )*/
			{
				// Mouse in viewport coordinates.
				if (@event is InputEventMouseButton eventMouseButton)
				{
					if (eventMouseButton.IsPressed())
					{
						GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
						playerWalkAction = true;
						moveNode = true;
						PerformPlayerWalkTo(playerCharacter.Position, eventMouseButton.Position);
					}
				}
				else if (@event is InputEventMouseMotion eventMouseMotion)
					GD.Print("Mouse Motion at: ", eventMouseMotion.Position);

				// Print the size of the viewport.
				//GD.Print("Viewport Resolution is: ", GetViewportRect().Size);
			}
		}

		public override void _Process(float delta)
		{
			double x = 0.0;
			double y = 0.0;
			float speed = walkSpeed;

			if (moveNode)
			{
				PlayerWalkAction(speed * delta);
			}
			if (Input.IsActionPressed("ui_cancel"))
			{
				ShowInGameMenu();
				return;
			}
			if (Input.IsActionPressed("ui_help"))
			{
				ShowContextHelp(levelState);
			}
			if (Input.IsActionPressed("ui_inventory"))
			{
				ShowInventory();
			}
			/*
			if (levelState != EnumScene1State.SCENE1_STATE_WAIT)
			{
				if (Input.IsMouseButtonPressed(Godot.Button))
				{
					PerformPlayerWalkTo(GetViewport().GetMousePosition());
				}
			}
			*/
		}
	}

}
