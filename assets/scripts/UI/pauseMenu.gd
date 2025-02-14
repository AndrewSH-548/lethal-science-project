extends Node2D

@export var visableOnPause = true;
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.


func _on_resume_pressed() -> void:
	print("resume pressed")
	hide()

func _on_options_pressed() -> void:
	print("options pressed")


func _on_quit_pressed() -> void:
	get_tree().quit()
