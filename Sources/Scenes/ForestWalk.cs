using Godot;
using System;

namespace Mars_Seal_Crimson
{

	public enum EnumScene3State
	{
		SCENE3_STATE_WAIT,
		SCENE3_STATE_BIG_FOOT,
		SCENE3_STATE_NARRATIVE1,
		SCENE3_STATE_NARRATIVE2,
		SCENE3_STATE_NARRATIVE3,
		SCENE3_STATE_SWITCH_SCENE,

	}

	public class ForestWalk : Spatial
	{
		const string nextSceneResource = "res://Scenes/DeepRavine#4.tscn";
		private EnumScene3State levelState = EnumScene3State.SCENE3_STATE_WAIT;
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
			GD.Print("Ready on ForestWalk");
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

		private EnumScene3State GetLevelState(int ticker) {
			var lState = EnumScene3State.SCENE3_STATE_WAIT;
			return lState;
		}

		public void _on_ProcessTimer_timeout()
		{
			processTimerCounter += 1;
			levelState = GetLevelState(processTimerCounter);
			switch (levelState)
			{
				case EnumScene3State.SCENE3_STATE_WAIT:
					{
						break;
					}
				case EnumScene3State.SCENE3_STATE_NARRATIVE1:
					{
						break;
					}
				case EnumScene3State.SCENE3_STATE_NARRATIVE2:
					{
						break;
					}
				case EnumScene3State.SCENE3_STATE_SWITCH_SCENE:
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

		private void ShowContextHelp(EnumScene3State currState)
		{
			string hint = "Keep pn playing ...";
			switch (currState) {
				case EnumScene3State.SCENE3_STATE_WAIT: {
					hint = "Be patient.\n You need to see the forest for the trees\n";
					break;
				}
				default: {
					break;
				}
			}
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
