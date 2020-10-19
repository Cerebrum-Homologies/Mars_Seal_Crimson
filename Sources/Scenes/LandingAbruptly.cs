using Godot;
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
		private PanelInfo helpBoard;
		private bool flagSceneExitAllowed = true;
		private bool switchSceneTrigger = false;
		private int processTimerCounter = 0;
		private double walkSpeed = 25.0;

		public delegate void ElegantExitDelegate();
		public event ElegantExitDelegate ElegantExit;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on IntroScene");
			sceneUtil = new SceneUtilities();
			sceneUtil.CleanPreviousScenes(this);
			processTimer = this.GetNodeOrNull<Godot.Timer>("Timer-Process");
			if (processTimer != null)
			{
				//introTimer.WaitTime = 0.1f;
				processTimer.Connect("timeout", this, nameof(_on_ProcessTimer_timeout));
				processTimer.Start();
			}
			//buttonGameIntroStart = this.GetNodeOrNull<TextureButton>("Button-Intro-Start");
			//Diagnostics.PrintNullValueMessage(buttonGameIntroStart, "buttonGameIntroStart");
			canvasOverlay = this.GetNodeOrNull<Godot.CanvasLayer>("CanvasLayer");
			if (canvasOverlay != null)
			{
				animationPlayer = canvasOverlay.GetNodeOrNull<Godot.AnimationPlayer>("AnimationPlayer");
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
				GD.Print($"Loading storyArcBoard from PackedScene, storyArcBoard = {helpBoard.Name}");
				helpBoard.Visible = false;
				AddChild(helpBoard);
			}
		}

		private void StartAnimation(String action)
		{
			if (animationPlayer != null)
			{
				animationPlayer.Play(action);
			}
		}

		private EnumScene1State GetLevelState(int ticker)
		{
			var lState = EnumScene1State.SCENE1_STATE_WAIT;
			if (ticker == 30)
				lState = EnumScene1State.SCENE1_STATE_START_WALK;
			if (ticker == 260)
				lState = EnumScene1State.SCENE1_STATE_SWITCH_SCENE;
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
				processTimer.Stop();
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

		public override void _Process(float delta)
		{
			double x = 0.0;
			double y = 0.0;
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
