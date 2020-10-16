extends Node2D

var panel_name = ""
var screen_size

func _ready():
	screen_size = get_viewport().size
	#position = Vector2(80.0, 105.0)
	#align_blimp(speed)

func set_name(name):
	panel_name = name

func set_panel_color(modcolor : Color):
	$"Panel-Layout".modulate = modcolor

func set_title(fontsize : int, title : String):
	$"Panel-Layout/Label-Title".text = title
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
