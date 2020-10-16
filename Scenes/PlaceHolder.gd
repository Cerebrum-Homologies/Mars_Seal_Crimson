extends Node2D

var introSceneResource : String = "res://Scenes/LandingAbruptly#1.tscn"
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func changeScene(sceneResource : String):
	#get_tree().change_scene("res://game/levels/fixed1.tscn")
	get_tree().change_scene(sceneResource)

func _on_ButtonIntroStart_pressed():
	changeScene(introSceneResource)
