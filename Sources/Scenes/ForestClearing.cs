using Godot;
using System.Linq;
using System.Collections.Generic;
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
	public partial class ForestClearing : Node3D
	{
		const string nextSceneResource = "res://Scenes/ForestWalk#3.tscn";
		const string panelInfoResource = "res://Scenes/UI/PanelInfo.tscn";
		const string inventoryBoxResource = "res://Scenes/UI/InventoryChest.tscn";
		const string sceneDescriptor = "ForestClearing";
		private EnumScene2State levelState = EnumScene2State.SCENE2_STATE_WAIT;
		private SceneUtilities sceneUtil;
		private Godot.Timer processTimer;
		private Godot.Camera3D sceneCamera;
		private Godot.CanvasLayer canvasOverlay;
		private Godot.AnimationPlayer animationPlayer;
		private main_player playerCharacter;
		private Godot.NavigationRegion2D navigationPlayerPath;
		private PanelInfo helpBoard;
		private InventoryChest inventoryChest;
		//private CharacterMovement gamePlayerMovement;
		private bool flagSceneExitAllowed = true;
		private bool switchSceneTrigger = false;
		private bool moveNode = false;
		private bool toggleInventory = false;
		private bool toggleHelp = false;
		private float detectPickupRadius = 120.0f;
		private Vector2 positionChest = new Vector2(30.0f, 80.0f);
		private List<(string value, Godot.Vector2 position)> listPickups = new List<(string value, Godot.Vector2 position)>();
		private int processTimerCounter = 0;
		private float walkSpeed = 250.0f;

		public delegate void ElegantExitDelegate();
		public event ElegantExitDelegate ElegantExit;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Ready on ForestClearing");
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
			//buttonGameIntroStart = this.GetNodeOrNull<TextureButton>("Button-Intro-Start");
			//Diagnostics.PrintNullValueMessage(buttonGameIntroStart, "buttonGameIntroStart");
			canvasOverlay = this.GetNodeOrNull<Godot.CanvasLayer>("CanvasLayer");
			if (canvasOverlay != null)
			{
				animationPlayer = canvasOverlay.GetNodeOrNull<Godot.AnimationPlayer>("AnimationPlayer");
				navigationPlayerPath = canvasOverlay.GetNodeOrNull<NavigationRegion2D>("NavigationRegionWalkPath");
				playerCharacter = navigationPlayerPath.GetNodeOrNull<main_player>("MainPlayer");
				//gamePlayerMovement = new CharacterMovement(playerCharacter, navigationPlayerPath, TestPlayerPosition);
			}
			ElegantExit += _on_ElegantExit;
			GeneratePanelInfo();
			GenerateInventoryChest();
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
				helpBoard = (PanelInfo)viewportScene.Instantiate();
				helpBoard.Connect("ready",new Callable(this,nameof(_on_HelpBoard_Ready)));
				GD.Print($"Loading PanelInfo from PackedScene, PanelInfo = {helpBoard.Name}");
				helpBoard.Visible = false;
				AddChild(helpBoard);
			}
		}

		private void GenerateInventoryChest()
		{
			PackedScene childScene = (PackedScene)ResourceLoader.Load(inventoryBoxResource);
			if (childScene != null)
			{
				inventoryChest = (InventoryChest)childScene.Instantiate();
				GD.Print($"Loading storyArcBoard from PackedScene, storyArcBoard = {inventoryChest.Name}");
				//playerViewport.AddChild(viewportScene.Instance());
				inventoryChest.Visible = false;
				//storyArcBoard.Connect("ready",new Callable(this,nameof(_on_StoryBoardArc_Ready)));
				AddChild(inventoryChest);
			}
		}

		private void DisplayInventoryChest(Vector2 leftTop, bool show = true)
		{ // = const Vector2(30.0f, 20.0f)
			if (inventoryChest == null)
				return;
			inventoryChest.Visible = show;
			GD.Print($"DisplayInventoryChest(), inventoryChest.Visible = {inventoryChest.Visible}");
			if (show)
			{
				inventoryChest.Position = leftTop;
			}
		}

		private EnumScene2State GetLevelState(int ticker)
		{
			var lState = EnumScene2State.SCENE2_STATE_WAIT;
			//if (ticker == 260)
			//	lState = EnumScene2State.SCENE2_STATE_SWITCH_SCENE;
			if (levelState != EnumScene2State.SCENE2_STATE_WAIT)
				lState = levelState;
			if (levelState != lState)
			{
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
						ElegantExit.Invoke();
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
				processTimer.Disconnect("timeout", new Callable(this,nameof(_on_ProcessTimer_timeout)));
				processTimer.Stop();
			}
			for (int ix = 0; ix < 4; ix++)
			{
				System.Threading.Thread.Sleep(100);
			}
			GD.Print($"_on_ElegantExit, switch scene to {nextSceneResource}\n");
			sceneUtil.ChangeSceneToFile(this, nextSceneResource);
		}

		private void TestPlayerPosition(Vector2 playerPosition)
		{
			float coordExitScreenX = SceneUtilities.GetApplicationWindowExtent(this).Size.X - 80.0f;
			if (playerPosition.X >= coordExitScreenX)
			{
				GD.Print($"TestPlayerPosition(), playerPosition = {playerPosition}, levelState = {levelState}");
				levelState = EnumScene2State.SCENE2_STATE_SWITCH_SCENE;
			}
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
							moveNode = true;
							playerCharacter.MovePlayerToPosition(eventMouseButton.Position);
						}
						if (eventMouseButton.ButtonIndex == MouseButton.Right) //Used for the secondary action -- picking things up , etc..
						{
							if (PickupObjectWithinRadius(detectPickupRadius, eventMouseButton.Position)) {

							}
						}
					}
				}
				//else if (@event is InputEventMouseMotion eventMouseMotion)
				//	GD.Print("Mouse Motion at: ", eventMouseMotion.Position);

				// Print the size of the viewport.
				//GD.Print("SubViewport Resolution is: ", GetViewportRect().Size);
			}
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

		private void ShowInventory()
		{
			toggleInventory = !toggleInventory;
			DisplayInventoryChest(positionChest, toggleInventory);
		}

		public override void _Process(double delta)
		{
			double speed = walkSpeed;

			if (moveNode)
			{
				//gamePlayerMovement.PlayerWalkAction(speed * delta);
			}
			if (Input.IsActionPressed("ui_cancel"))
			{
				ShowInGameMenu();
				return;
			}
			if (Input.IsActionPressed("ui_help"))
			{
				if (InputAssistance.KeyBounceCheckAlternative("ui_help", 0.25f, 0.75f))
					ShowContextHelp(levelState);
			}
			if (Input.IsActionPressed("ui_inventory"))
			{
				if (InputAssistance.KeyBounceCheckAlternative("ui_inventory", 0.25f, 0.75f))
				{
					GD.Print("Inventory input key pressed\n");
					ShowInventory();
				}
			}
			TestPlayerPosition(playerCharacter.Position);
		}
	}
}
