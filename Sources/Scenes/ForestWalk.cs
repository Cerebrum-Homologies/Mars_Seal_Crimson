using Godot;
using System;
using System.Collections.Generic;

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
        const string panelInfoResource = "res://Scenes/UI/PanelInfo.tscn";
        const string inventoryBoxResource = "res://Scenes/UI/InventoryChest.tscn";
        const string sceneDescriptor = "DeepRavine";
        private EnumScene3State levelState = EnumScene3State.SCENE3_STATE_WAIT;
        private SceneUtilities sceneUtil;
        private Godot.Timer processTimer;
        private Godot.Camera sceneCamera;
        private Godot.CanvasLayer canvasOverlay;
        private Godot.AnimationPlayer animationPlayer;
        private Godot.AnimatedSprite playerCharacter;
        private Navigation2D navigationPlayerPath;
        private PanelInfo helpBoard;
        private InventoryChest inventoryChest;
        private CharacterMovement gamePlayerMovement;
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
            GD.Print("Ready on ForestWalk");
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
            if (canvasOverlay != null)
            {
                animationPlayer = canvasOverlay.GetNodeOrNull<Godot.AnimationPlayer>("AnimationPlayer");
                navigationPlayerPath = canvasOverlay.GetNodeOrNull<Navigation2D>("Navigation2D");
                playerCharacter = canvasOverlay.GetNodeOrNull<Godot.AnimatedSprite>("Animated-Player-Character");
                gamePlayerMovement = new CharacterMovement(playerCharacter, navigationPlayerPath, TestPlayerPosition);
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
                helpBoard = (PanelInfo)viewportScene.Instance();
                helpBoard.Connect("ready", this, nameof(_on_HelpBoard_Ready));
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
                inventoryChest = (InventoryChest)childScene.Instance();
                GD.Print($"Loading storyArcBoard from PackedScene, storyArcBoard = {inventoryChest.Name}");
                //playerViewport.AddChild(viewportScene.Instance());
                inventoryChest.Visible = false;
                //storyArcBoard.Connect("ready", this, nameof(_on_StoryBoardArc_Ready));
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
                inventoryChest.RectPosition = leftTop;
            }
        }

        private void TestPlayerPosition(Vector2 playerPosition)
        {
            //GetViewportRect().Size;
            float coordExitScreenX = 1580.0f; // Should be calculated proportionally using the viewport dimension
            if (playerPosition.x >= coordExitScreenX)
            {
                levelState = EnumScene3State.SCENE3_STATE_SWITCH_SCENE;
            }
        }

        private EnumScene3State GetLevelState(int ticker)
        {
            var lState = EnumScene3State.SCENE3_STATE_WAIT;
            if (ticker == 260)
                lState = EnumScene3State.SCENE3_STATE_SWITCH_SCENE;
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
            GD.Print($"Exiting scene, {sceneDescriptor}");
            if (processTimer != null)
            {
                processTimer.Disconnect("timeout", this, nameof(_on_ProcessTimer_timeout));
                processTimer.Stop();
            }
            for (int ix = 0; ix < 4; ix++)
            {
                System.Threading.Thread.Sleep(100);
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
            string hint = "Keep on playing ...";
            switch (currState)
            {
                case EnumScene3State.SCENE3_STATE_WAIT:
                    {
                        hint = "Be patient.\n You need to see the forest for the trees\n";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void ShowInventory()
        {
            toggleInventory = !toggleInventory;
            DisplayInventoryChest(positionChest, toggleInventory);
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
                        if (eventMouseButton.ButtonIndex == (int)ButtonList.Left)
                        {
                            GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
                            moveNode = true;
                            gamePlayerMovement.PerformPlayerWalkTo(playerCharacter.Position, eventMouseButton.Position);
                        }
                        if (eventMouseButton.ButtonIndex == (int)ButtonList.Right) //Used for the secondary action -- picking things up , etc..
                        {
                            if (PickupObjectWithinRadius(detectPickupRadius, eventMouseButton.Position))
                            {

                            }
                        }
                    }
                }
            }
        }

        public override void _Process(float delta)
        {
            double x = 0.0;
            double y = 0.0;
            float speed = walkSpeed;

            if (moveNode)
            {
                gamePlayerMovement.PlayerWalkAction(speed * delta);
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
        }
    }
}
