using Godot;
using System;

namespace Mars_Seal_Crimson
{
	public class PlaceHolder : Node2D
	{
		private TextureButton buttonGameIntroStart = null;
		private SceneUtilities sceneUtil;
		private Godot.AnimatedSprite playerCharacter;
		const string nextSceneResource = "res://Scenes/LandingAbruptly#1.tscn";
		public delegate void IntroLaunchDelegate();
		public event IntroLaunchDelegate IntroLaunch;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on PlaceHolder");
			Diagnostics.PrintChildrenList("PlaceHolder -- Tree :: ", this);
			sceneUtil = new SceneUtilities();
			//sceneUtil.CleanPreviousScenes(this);
			buttonGameIntroStart = this.GetNodeOrNull<TextureButton>("Button-Intro-Start");
			Diagnostics.PrintNullValueMessage(buttonGameIntroStart, "buttonGameIntroStart");
			playerCharacter = this.GetNodeOrNull<Godot.AnimatedSprite>("Animated-Player-Character");
			playerCharacter.Play("default");
			if (buttonGameIntroStart != null)
			{
				buttonGameIntroStart.Connect("pressed", this, nameof(_onButtonGameInto_pressed));
				IntroLaunch += _on_IntroLaunch;
			}
		}

		private void _on_IntroLaunch()
		{
			playerCharacter.Stop();
			for (int ix = 0; ix < 4; ix++)
			{
				System.Threading.Thread.Sleep(100);
			}
			sceneUtil.ChangeSceneFrom2D(this, nextSceneResource);
		}

		private void _onButtonGameInto_pressed()
		{
			GD.Print("_onButtonGameInto_pressed, changing scenes");
			IntroLaunch.Invoke();
		}
	}
}
