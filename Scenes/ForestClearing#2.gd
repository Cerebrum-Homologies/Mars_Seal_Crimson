extends Spatial

var panelinfo_class = preload("res://Scenes/UI/PanelInfo.tscn")
var forestDeepResource : String = "res://Scenes/ForestDeep#3.tscn"

var timer_ticks = 0
var help_toggle = false
var pnl_helper = null


func create_panel_info(modcolor : Color):
	var pnl_info = panelinfo_class.instance()
	if modcolor.r > 0.01 or modcolor.g > 0.01 or modcolor.b > 0.01:
		pnl_info.set_panel_color(modcolor)
	return pnl_info

func _ready():
	#Find the width and height of the viewport
	#$"CanvasLayer/Animated-Player-Character".playing = true
	#Adjust the sprite "world" layers as needed
	$"Timer-Process".start()

func _input(event):
	if event.is_action_pressed("ui_cancel"):
		get_tree().quit()
	if event.is_action_pressed("ui_help"):
		$"Timer-Process".stop()
		help_toggle = !help_toggle
		if help_toggle == true:
			if pnl_helper == null:
				pnl_helper = create_panel_info(Color.bisque)
				pnl_helper.position = Vector2(80, 50)
				pnl_helper.set_title(20, 'In-game help')
				add_child(pnl_helper)
			pnl_helper.visible = true
		else:
			pnl_helper.visible = false

func game_narrative_text(storyBlurb : String):
	if storyBlurb != null:
		$CanvasLayer/Label.text = storyBlurb

func _on_TimerProcess_timeout():
	timer_ticks += 1
	if timer_ticks == 80:
		game_narrative_text('There seems to be a path leading into the forest')
