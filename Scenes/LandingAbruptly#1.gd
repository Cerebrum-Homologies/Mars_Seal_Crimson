extends Node3D

var panelinfo_class = preload("res://Scenes/UI/PanelInfo.tscn")
var forestSceneResource : String = "res://Scenes/ForestClearing#2.tscn"

var timer_ticks = 0
var help_toggle = false
var pnl_helper = null

func changeScene(sceneResource : String):
	get_tree().change_scene_to_file(sceneResource)

func create_panel_info(modcolor : Color):
	var pnl_info = panelinfo_class.instantiate()
	if modcolor.r > 0.01 or modcolor.g > 0.01 or modcolor.b > 0.01:
		pnl_info.set_panel_color(modcolor)
	return pnl_info

func player_walk_path():
	$CanvasLayer/AnimationPlayer.play("Walk1")

# Called when the node enters the scene tree for the first time.
func _ready():
	#Find the width and height of the viewport
	$"CanvasLayer/Animated-Player-Character".playing = true
	$"Timer-Process".start()

func _input(event):
	if event.is_action_pressed("ui_cancel"):
		get_tree().quit()
	if event.is_action_pressed("ui_help"):
		$"Timer-Process".stop()
		help_toggle = !help_toggle
		if help_toggle == true:
			if pnl_helper == null:
				pnl_helper = create_panel_info(Color.BISQUE)
				pnl_helper.position = Vector2(80, 50)
				pnl_helper.set_title(20, 'In-game help')
				add_child(pnl_helper)
			pnl_helper.visible = true
		else:
			pnl_helper.visible = false
		
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func _on_TimerProcess_timeout():
	timer_ticks += 1
	if timer_ticks == 20:
		player_walk_path()
	if timer_ticks == 160:
		changeScene(forestSceneResource)
