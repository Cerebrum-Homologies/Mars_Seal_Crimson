using Godot;
using System;

namespace Mars_Seal_Crimson
{
	public enum EnumScene2State
	{
		SCENE2_STATE_WAIT,
		SCENE2_STATE_START_WALK,
		SCENE2_STATE_NARRATIVE1,
		SCENE2_STATE_NARRATIVE2,
		SCENE2_STATE_SWITCH_SCENE,

	}
	public class ForestClearing : Spatial
	{
		const string nextSceneResource = "res://Scenes/ForestWalk#3.tscn";
		private EnumScene2State levelState = EnumScene2State.SCENE2_STATE_WAIT;
		private SceneUtilities sceneUtil;
		private Godot.Timer processTimer;
		private Godot.Camera sceneCamera;
		private Godot.CanvasLayer canvasOverlay;
		private Godot.AnimationPlayer animationPlayer;
		private bool flagSceneExitAllowed = true;
		private bool switchSceneTrigger = false;
		private int processTimerCounter = 0;
		private double walkSpeed = 25.0;

		public delegate void ElegantExitDelegate();
		public event ElegantExitDelegate ElegantExit;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on ForestClearing");
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
		}

		private EnumScene2State GetLevelState(int ticker)
		{
			var lState = EnumScene2State.SCENE2_STATE_WAIT;
			if (ticker == 260)
				lState = EnumScene2State.SCENE2_STATE_SWITCH_SCENE;
			return lState;
		}

		public void _on_ProcessTimer_timeout()
		{
			processTimerCounter += 1;
			levelState = GetLevelState(processTimerCounter);
			switch (levelState)
			{
				case EnumScene2State.SCENE2_STATE_WAIT:
					{
						break;
					}
				case EnumScene2State.SCENE2_STATE_START_WALK:
					{
						break;
					}
				case EnumScene2State.SCENE2_STATE_NARRATIVE1:
					{
						break;
					}
				case EnumScene2State.SCENE2_STATE_SWITCH_SCENE:
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
			for (int ix = 0; ix < 4; ix++)
			{
				System.Threading.Thread.Sleep(100);
			}
			GD.Print($"_on_ElegantExit, switch scene to {nextSceneResource}\n");
			sceneUtil.ChangeScene(this, nextSceneResource);
		}

		private bool ShowInGameMenu()
		{
			bool res = false;
			SceneUtilities.ExitApplication(this);
			return res;
		}

		private void ShowContextHelp(EnumScene2State currState)
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
