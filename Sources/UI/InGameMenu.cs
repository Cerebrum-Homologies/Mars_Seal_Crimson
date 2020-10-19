using Godot;
using System;

namespace Mars_Seal_Crimson
{
	public class InGameMenu : Control
	{
		private Panel panelGameMenu;
		private Button buttonExitGame = null;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on InGameMenu");
			panelGameMenu = this.GetNodeOrNull<Panel>("MenuFrame");
			Diagnostics.PrintNullValueMessage(panelGameMenu, "panelGameMenu");
			if (panelGameMenu != null)
			{
				buttonExitGame = panelGameMenu.GetNodeOrNull<Button>("button-Exit-Game");
				Diagnostics.PrintNullValueMessage(buttonExitGame, "buttonExitGame");
				buttonExitGame.Connect("pressed", this, nameof(_onButtonExitGame_pressed));
			}
		}

		private void _onButtonExitGame_pressed()
		{
			SceneUtilities.ExitApplication(this);
		}
		//  // Called every frame. 'delta' is the elapsed time since the previous frame.
		//  public override void _Process(float delta)
		//  {
		//      
		//  }
	}
}
